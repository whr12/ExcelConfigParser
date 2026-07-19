using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    public class GlobalConfigWrapperCreator
    {
        private readonly string _templatePath;

        public GlobalConfigWrapperCreator(string templatePath)
        {
            _templatePath = templatePath;
        }

        public void Create(GlobalConfigSheetInfo info, string outputPath)
        {
            if (string.IsNullOrEmpty(_templatePath) || !File.Exists(_templatePath))
            {
                Log.Warn($"Template missing: {_templatePath}");
                return;
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var genName = info.DefineName + "_gen";

            var template = File.ReadAllText(_templatePath);
            var content = template
                .Replace("#NAMESPACE#", info.Namespace)
                .Replace("#DEFINE_NAME#", info.DefineName)
                .Replace("#GEN_NAME#", genName)
                .Replace("#BINARY_FILE#", info.BinaryFileName)
                .Replace("#PROPERTIES#", GenerateProperties(info.Fields));

            var filePath = Path.Combine(outputPath, $"{info.DefineName}Data.cs");
            File.WriteAllText(filePath, content, ExcelParserHelper.UTF8);
        }

        private static string GenerateProperties(List<SingleDataDefine> fields)
        {
            var sb = new StringBuilder();
            foreach (var f in fields)
            {
                var csType = MapToCSharpType(f.typeName);
                var pascalName = ExcelParserHelper.ToPascalCase(f.dataName);
                var accessor = f.typeName.StartsWith("[") ? $"Get{pascalName}Array()" : pascalName;
                if (!string.IsNullOrEmpty(f.Comment))
                    sb.AppendLine($"        /// <summary> {f.Comment} </summary>");
                sb.AppendLine($"        public {csType} {pascalName} => _data.{accessor};");
            }
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
