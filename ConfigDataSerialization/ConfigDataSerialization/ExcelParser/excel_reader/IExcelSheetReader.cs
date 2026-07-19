using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public interface IExcelSheetReader
    {
        string GetExcelName();

        string GetSheetName();

        int GetSheetIndex();

        string ReadSheet(int row, int column);

        int GetSheetColumnCount();

        int GetSheetRowCount();
    }
}
