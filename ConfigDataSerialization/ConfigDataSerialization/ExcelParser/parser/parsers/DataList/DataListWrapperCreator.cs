using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigDataSerialization.ExcelParser.parser.parsers
{
    /// <summary>
    /// 类型1 wrapper 生成。使用 DataListTemplate 模板替换占位符。
    /// </summary>
    public class DataListWrapperCreator
    {
        private readonly string _templatePath;

        public DataListWrapperCreator(string templatePath)
        {
            _templatePath = templatePath;
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

        public void Create(DataListSheetInfo info, string outputPath)
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
                .Replace("#FIELD_COMMENTS#", GenerateFieldComments(info.Fields));

            var filePath = Path.Combine(outputPath, $"{info.DefineName}Data.cs");
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}
