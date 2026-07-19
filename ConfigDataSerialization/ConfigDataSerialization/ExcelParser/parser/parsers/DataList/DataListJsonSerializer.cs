using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型1 json 生成：{ "datas": [ { ... }, { ... } ] }
    /// </summary>
    public class DataListJsonSerializer
    {
        public void Serialize(DataListSheetInfo info, IExcelSheetReader reader, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".json");

            using var writer = new StreamWriter(filePath, false, ExcelParserHelper.UTF8);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine("{");
            writer.WriteLine("  \"datas\":");
            writer.WriteLine("  [");

            int rowCount = reader.GetSheetRowCount();
            bool isFirstRecord = true;
            for (int row = info.DataStartRow; row <= rowCount; row++)
            {
                if (!isFirstRecord)
                    writer.WriteLine("    },");
                isFirstRecord = false;

                writer.WriteLine("    {");
                for (int i = 0; i < info.Fields.Count; i++)
                {
                    var f = info.Fields[i];
                    var raw = reader.ReadSheet(row, f.columnIndex);
                    f.dataType.TryConvertToJsonString(raw, out var jsonValue);
                    var comma = i < info.Fields.Count - 1 ? "," : "";
                    writer.WriteLine($"      \"{f.dataName}\": {jsonValue}{comma}");
                }
            }

            if (!isFirstRecord)
                writer.WriteLine("    }");

            writer.WriteLine("  ]");
            writer.WriteLine("}");
            writer.Flush();
        }
    }
}
