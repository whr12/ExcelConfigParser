using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// 类型3 — 枚举定义解析器。按行读取枚举值定义。
    /// 实现 IExcelParser&lt;EnumSheetInfo&gt;，无状态。
    /// </summary>
    public class EnumParser : IExcelParser<EnumSheetInfo>
    {
        private readonly ParseConfig _config;

        public EnumParser(ParseConfig config)
        {
            _config = config;
        }

        public EnumSheetInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();
            var baseName = sheetName.Substring(_config.EnumPrefix.Length);

            var info = new EnumSheetInfo
            {
                ExcelFileName = reader.GetExcelName(),
                SheetName = sheetName,
                DefineName = DefaultExcelParser.ConvertSheetNameToDefineName(baseName),
                BinaryFileName = null,  // 枚举不需要 .bin
                Namespace = _config.Namespace,
                Values = new List<EnumValueDefinition>(),
            };

            // 从 DataStartRow 开始遍历：Col1=数值, Col2=枚举名, Col3=备注
            int rowCount = reader.GetSheetRowCount();
            for (int row = _config.DataStartRow; row <= rowCount; row++)
            {
                var valueText = reader.ReadSheet(row, 1);
                var nameText = reader.ReadSheet(row, 2);
                var commentText = reader.ReadSheet(row, 3);

                if (string.IsNullOrEmpty(nameText))
                    continue;

                int.TryParse(valueText, out int value);

                info.Values.Add(new EnumValueDefinition
                {
                    Value = value,
                    Name = nameText,
                    Comment = commentText,
                });
            }

            return info;
        }
    }
}
