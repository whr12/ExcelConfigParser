using System.IO;
using OfficeOpenXml;

namespace ConfigDataSerialization.ExcelParser
{
    public class ExcelReader : IExcelReader, IExcelSheetReader, IDisposable
    {
        private readonly string _filePath;
        private FileStream _fileStream;
        private ExcelPackage _package;
        private ExcelWorksheet _workSheet;

        public static ExcelReader CreateExcelReader(string filePath)
        {
            return new ExcelReader(filePath);
        }

        private ExcelReader(string filePath)
        {
            _filePath = filePath;
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _package = new ExcelPackage(_fileStream);
            _workSheet = _package.Workbook.Worksheets[0];
        }

        public string GetExcelName()
        {
            return Path.GetFileName(_filePath);
        }

        public int GetWorkSheetsCount()
        {
            return _workSheet.Workbook.Worksheets.Count;
        }

        public bool TrySwitchSheet(int sheetIndex)
        {
            if (sheetIndex >= 0 && _package.Workbook.Worksheets.Count > sheetIndex)
            {
                _workSheet = _package.Workbook.Worksheets[sheetIndex];
                return true;
            }
            return false;
        }

        public bool TrySwitchSheet(string sheetName)
        {
            if (_package.Workbook.Worksheets[sheetName] == null)
                return false;

            _workSheet = _package.Workbook.Worksheets[sheetName];
            return true;
        }

        public string GetSheetName()
        {
            return _workSheet.Name;
        }

        public int GetSheetIndex()
        {
            return _workSheet.Index;
        }

        public string ReadSheet(int row, int column)
        {
            return _workSheet.Cells[row, column].Text;
        }

        public int GetSheetColumnCount()
        {
            return _workSheet.Dimension.Columns;
        }

        public int GetSheetRowCount()
        {
            return _workSheet.Dimension.Rows;
        }

        public void Dispose()
        {
            _package.Dispose();
            _fileStream.Dispose();
        }
    }
}
