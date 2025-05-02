using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    internal struct ParseConfig
    {
        public string NameSpace;
        public string ExcelPath;
        public string OutputPath;

        public string InvalidPrefix;
        public string EnumPrefix;
        public string GlobalPrefix;

        public int DataNameRow;
        public int DataTypeRow;
        public int DataStartRow;
    }
}
