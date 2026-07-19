using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// 工具方法。三个 Parser 共用的无状态函数。
    /// </summary>
    public static class ExcelParserHelper
    {
        /// <summary>UTF-8 without BOM，全局统一编码</summary>
        public static readonly System.Text.UTF8Encoding UTF8 = new(false);
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
        /// 判断字段类型是否为自定义类型（非标量、非数组）。
        /// </summary>
        public static bool IsCustomType(string typeName)
        {
            if (typeName.StartsWith("[")) return false;
            return !BuiltInTypes.Contains(typeName);
        }

        /// <summary>
        /// 自定义类型名转 PascalCase。枚举引用和 .fbs 定义保持一致。
        /// </summary>
        public static string ToPascalCase(string name)
        {
            return Regex.Replace(name, @"(^|_)([a-zA-Z])",
                m => m.Groups[2].Value.ToUpper());
        }

        /// <summary>
        /// 从字段列表中提取需要 include 的枚举类型文件。
        /// </summary>
        public static List<string> GetRequiredIncludes(List<SingleDataDefine> fields)
        {
            var includes = new List<string>();
            foreach (var f in fields)
            {
                if (!IsCustomType(f.typeName))
                    continue;

                var includeFile = $"E_{f.typeName}.fbs";
                if (!includes.Contains(includeFile))
                    includes.Add(includeFile);
            }
            return includes;
        }
    }
}
