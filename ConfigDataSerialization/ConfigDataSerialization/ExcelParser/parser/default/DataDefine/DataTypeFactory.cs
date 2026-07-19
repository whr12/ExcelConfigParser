using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public static class DataTypeFactory
    {
        static Dictionary<string, ITypeDefine> catchedTypes = new Dictionary<string, ITypeDefine>();

        static readonly string EnumPattern = @"^[a-zA-Z_][a-zA-Z0-9_]*$";
        static readonly string[] CSharpKeywords =
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit",
            "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal",
            "is", "lock", "long", "namespace", "new", "null", "object", "operator",
            "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        };


        public static ITypeDefine ConvertToType(string excelTypeDefine)
        {
            if(!catchedTypes.TryGetValue(excelTypeDefine, out ITypeDefine typeDefine))
            {
                typeDefine = CreateTypeDefine(excelTypeDefine);
                catchedTypes.Add(excelTypeDefine, typeDefine);
            }

            return typeDefine;
        }

        private static ITypeDefine? CreateTypeDefine(string excelTypeDefine)
        {
            #region 处理简单类型
            if ("int".Equals(excelTypeDefine))
            {
                return new IntegerTypeDefine();
            }

            if ("uint".Equals(excelTypeDefine))
            {
                return new UintTypeDefine();
            }

            if ("float".Equals(excelTypeDefine))
            {
                return new FloatTypeDefine();
            }

            if ("bool".Equals(excelTypeDefine))
            {
                return new BooleanTypeDefine();
            }

            if ("string".Equals(excelTypeDefine))
            {
                return new StringTypeDefine();
            }
            #endregion

            // 分析是否为数组
            if(MatchArray(excelTypeDefine, out string genericString, out string separateString))
            {
                ITypeDefine genericType = ConvertToType(genericString);
                if (genericType != null)
                {
                    ArrayTypeDefine arrayType = new ArrayTypeDefine(genericType, separateString);
                    return arrayType;
                }
                else
                {
                    //满足了数组的格式，但类型不匹配，为无效的类型定义方式
                    return null;
                }
            }

            // 其他方式都认为是枚举类型
            if(Regex.IsMatch(excelTypeDefine, EnumPattern))
            {
                if(!Array.Exists(CSharpKeywords, kw => kw.Equals(excelTypeDefine, StringComparison.Ordinal)))
                {
                    return new EnumTypeDefine(excelTypeDefine);
                }
            }

            return null;
        }

        private static bool MatchArray(string inputValue, out string? genericString, out string? seperateString)
        {
            genericString = null;
            seperateString = null;

            Match match = Regex.Match(inputValue, @"^(.*)\[([^\]]*)\](.*)$");
            if (match.Success)
            {
                genericString = match.Groups[1].Value;
                seperateString = match.Groups[2].Value;
                if (string.IsNullOrEmpty(seperateString))
                {
                    seperateString = ",";
                }
                return true;
            }

            return false;
        }
    }
}
