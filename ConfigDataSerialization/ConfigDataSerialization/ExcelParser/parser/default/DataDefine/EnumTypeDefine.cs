using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public class EnumTypeDefine : ITypeDefine
    {
        private string enumName;
        public string Name => enumName;

        public int TypeLength => 4;

        public EnumTypeDefine(string enumName)
        {
            this.enumName = enumName;
        }

        public bool TryConvertToJsonString(string rawText, out string result)
        {
            result = "0";
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return true;
            }

            if (int.TryParse(rawText, out var value))
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
