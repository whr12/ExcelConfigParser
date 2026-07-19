using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public interface IExcelReader
    {
        string GetExcelName();

        int GetWorkSheetsCount();

        bool TrySwitchSheet(int sheetIndex);

        bool TrySwitchSheet(string sheetName);

    }
}
