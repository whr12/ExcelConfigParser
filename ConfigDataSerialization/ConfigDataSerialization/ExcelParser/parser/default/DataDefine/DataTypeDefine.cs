using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public class SingleDataDefine
    {
        public ITypeDefine dataType;
        public string dataName;
        public int columnIndex;
        public string typeName => dataType.Name;
        public int typeLength => dataType.TypeLength;
        public string Comment;
    }

    public interface ITypeDefine
    {
        string Name { get; }
        int TypeLength { get; }
        /// <summary>
        /// 将原始数据转化为json格式描述的字符串，若原始数据无法与类型匹配，则转化为默认值，且返回false
        /// </summary>
        /// <param name="rawText"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryConvertToJsonString(string rawText, out string result);
    }
}
