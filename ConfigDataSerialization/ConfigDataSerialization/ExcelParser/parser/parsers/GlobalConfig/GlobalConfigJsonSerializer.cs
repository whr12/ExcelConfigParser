using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型2 json 生成：{ "Field1": val1, "Field2": val2, ... }
    /// </summary>
    public class GlobalConfigJsonSerializer
    {
        public void Serialize(GlobalConfigSheetInfo info, IExcelSheetReader reader, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".json");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine("{");
            for (int i = 0; i < info.Fields.Count; i++)
            {
                var f = info.Fields[i];
                var raw = reader.ReadSheet(f.columnIndex, info.ValueColumn);
                f.dataType.TryConvertToJsonString(raw, out var jsonValue);
                var comma = i < info.Fields.Count - 1 ? "," : "";
                writer.WriteLine($"  \"{f.dataName}\": {jsonValue}{comma}");
            }
            writer.WriteLine("}");
            writer.Flush();
        }
    }
}
