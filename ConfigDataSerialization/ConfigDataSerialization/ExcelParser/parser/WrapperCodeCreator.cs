using System.IO;
using System.Text;

namespace ConfigDataSerialization.ExcelParser.parser
{
    public class WrapperCodeCreator
    {
        private readonly string _dataListTemplate;
        private readonly string _globalConfigTemplate;

        public WrapperCodeCreator(string dataListTemplate, string globalConfigTemplate)
        {
            _dataListTemplate = dataListTemplate;
            _globalConfigTemplate = globalConfigTemplate;
        }

        /// <summary>
        /// 类型1：使用 DataListTemplate，生成 {DefineName}Data.cs
        /// </summary>
        public void CreateWrapper(DataListSheetInfo info, string outputPath)
        {
            if (!TryReadTemplate(_dataListTemplate, out var template))
                return;

            var content = ApplyBaseReplacements(template, info);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var filePath = Path.Combine(outputPath, $"{info.DefineName}Data.cs");
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// 类型2：使用 GlobalConfigTemplate，动态生成属性，输出 {DefineName}Data.cs
        /// </summary>
        public void CreateWrapper(GlobalConfigSheetInfo info, string outputPath)
        {
            if (!TryReadTemplate(_globalConfigTemplate, out var template))
                return;

            var content = ApplyBaseReplacements(template, info);
            content = content.Replace("#PROPERTIES#", GenerateProperties(info.Fields));

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var filePath = Path.Combine(outputPath, $"{info.DefineName}Data.cs");
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        // ===== 内部 =====

        private static string ApplyBaseReplacements(string template, SheetDataInfo info)
        {
            return template
                .Replace("#NAMESPACE#", info.Namespace)
                .Replace("#DEFINE_NAME#", info.DefineName)
                .Replace("#BINARY_FILE#", info.BinaryFileName);
        }

        private static string GenerateProperties(System.Collections.Generic.List<FieldDefinition> fields)
        {
            var sb = new StringBuilder();
            foreach (var f in fields)
            {
                var csType = MapToCSharpType(f.TypeName);
                sb.AppendLine($"        public {csType} {f.Name} => _data.{f.Name};");
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
                _ => fbType, // 枚举类型直接用枚举名
            };
        }

        private static bool TryReadTemplate(string path, out string content)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Log.Warn($"模板文件不存在，跳过 wrapper 生成: {path}");
                content = null;
                return false;
            }

            content = File.ReadAllText(path);
            return true;
        }
    }
}
