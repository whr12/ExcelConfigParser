using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型2 fbs 生成：单 table + root_type。
    /// flatbuffer 内部类型加 _gen 后缀，业务层不可见。
    /// </summary>
    public class GlobalConfigFbsGenerator
    {
        public void Generate(GlobalConfigSheetInfo info, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".fbs");
            var genName = info.DefineName + "_gen";

            using var writer = new StreamWriter(filePath, false, ExcelParserHelper.UTF8);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");

            var includes = ExcelParserHelper.GetRequiredIncludes(info.Fields);
            foreach (var inc in includes)
                writer.WriteLine($"include \"{inc}\";");

            writer.WriteLine($"namespace {info.Namespace};");
            writer.WriteLine();

            writer.WriteLine($"table {genName}");
            writer.WriteLine("{");
            foreach (var f in info.Fields)
            {
                if (!string.IsNullOrEmpty(f.Comment))
                    writer.WriteLine($"  /// {f.Comment}");
                var typeName = ExcelParserHelper.IsCustomType(f.typeName)
                    ? ExcelParserHelper.ToPascalCase(f.typeName) : f.typeName;
                writer.WriteLine($"  {f.dataName} : {typeName};");
            }
            writer.WriteLine("}");

            writer.WriteLine();
            writer.WriteLine($"root_type {genName};");
            writer.Flush();
        }
    }
}
