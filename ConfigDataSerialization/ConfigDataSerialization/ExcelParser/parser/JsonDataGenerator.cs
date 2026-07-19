using System.Collections.Generic;
using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// .json 中间数据生成器。通过方法重载实现类型1和类型2的数据序列化。
    /// 类型3（枚举）不需要生成数据文件。
    /// </summary>
    public class JsonDataGenerator :
        IDataSerializer<DataListSheetInfo>,
        IDataSerializer<GlobalConfigSheetInfo>
    {
        // ===== 类型1：按行遍历 =====

        public void Serialize(DataListSheetInfo info, IExcelSheetReader reader, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".json");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine("{");
            writer.WriteLine("  \"datas\":");
            writer.WriteLine("  [");

            // 先收集有效数据行（跳过空行）
            var rows = new List<int>();
            int rowCount = reader.GetSheetRowCount();
            for (int row = info.DataStartRow; row <= rowCount; row++)
                rows.Add(row);

            for (int r = 0; r < rows.Count; r++)
            {
                int row = rows[r];
                bool isLast = (r == rows.Count - 1);

                writer.WriteLine("    {");
                for (int i = 0; i < info.Fields.Count; i++)
                {
                    var f = info.Fields[i];
                    var raw = reader.ReadSheet(row, f.DataIndex);
                    var jsonValue = ConvertToJsonValue(raw, f.TypeName);
                    var comma = i < info.Fields.Count - 1 ? "," : "";
                    writer.WriteLine($"      \"{f.Name}\": {jsonValue}{comma}");
                }
                writer.WriteLine(isLast ? "    }" : "    },");
            }

            writer.WriteLine("  ]");
            writer.WriteLine("}");
            writer.Flush();
        }

        // ===== 类型2：读固定列 =====

        public void Serialize(GlobalConfigSheetInfo info, IExcelSheetReader reader, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".json");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine("{");
            for (int i = 0; i < info.Fields.Count; i++)
            {
                var f = info.Fields[i];
                var raw = reader.ReadSheet(f.DataIndex, info.ValueColumn);
                var jsonValue = ConvertToJsonValue(raw, f.TypeName);
                var comma = i < info.Fields.Count - 1 ? "," : "";
                writer.WriteLine($"  \"{f.Name}\": {jsonValue}{comma}");
            }
            writer.WriteLine("}");
            writer.Flush();
        }

        // ===== 工具 =====

        private static string ConvertToJsonValue(string rawText, string typeName)
        {
            if (typeName == "string")
                return $"\"{rawText}\"";

            if (typeName == "bool")
            {
                var lower = rawText.Trim().ToLowerInvariant();
                return lower == "true" || lower == "1" ? "true" : "false";
            }

            if (string.IsNullOrWhiteSpace(rawText))
                return typeName.StartsWith("[") ? "[]" : "0";

            if (typeName.StartsWith("["))
            {
                if (rawText.TrimStart().StartsWith("["))
                    return rawText.Trim();
                return $"[{rawText}]";
            }

            if (int.TryParse(rawText, out _) || uint.TryParse(rawText, out _) ||
                float.TryParse(rawText, out _) || double.TryParse(rawText, out _))
                return rawText.Trim();

            return rawText;
        }
    }
}
