

namespace ConfigDataSerialization.ExcelParser
{
    internal class DefaultExcelDataParser : IExcelDataParser
    {
        private int dataNameRow;
        private int dataTypeRow;
        private int dataStartRow;

        public void ParseToJsonData(ExcelReader excelReader, string outputFile)
        {
            if (excelReader == null)
            {
                Console.WriteLine("[Error]input reader shouldn't be null.");
                return;
            }

            if (string.IsNullOrEmpty(outputFile))
            {
                Console.Write("[Error]output file path should't be null or empty.");
                return;
            }

            using (FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                
            }
        }
    }

}
