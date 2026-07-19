using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// 类型2 — 全局配置解析器。按行读取字段元数据（行列颠倒）。
    /// 实现 IExcelParser&lt;GlobalConfigSheetInfo&gt;，无状态。
    /// </summary>
    public class GlobalConfigParser : IExcelParser<GlobalConfigSheetInfo>
    {
        private readonly ParseConfig _config;

        public GlobalConfigParser(ParseConfig config)
        {
            _config = config;
        }

        public GlobalConfigSheetInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();
            var baseName = sheetName.Substring(_config.GlobalPrefix.Length);

            var info = new GlobalConfigSheetInfo
            {
                ExcelFileName = reader.GetExcelName(),
                SheetName = sheetName,
                DefineName = DefaultExcelParser.ConvertSheetNameToDefineName(baseName),
                BinaryFileName = sheetName + ".bin",
                Namespace = _config.Namespace,
                ValueColumn = 5,
                Fields = new List<FieldDefinition>(),
            };

            // 从 Row5 开始遍历每一行：Col2=字段名, Col3=类型名
            int rowCount = reader.GetSheetRowCount();
            for (int row = _config.DataStartRow; row <= rowCount; row++)
            {
                var nameValue = reader.ReadSheet(row, 2);
                var typeValue = reader.ReadSheet(row, 3);

                if (string.IsNullOrEmpty(nameValue) || string.IsNullOrEmpty(typeValue))
                    continue;

                info.Fields.Add(new FieldDefinition
                {
                    Name = nameValue,
                    TypeName = DefaultExcelParser.ConvertExcelTypeToFbsType(typeValue),
                    DataIndex = row,
                });
            }

            return info;
        }
    }
}
