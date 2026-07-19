using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    public class DataListWrapperCreator
    {
        private readonly string _templatePath;

        public DataListWrapperCreator(string templatePath)
        {
            _templatePath = templatePath;
        }

        public void Create(DataListSheetInfo info, string outputPath)
        {
            if (string.IsNullOrEmpty(_templatePath) || !File.Exists(_templatePath))
            {
                Log.Warn($"Template missing: {_templatePath}");
                return;
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var genName = info.DefineName + "_gen";

            // Data 类（模板）
            var template = File.ReadAllText(_templatePath);
            var dataContent = template
                .Replace("#NAMESPACE#", info.Namespace)
                .Replace("#DEFINE_NAME#", info.DefineName)
                .Replace("#GEN_NAME#", genName)
                .Replace("#BINARY_FILE#", info.BinaryFileName)
                .Replace("#FIELD_COMMENTS#", GenerateFieldComments(info.Fields));

            File.WriteAllText(
                Path.Combine(outputPath, $"{info.DefineName}Data.cs"),
                dataContent, ExcelParserHelper.UTF8);

            // Wrapper 类（纯代码生成）
            var wrapperContent = GenerateWrapperClass(info.DefineName, genName, info.Fields, info.Namespace);
            File.WriteAllText(
                Path.Combine(outputPath, $"{info.DefineName}.cs"),
                wrapperContent, ExcelParserHelper.UTF8);
        }

        private static string GenerateFieldComments(List<SingleDataDefine> fields)
        {
            var sb = new StringBuilder();
            foreach (var f in fields)
            {
                if (!string.IsNullOrEmpty(f.Comment))
                    sb.AppendLine($"    /// {f.dataName} — {f.Comment}");
            }
            return sb.Length == 0 ? string.Empty : sb.ToString().TrimEnd();
        }

        private static string GenerateWrapperClass(string defineName, string genName,
            List<SingleDataDefine> fields, string ns)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {defineName}");
            sb.AppendLine("    {");
            sb.AppendLine($"        private {genName} _raw;");
            sb.AppendLine();
            sb.AppendLine($"        public {defineName}({genName} raw) {{ _raw = raw; }}");
            sb.AppendLine();

            foreach (var f in fields)
            {
                var pascalName = ExcelParserHelper.ToPascalCase(f.dataName);
                var csType = MapToCSharpType(f.typeName);
                var accessor = f.typeName.StartsWith("[") ? $"Get{pascalName}Array()" : pascalName;
                if (!string.IsNullOrEmpty(f.Comment))
                    sb.AppendLine($"        /// <summary> {f.Comment} </summary>");
                sb.AppendLine($"        public {csType} {pascalName} => _raw.{accessor};");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string MapToCSharpType(string fbType)
        {
            if (ExcelParserHelper.IsCustomType(fbType))
                return ExcelParserHelper.ToPascalCase(fbType);

            return fbType switch
            {
                "int" => "int",
                "uint" => "uint",
                "float" => "float",
                "bool" => "bool",
                "string" => "string",
                _ when fbType.StartsWith("[") => fbType[1..^1] + "[]",
                _ => fbType,
            };
        }
    }
}
