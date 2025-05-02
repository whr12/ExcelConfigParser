using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    internal interface IExcelDataParser
    {
        public void ParseToJsonData(ExcelReader excelReader, string outputFile);
    }

    internal interface IExcelTypeParser
    {
        public void ParseToCode(ExcelReader excelReader, string outputFile);
    }
}
