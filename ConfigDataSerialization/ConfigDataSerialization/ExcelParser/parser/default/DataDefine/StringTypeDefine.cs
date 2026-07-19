using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public class StringTypeDefine : ITypeDefine
    {
        public string Name => "string";

        public int TypeLength => 4;

        public bool TryConvertToJsonString(string rawText, out string result)
        {
            result = $"\"{rawText}\"";
            return true;
        }
    }
}
