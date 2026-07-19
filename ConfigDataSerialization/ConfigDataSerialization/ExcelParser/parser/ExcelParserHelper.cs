using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// 工具方法。三个 Parser 共用的无状态函数。
    /// </summary>
    public static class ExcelParserHelper
    {
        /// <summary>
        /// snake_case → PascalCase。如 "battle_unit" → "BattleUnit"
        /// </summary>
        public static string ConvertSheetNameToDefineName(string sheetName)
        {
            return Regex.Replace(sheetName, @"(^|_)([a-zA-Z])",
                m => m.Groups[2].Value.ToUpper());
        }

        /// <summary>
        /// 校验字段名是否为 snake_case。不是则抛异常，导表终止。
        /// </summary>
        public static void ValidateSnakeCase(string fieldName, string sheetName, string excelFile)
        {
            if (Regex.IsMatch(fieldName, @"^[a-z][a-z0-9]*(_[a-z0-9]+)*$"))
                return;

            var suggestion = Regex.Replace(fieldName, @"([A-Z])", "_$1").ToLowerInvariant().TrimStart('_');
            var msg = $"[{excelFile}] [{sheetName}] 字段名 \"{fieldName}\" 不符合 snake_case，请改为 \"{suggestion}\"";
            Log.Error(msg);
            throw new System.Exception(msg);
        }

        private static readonly HashSet<string> BuiltInTypes = new()
            { "int", "uint", "float", "bool", "string", "byte", "short", "ushort", "long", "ulong", "double" };

        /// <summary>
        /// 从字段列表中提取需要 include 的枚举类型文件。
        /// </summary>
        public static List<string> GetRequiredIncludes(List<SingleDataDefine> fields)
        {
            var includes = new List<string>();
            foreach (var f in fields)
            {
                if (f.typeName.StartsWith("["))
                    continue;
                if (BuiltInTypes.Contains(f.typeName))
                    continue;

                var includeFile = $"E_{f.typeName}.fbs";
                if (!includes.Contains(includeFile))
                    includes.Add(includeFile);
            }
            return includes;
        }
    }
}
