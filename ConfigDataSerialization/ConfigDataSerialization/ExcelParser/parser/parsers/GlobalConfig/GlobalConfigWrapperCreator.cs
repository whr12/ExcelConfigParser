using System.IO;
using System.Text;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型2 wrapper 生成。使用 GlobalConfigTemplate，动态生成 #PROPERTIES#。
    /// </summary>
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
                Log.Warn($"模板文件不存在: {_templatePath}");
                return;
            }

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var template = File.ReadAllText(_templatePath);
            var content = template
                .Replace("#NAMESPACE#", info.Namespace)
                .Replace("#DEFINE_NAME#", info.DefineName)
                .Replace("#BINARY_FILE#", info.BinaryFileName)
                .Replace("#PROPERTIES#", GenerateProperties(info.Fields));

            var filePath = Path.Combine(outputPath, $"{info.DefineName}Data.cs");
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        private static string GenerateProperties(System.Collections.Generic.List<SingleDataDefine> fields)
        {
            var sb = new StringBuilder();
            foreach (var f in fields)
            {
                var csType = MapToCSharpType(f.typeName);
                if (!string.IsNullOrEmpty(f.Comment))
                    sb.AppendLine($"        /// <summary> {f.Comment} </summary>");
                sb.AppendLine($"        public {csType} {f.dataName} => _data.{f.dataName};");
            }
            return sb.ToString();
        }

        private static string MapToCSharpType(string fbType)
        {
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
