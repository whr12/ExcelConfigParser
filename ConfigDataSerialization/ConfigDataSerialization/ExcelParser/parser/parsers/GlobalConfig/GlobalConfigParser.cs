using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型2 — 全局配置解析器门面。
    /// </summary>
    public class GlobalConfigParser : IExcelParser
    {
        private readonly ParseConfig _config;
        private readonly GlobalConfigFbsGenerator _fbsGen;
        private readonly GlobalConfigJsonSerializer _jsonGen;
        private readonly GlobalConfigWrapperCreator _wrapperCreator;

        public GlobalConfigParser(ParseConfig config)
        {
            _config = config;
            _fbsGen = new GlobalConfigFbsGenerator();
            _jsonGen = new GlobalConfigJsonSerializer();
            _wrapperCreator = new GlobalConfigWrapperCreator(config.GlobalConfigTemplate);
        }

        public SheetDataInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();
            var baseName = sheetName.Substring(_config.GlobalPrefix.Length);

            var info = new GlobalConfigSheetInfo
            {
                ExcelFileName = reader.GetExcelName(),
                SheetName = sheetName,
                DefineName = ExcelParserHelper.ConvertSheetNameToDefineName(baseName),
                BinaryFileName = sheetName + ".bin",
                Namespace = _config.Namespace,
                ValueColumn = 5,
                Fields = new List<SingleDataDefine>(),
            };

            int rowCount = reader.GetSheetRowCount();
            for (int row = _config.DataStartRow; row <= rowCount; row++)
            {
                var nameValue = reader.ReadSheet(row, 2);
                var typeValue = reader.ReadSheet(row, 3);
                var commentValue = reader.ReadSheet(row, 1);

                if (string.IsNullOrEmpty(nameValue) || string.IsNullOrEmpty(typeValue))
                    continue;

                ExcelParserHelper.ValidateSnakeCase(nameValue, sheetName, reader.GetExcelName());

                var dataType = DataTypeFactory.ConvertToType(typeValue);
                if (dataType == null)
                {
                    Log.Error($"[{sheetName}] 无效类型: {typeValue}，行 {row}");
                    continue;
                }

                info.Fields.Add(new SingleDataDefine
                {
                    dataName = nameValue,
                    dataType = dataType,
                    columnIndex = row,
                    Comment = commentValue,
                });
            }

            return info;
        }

        public void GenerateFbs(SheetDataInfo info, string outputPath)
        {
            _fbsGen.Generate((GlobalConfigSheetInfo)info, outputPath);
        }

        public void SerializeJson(SheetDataInfo info, IExcelSheetReader reader, string outputPath)
        {
            _jsonGen.Serialize((GlobalConfigSheetInfo)info, reader, outputPath);
        }

        public void CreateWrapper(SheetDataInfo info, string outputPath)
        {
            _wrapperCreator.Create((GlobalConfigSheetInfo)info, outputPath);
        }
    }
}
