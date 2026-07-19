using System;
using System.Collections.Generic;
using System.IO;
using ConfigDataSerialization.ExcelParser.parser;

namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// Excel → FlatBuffer 导出编排器。
    /// 按前缀选择 parser → 生成 .fbs / .json → flatc 编译 → 输出 wrapper。
    /// 对 SheetDataInfo 子类型做 switch 分发，编译器自动匹配泛型重载。
    /// </summary>
    public class ExcelParser
    {
        private ParseConfig _config;
        private DefaultExcelParser _defaultParser;
        private GlobalConfigParser _globalParser;
        private EnumParser _enumParser;
        private FbsCodeGenerator _fbsGen;
        private JsonDataGenerator _jsonGen;
        private WrapperCodeCreator _wrapperCreator;
        private FlatBufferCompiler _flatc;

        public static ExcelParser Create(ParseConfig config)
        {
            if (string.IsNullOrEmpty(config.ExcelPath))
                throw new NullReferenceException($"Excel path is null or empty: {config.ExcelPath}");

            if (!Directory.Exists(config.ExcelPath))
                throw new IOException($"Excel path does not exist: {config.ExcelPath}");

            var parser = new ExcelParser();
            parser.Init(config);
            return parser;
        }

        private ExcelParser() { }

        private void Init(ParseConfig config)
        {
            this._config = config;
            OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Personal");

            _defaultParser = new DefaultExcelParser(config);
            _globalParser = new GlobalConfigParser(config);
            _enumParser = new EnumParser(config);
            _fbsGen = new FbsCodeGenerator();
            _jsonGen = new JsonDataGenerator();
            _wrapperCreator = new WrapperCodeCreator(config.DataListTemplate, config.GlobalConfigTemplate);
            _flatc = new FlatBufferCompiler(config.FlatcPath);

            // 确保输出目录存在
            EnsureDir(config.OutputPath);
        }

        // ===== 对外入口 =====

        public void ParseCode()
        {
            var parsedSheets = new HashSet<string>();
            var fbsFiles = new List<string>();

            var fbsDir = Path.Combine(_config.OutputPath, "fbs");
            EnsureDir(fbsDir);

            foreach (var file in ListExcelFiles())
            {
                using var excelReader = ExcelReader.CreateExcelReader(file);
                for (int i = 0; i < excelReader.GetWorkSheetsCount(); i++)
                {
                    if (!excelReader.TrySwitchSheet(i))
                        continue;

                    var sheetName = excelReader.GetSheetName();
                    if (IsIgnored(sheetName, out _))
                        continue;

                    if (!parsedSheets.Add(sheetName))
                    {
                        Log.Error($"重复 sheet: {excelReader.GetExcelName()} → {sheetName}");
                        continue;
                    }

                    // 按类型解析并生成 .fbs
                    var info = AnalyseSheet(excelReader);
                    if (info == null) continue;

                    GenerateFbs(info, fbsDir, fbsFiles);
                }
            }

            // flatc: .fbs → C#
            _flatc.CompileFbsToCSharp(fbsFiles,
                Path.Combine(_config.OutputPath, "CS", "flatbuffer"));
        }

        public void ParseData()
        {
            var parsedSheets = new HashSet<string>();
            var jsonFiles = new List<string>();
            var fbsFiles = new List<string>();

            var jsonDir = Path.Combine(_config.OutputPath, "json");
            EnsureDir(jsonDir);

            foreach (var file in ListExcelFiles())
            {
                using var excelReader = ExcelReader.CreateExcelReader(file);
                for (int i = 0; i < excelReader.GetWorkSheetsCount(); i++)
                {
                    if (!excelReader.TrySwitchSheet(i))
                        continue;

                    var sheetName = excelReader.GetSheetName();
                    if (IsIgnored(sheetName, out var prefixTag))
                        continue;

                    if (!parsedSheets.Add(sheetName))
                    {
                        Log.Error($"重复 sheet: {excelReader.GetExcelName()} → {sheetName}");
                        continue;
                    }

                    // 枚举类型不需要生成 json 数据
                    if (prefixTag == _config.EnumPrefix)
                        continue;

                    // 解析并生成 .json
                    var info = AnalyseSheet(excelReader);
                    if (info == null) continue;

                    SerializeJson(info, excelReader, jsonDir, jsonFiles);
                    fbsFiles.Add(Path.Combine(_config.OutputPath, "fbs", info.SheetName + ".fbs"));
                }
            }

            // flatc: .json + .fbs → .bin
            _flatc.CompileJsonToBinary(jsonFiles, fbsFiles,
                Path.Combine(_config.OutputPath, "binary"));
        }

        // ===== 内部流程 =====

        private SheetDataInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();

            if (sheetName.StartsWith(_config.EnumPrefix))
                return _enumParser.AnalyseSheet(reader);

            if (sheetName.StartsWith(_config.GlobalPrefix))
                return _globalParser.AnalyseSheet(reader);

            return _defaultParser.AnalyseSheet(reader);
        }

        private void GenerateFbs(SheetDataInfo info, string outputDir, List<string> fbsFiles)
        {
            switch (info)
            {
                case DataListSheetInfo dl:
                    _fbsGen.Generate(dl, outputDir);
                    break;
                case GlobalConfigSheetInfo gc:
                    _fbsGen.Generate(gc, outputDir);
                    break;
                case EnumSheetInfo e:
                    _fbsGen.Generate(e, outputDir);
                    break;
            }

            var path = Path.Combine(outputDir, info.SheetName + ".fbs");
            fbsFiles.Add(path);

            // 生成 wrapper（枚举不需要）
            var wrapperDir = Path.Combine(_config.OutputPath, "CS", "wrapper");
            switch (info)
            {
                case DataListSheetInfo dl:
                    _wrapperCreator.CreateWrapper(dl, wrapperDir);
                    break;
                case GlobalConfigSheetInfo gc:
                    _wrapperCreator.CreateWrapper(gc, wrapperDir);
                    break;
            }
        }

        private void SerializeJson(SheetDataInfo info, IExcelSheetReader reader, string outputDir, List<string> jsonFiles)
        {
            switch (info)
            {
                case DataListSheetInfo dl:
                    _jsonGen.Serialize(dl, reader, outputDir);
                    break;
                case GlobalConfigSheetInfo gc:
                    _jsonGen.Serialize(gc, reader, outputDir);
                    break;
            }

            jsonFiles.Add(Path.Combine(outputDir, info.SheetName + ".json"));
        }

        // ===== 工具 =====

        private string[] ListExcelFiles()
        {
            return Directory.GetFiles(_config.ExcelPath, "*.xlsx", SearchOption.AllDirectories);
        }

        private bool IsIgnored(string sheetName, out string matchedPrefix)
        {
            matchedPrefix = null;

            if (string.IsNullOrEmpty(sheetName))
                return true;

            foreach (var prefix in new[] { _config.InvalidPrefix, _config.EnumPrefix, _config.GlobalPrefix })
            {
                if (!string.IsNullOrEmpty(prefix) && sheetName.StartsWith(prefix))
                {
                    matchedPrefix = prefix;
                    // 只有 InvalidPrefix 需要跳过；其他前缀需要处理（但这里做前缀标记）
                    return prefix == _config.InvalidPrefix;
                }
            }

            return false;
        }

        private static void EnsureDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
