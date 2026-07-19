using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型1 fbs 生成：table + tableList + root_type。
    /// </summary>
    public class DataListFbsGenerator
    {
        public void Generate(DataListSheetInfo info, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".fbs");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");

            var includes = ExcelParserHelper.GetRequiredIncludes(info.Fields);
            foreach (var inc in includes)
                writer.WriteLine($"include \"{inc}\";");

            writer.WriteLine($"namespace {info.Namespace};");
            writer.WriteLine();

            writer.WriteLine($"table {info.DefineName}");
            writer.WriteLine("{");
            foreach (var f in info.Fields)
            {
                if (!string.IsNullOrEmpty(f.Comment))
                    writer.WriteLine($"  /// {f.Comment}");
                writer.WriteLine($"  {f.dataName} : {f.typeName};");
            }
            writer.WriteLine("}");

            writer.WriteLine();
            writer.WriteLine($"table {info.DefineName}List");
            writer.WriteLine("{");
            writer.WriteLine($"  datas : [{info.DefineName}];");
            writer.WriteLine("}");

            writer.WriteLine();
            writer.WriteLine($"root_type {info.DefineName}List;");
            writer.Flush();
        }
    }
}
