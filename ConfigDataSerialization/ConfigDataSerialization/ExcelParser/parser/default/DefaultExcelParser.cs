using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// 类型1 — 数据列表解析器。按列读取字段元数据。
    /// 实现 IExcelParser&lt;DataListSheetInfo&gt;，无状态，AnalyseSheet 为纯函数。
    /// </summary>
    public class DefaultExcelParser : IExcelParser<DataListSheetInfo>
    {
        private readonly ParseConfig _config;

        public DefaultExcelParser(ParseConfig config)
        {
            _config = config;
        }

        public DataListSheetInfo AnalyseSheet(IExcelSheetReader reader)
        {
            var sheetName = reader.GetSheetName();

            var info = new DataListSheetInfo
            {
                ExcelFileName = reader.GetExcelName(),
                SheetName = sheetName,
                DefineName = ConvertSheetNameToDefineName(sheetName),
                BinaryFileName = sheetName + ".bin",
                Namespace = _config.Namespace,
                DataNameRow = _config.DataNameRow,
                DataTypeRow = _config.DataTypeRow,
                DataStartRow = _config.DataStartRow,
                Fields = new List<FieldDefinition>(),
            };

            for (int col = 1; col <= reader.GetSheetColumnCount(); col++)
            {
                var nameValue = reader.ReadSheet(_config.DataNameRow, col);
                var typeValue = reader.ReadSheet(_config.DataTypeRow, col);

                if (string.IsNullOrEmpty(nameValue) || string.IsNullOrEmpty(typeValue))
                    continue;

                info.Fields.Add(new FieldDefinition
                {
                    Name = nameValue,
                    TypeName = ConvertExcelTypeToFbsType(typeValue),
                    DataIndex = col,
                });
            }

            return info;
        }

        /// <summary>
        /// snake_case → PascalCase。如 "battle_unit" → "BattleUnit"
        /// </summary>
        public static string ConvertSheetNameToDefineName(string sheetName)
        {
            return Regex.Replace(sheetName, @"(^|_)([a-zA-Z])",
                m => m.Groups[2].Value.ToUpper());
        }

        /// <summary>
        /// Excel 类型名 → flatbuffer 类型名。int[] → [int]
        /// </summary>
        public static string ConvertExcelTypeToFbsType(string excelType)
        {
            var match = Regex.Match(excelType, @"^(.+?)\[.*\]$");
            if (match.Success)
                return $"[{match.Groups[1].Value}]";
            return excelType;
        }
    }
}
