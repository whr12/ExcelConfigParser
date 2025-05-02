using OfficeOpenXml;

namespace ConfigDataSerialization.ExcelParser
{
    internal class ExcelReader : IDisposable
    {
        private ExcelPackage package;

        private ExcelWorksheet workSheet;

        public static ExcelReader CreateExcelReader(string filePath)
        {
            ExcelReader reader = new ExcelReader(filePath);            

            return reader;
        }

        private ExcelReader(string filePath)
        {
            package = new ExcelPackage(filePath);

            workSheet = package.Workbook.Worksheets[0];
        }

        public string GetExcelName()
        {
            return package.File.Name;
        }

        public int GetWorksheetsCount()
        {
            return workSheet.Workbook.Worksheets.Count;
        }

        public bool TrySwitchSheet(int sheetIndex)
        {
            if (package.Workbook.Worksheets.Count > sheetIndex)
            {
                workSheet = package.Workbook.Worksheets[sheetIndex];
                return true;
            }
            return false;
        }

        public string GetSheetName()
        {
            return workSheet.Name;
        }

        public string ReadExcel(int row, int column)
        {
            return workSheet.Cells[row, column].Text;
        }

        public int GetSheetColumnCount()
        {
             return workSheet.Dimension.Columns;
        }

        public int GetSheetRowCount()
        {
            return workSheet.Dimension.Rows;
        }

        public void Dispose()
        {
            package.Dispose();
        }
    }
}
