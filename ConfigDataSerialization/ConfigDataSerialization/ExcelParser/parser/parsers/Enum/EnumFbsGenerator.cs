using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型3 fbs 生成：enum {DefineName} : int { ... }
    /// </summary>
    public class EnumFbsGenerator
    {
        public void Generate(EnumSheetInfo info, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".fbs");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine($"namespace {info.Namespace};");
            writer.WriteLine();

            writer.WriteLine($"enum {info.DefineName} : int");
            writer.WriteLine("{");
            foreach (var v in info.Values)
            {
                if (!string.IsNullOrEmpty(v.Comment))
                    writer.WriteLine($"  /// {v.Comment}");
                writer.WriteLine($"  {v.Name} = {v.Value},");
            }
            writer.WriteLine("}");
            writer.Flush();
        }
    }
}
