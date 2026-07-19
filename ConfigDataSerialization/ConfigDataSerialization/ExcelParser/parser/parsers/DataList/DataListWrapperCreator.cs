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

            var template = File.ReadAllText(_templatePath);
            var content = template
                .Replace("#NAMESPACE#", info.Namespace)
                .Replace("#DEFINE_NAME#", info.DefineName)
                .Replace("#BINARY_FILE#", info.BinaryFileName)
                .Replace("#FIELD_COMMENTS#", GenerateFieldComments(info.Fields))
                .Replace("#ROW_CLASS#", GenerateRowClass(info.DefineName, info.Fields));

            var filePath = Path.Combine(outputPath, $"{info.DefineName}Data.cs");
            File.WriteAllText(filePath, content, ExcelParserHelper.UTF8);
        }

        private static string GenerateFieldComments(List<SingleDataDefine> fields)
        {
            var sb = new StringBuilder();
            foreach (var f in fields)
            {
                if (!string.IsNullOrEmpty(f.Comment))
                    sb.AppendLine($"    /// {f.dataName} — {f.Comment}");
            }
            var result = sb.ToString();
            return string.IsNullOrEmpty(result) ? string.Empty : result.TrimEnd();
        }

        private static string GenerateRowClass(string defineName, List<SingleDataDefine> fields)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"    public class {defineName}Row");
            sb.AppendLine("    {");

            // Properties
            foreach (var f in fields)
            {
                var csType = MapToCSharpType(f.typeName);
                var pascalName = ExcelParserHelper.ToPascalCase(f.dataName);
                if (!string.IsNullOrEmpty(f.Comment))
                    sb.AppendLine($"        /// <summary> {f.Comment} </summary>");
                sb.AppendLine($"        public {csType} {pascalName} {{ get; private set; }}");
            }

            sb.AppendLine();
            sb.AppendLine($"        internal {defineName}Row({defineName} src)");
            sb.AppendLine("        {");
            foreach (var f in fields)
            {
                var pascalName = ExcelParserHelper.ToPascalCase(f.dataName);
                if (f.typeName.StartsWith("["))
                    sb.AppendLine($"            {pascalName} = src.Get{pascalName}Array();");
                else
                    sb.AppendLine($"            {pascalName} = src.{pascalName};");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");

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
