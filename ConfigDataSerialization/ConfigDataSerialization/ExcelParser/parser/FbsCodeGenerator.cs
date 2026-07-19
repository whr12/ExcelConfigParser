using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// .fbs schema 生成器。通过方法重载实现三类数据的代码生成。
    /// </summary>
    public class FbsCodeGenerator :
        ICodeGenerator<DataListSheetInfo>,
        ICodeGenerator<GlobalConfigSheetInfo>,
        ICodeGenerator<EnumSheetInfo>
    {
        // ===== 类型1：table + tableList =====

        public void Generate(DataListSheetInfo info, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".fbs");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine($"namespace {info.Namespace};");
            writer.WriteLine();

            writer.WriteLine($"table {info.DefineName}");
            writer.WriteLine("{");
            foreach (var f in info.Fields)
                writer.WriteLine($"  {f.Name} : {f.TypeName};");
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

        // ===== 类型2：单 struct =====

        public void Generate(GlobalConfigSheetInfo info, string outputPath)
        {
            var filePath = Path.Combine(outputPath, info.SheetName + ".fbs");

            using var writer = new StreamWriter(filePath);

            writer.WriteLine($"// {info.ExcelFileName}  →  {info.SheetName}");
            writer.WriteLine($"namespace {info.Namespace};");
            writer.WriteLine();

            writer.WriteLine($"struct {info.DefineName}");
            writer.WriteLine("{");
            foreach (var f in info.Fields)
                writer.WriteLine($"  {f.Name} : {f.TypeName};");
            writer.WriteLine("}");

            writer.WriteLine();
            writer.WriteLine($"root_type {info.DefineName};");
            writer.Flush();
        }

        // ===== 类型3：enum =====

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
                    writer.WriteLine($"  {v.Name} = {v.Value},  // {v.Comment}");
                else
                    writer.WriteLine($"  {v.Name} = {v.Value},");
            }
            writer.WriteLine("}");

            writer.Flush();
        }
    }
}
