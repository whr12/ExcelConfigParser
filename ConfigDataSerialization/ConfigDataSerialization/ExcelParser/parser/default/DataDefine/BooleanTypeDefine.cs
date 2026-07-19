using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public class BooleanTypeDefine : ITypeDefine
    {
        private string DefaultValue = "false";

        public string Name => "bool";

        public int TypeLength => 4;

        public bool TryConvertToJsonString(string rawText, out string result)
        {
            result = DefaultValue;
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return true;
            }

            // 数值：0 → false，非0 → true
            if (int.TryParse(rawText, out var intValue))
            {
                result = intValue != 0 ? "true" : "false";
                return true;
            }

            if (bool.TryParse(rawText, out var value))
            {
                result = value.ToString().ToLowerInvariant();
                return true;
            }

            Log.Error($"Input Text({rawText}) can't convert to {Name}, return {result} instead.");
            return false;
        }
    }
}
