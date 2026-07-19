using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public class ArrayTypeDefine : ITypeDefine
    {
        public ArrayTypeDefine(ITypeDefine genericsType, string separateChar)
        {
            this.genericsTypeDefine = genericsType;
            this.separateChar = separateChar;
        }

        private ITypeDefine genericsTypeDefine;

        private string separateChar;

        public string Name => $"[{genericsTypeDefine.Name}]";

        public int TypeLength => 4;

        public bool TryConvertToJsonString(string rawText, out string result)
        {
            if (string.IsNullOrEmpty(rawText))
            {
                result = "[]";
                return true;
            }

            bool noError = true;
            var splitStr = rawText.Split(separateChar);
            if (splitStr.Length == 1)
            {
                noError = genericsTypeDefine.TryConvertToJsonString(splitStr[0], out result);
                if (noError)
                {
                    result = $"[{result}]";
                }
                else
                {
                    result = "[]";
                }
                return noError;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            for (int i = 0; i < splitStr.Length; ++i)
            {
                string s = splitStr[i];
                bool r = genericsTypeDefine.TryConvertToJsonString(splitStr[i], out string v);
                noError = noError && r;
                if (i > 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(v);
            }
            stringBuilder.Append(']');
            result = stringBuilder.ToString();
            return noError;
        }
    }
}
