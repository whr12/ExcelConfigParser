using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型3 — 枚举定义解析器门面。
    /// 只需生成 .fbs，JSON 和 Wrapper 为空实现。
    /// </summary>
    public class EnumParser : IExcelParser
    {
        private readonly ParseConfig _config;
        private readonly EnumFbsGenerator _fbsGen;

        public EnumParser(ParseConfig config)
        {
            _config = config;
            _fbsGen = new EnumFbsGenerator();
        }

        public SheetDataInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();
            var baseName = sheetName.Substring(_config.EnumPrefix.Length);

            var info = new EnumSheetInfo
            {
                ExcelFileName = reader.GetExcelName(),
                SheetName = sheetName,
                DefineName = baseName,
                BinaryFileName = string.Empty,
                Namespace = _config.Namespace,
                Values = new List<EnumValueDefinition>(),
            };

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

        public void GenerateFbs(SheetDataInfo info, string outputPath)
        {
            _fbsGen.Generate((EnumSheetInfo)info, outputPath);
        }

        public void SerializeJson(SheetDataInfo info, IExcelSheetReader reader, string outputPath)
        {
            // 枚举不需要 json
        }

        public void CreateWrapper(SheetDataInfo info, string outputPath)
        {
            // 枚举不需要 wrapper，flatc 直接生成 C# enum
        }
    }
}
