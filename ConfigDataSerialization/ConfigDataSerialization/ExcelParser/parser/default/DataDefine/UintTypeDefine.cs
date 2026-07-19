using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public class UintTypeDefine : ITypeDefine
    {
        private string DefaultValue = "0";

        public string Name => "uint";

        public int TypeLength => 4;

        public bool TryConvertToJsonString(string rawText, out string result)
        {
            result = DefaultValue;
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return true;
            }

            if (uint.TryParse(rawText, out var value))
            {
                result = value.ToString();
                return true;
            }
            else
            {
                Log.Error($"Input Text({rawText}) can't convert to {Name}, return {result} instead.");
                return false;
            }
        }
    }
}
