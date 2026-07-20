using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigDataSerialization.ExcelParser.parser.parsers;
using OfficeOpenXml;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// Excel → FlatBuffer 导出编排器。
    /// 只依赖 IExcelParser 接口和工厂方法，不持有任何具体 Parser 引用。
    /// 新增 Excel 类型只需在工厂中注册，编排器无改动。
    /// </summary>
    public class ExcelParser
    {
        private ParseConfig _config;
        private FlatBufferCompiler _flatc;

        public static ExcelParser Create(ParseConfig config)
        {
            if (string.IsNullOrEmpty(config.ExcelPath))
                throw new NullReferenceException($"Excel path is null or empty: {config.ExcelPath}");

            if (!Directory.Exists(config.ExcelPath))
                throw new IOException($"Excel path does not exist: {config.ExcelPath}");

            ExcelPackage.License.SetNonCommercialPersonal("Personal");

            EnsureDir(config.OutputPath);

            return new ExcelParser
            {
                _config = config,
                _flatc = new FlatBufferCompiler(config.FlatcPath),
            };
        }

        private ExcelParser() { }

        // ===== 工厂 =====

        /// <summary>
        /// 按 sheet 名前缀返回对应的 IExcelParser 实现。
        /// 新增数据类型只需在此添加 case，不改编排器逻辑。
        /// </summary>
        private static IExcelParser CreateParser(ParseConfig config, string sheetName)
        {
            if (sheetName.StartsWith(config.EnumPrefix))
                return new EnumParser(config);

            if (sheetName.StartsWith(config.GlobalPrefix))
                return new GlobalConfigParser(config);

            return new DataListParser(config);
        }

        // ===== 对外入口 =====

        public void ParseCode()
        {
            var parsedSheets = new Dictionary<string, string>();
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
                    if (IsIgnored(sheetName))
                        continue;

                    if (parsedSheets.TryGetValue(sheetName, out var firstFile))
                    {
                        var msg = $"[{firstFile}] 和 [{excelReader.GetExcelName()}] 存在同名 sheet: {sheetName}";
                        Log.Error(msg);
                        throw new System.Exception(msg);
                    }
                    parsedSheets[sheetName] = excelReader.GetExcelName();

                    var parser = CreateParser(_config, sheetName);
                    var info = parser.AnalyseSheet(excelReader);

                    parser.GenerateFbs(info, fbsDir);
                    fbsFiles.Add(Path.Combine(fbsDir, info.SheetName + ".fbs"));

                    parser.CreateWrapper(info,
                        Path.Combine(_config.OutputPath, "CS", "wrapper"));
                }
            }

            _flatc.CompileFbsToCSharp(fbsFiles,
                Path.Combine(_config.OutputPath, "CS", "flatbuffer"));
        }

        public void ParseData()
        {
            var parsedSheets = new Dictionary<string, string>();
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
                    if (IsIgnored(sheetName))
                        continue;

                    // 枚举类型不需要导出二进制数据
                    if (sheetName.StartsWith(_config.EnumPrefix))
                        continue;

                    if (parsedSheets.TryGetValue(sheetName, out var firstFile))
                    {
                        var msg = $"[{firstFile}] 和 [{excelReader.GetExcelName()}] 存在同名 sheet: {sheetName}";
                        Log.Error(msg);
                        throw new System.Exception(msg);
                    }
                    parsedSheets[sheetName] = excelReader.GetExcelName();

                    var parser = CreateParser(_config, sheetName);
                    var info = parser.AnalyseSheet(excelReader);

                    parser.SerializeJson(info, excelReader, jsonDir);
                    jsonFiles.Add(Path.Combine(jsonDir, info.SheetName + ".json"));
                    fbsFiles.Add(Path.Combine(_config.OutputPath, "fbs", info.SheetName + ".fbs"));
                }
            }

            _flatc.CompileJsonToBinary(jsonFiles, fbsFiles,
                Path.Combine(_config.OutputPath, "binary"));
        }

        // ===== 工具 =====

        private string[] ListExcelFiles()
        {
            return Directory.GetFiles(_config.ExcelPath, "*.xlsx", SearchOption.AllDirectories)
                .Where(f => !Path.GetFileName(f).StartsWith("~$"))
                .ToArray();
        }

        private bool IsIgnored(string sheetName)
        {
            return !string.IsNullOrEmpty(_config.InvalidPrefix)
                && sheetName.StartsWith(_config.InvalidPrefix);
        }

        private static void EnsureDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
