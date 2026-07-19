using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型1 — 数据列表解析器门面。
    /// 组合 FbsGenerator / JsonSerializer / WrapperCreator，实现 IExcelParser。
    /// </summary>
    public class DataListParser : IExcelParser
    {
        private readonly ParseConfig _config;
        private readonly DataListFbsGenerator _fbsGen;
        private readonly DataListJsonSerializer _jsonGen;
        private readonly DataListWrapperCreator _wrapperCreator;

        public DataListParser(ParseConfig config)
        {
            _config = config;
            _fbsGen = new DataListFbsGenerator();
            _jsonGen = new DataListJsonSerializer();
            _wrapperCreator = new DataListWrapperCreator(config.DataListTemplate);
        }

        public SheetDataInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();

            var info = new DataListSheetInfo
            {
                ExcelFileName = reader.GetExcelName(),
                SheetName = sheetName,
                DefineName = ExcelParserHelper.ConvertSheetNameToDefineName(sheetName),
                BinaryFileName = sheetName + ".bin",
                Namespace = _config.Namespace,
                DataNameRow = _config.DataNameRow,
                DataTypeRow = _config.DataTypeRow,
                DataStartRow = _config.DataStartRow,
                Fields = new List<SingleDataDefine>(),
            };

            int commentRow = _config.DataNameRow - 1;

            for (int col = 1; col <= reader.GetSheetColumnCount(); col++)
            {
                var nameValue = reader.ReadSheet(_config.DataNameRow, col);
                var typeValue = reader.ReadSheet(_config.DataTypeRow, col);
                var commentValue = reader.ReadSheet(commentRow, col);

                if (string.IsNullOrEmpty(nameValue) || string.IsNullOrEmpty(typeValue))
                    continue;

                ExcelParserHelper.ValidateSnakeCase(nameValue, sheetName, reader.GetExcelName());

                var dataType = DataTypeFactory.ConvertToType(typeValue);
                if (dataType == null)
                {
                    Log.Error($"[{sheetName}] 无效类型: {typeValue}，列 {col}");
                    continue;
                }

                info.Fields.Add(new SingleDataDefine
                {
                    dataName = nameValue,
                    dataType = dataType,
                    columnIndex = col,
                    Comment = commentValue,
                });
            }

            return info;
        }

        public void GenerateFbs(SheetDataInfo info, string outputPath)
        {
            _fbsGen.Generate((DataListSheetInfo)info, outputPath);
        }

        public void SerializeJson(SheetDataInfo info, IExcelSheetReader reader, string outputPath)
        {
            _jsonGen.Serialize((DataListSheetInfo)info, reader, outputPath);
        }

        public void CreateWrapper(SheetDataInfo info, string outputPath)
        {
            _wrapperCreator.Create((DataListSheetInfo)info, outputPath);
        }
    }
}
