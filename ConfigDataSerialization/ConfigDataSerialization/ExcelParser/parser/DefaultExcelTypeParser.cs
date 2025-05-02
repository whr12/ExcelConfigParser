using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    internal class DefaultExcelTypeParser : IExcelTypeParser
    {
        private int dataNameRow = 2;
        private int dataTypeRow = 3;

        private Dictionary<string, string> typeNameMap = new Dictionary<string, string>();

        public void ParseToCode(ExcelReader excelReader, string outputFile)
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

            string fileName = Path.GetFileNameWithoutExtension(outputFile);

            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine($"// Parse from {excelReader.GetExcelName()}, {excelReader.GetSheetName()}");
                writer.WriteLine();
                writer.WriteLine("include \"enum/enum.fbs\"");
                writer.WriteLine("namespace GameConfig;");
                writer.WriteLine();

                writer.WriteLine($"table {fileName}");
                writer.WriteLine("{");

                for (int i = 1; i <= excelReader.GetSheetColumnCount(); i++)
                {
                    string nameValue = excelReader.ReadExcel(dataNameRow, i);
                    string typeValue = excelReader.ReadExcel(dataTypeRow, i);
                    if (string.IsNullOrEmpty(nameValue) || string.IsNullOrEmpty(typeValue))
                    {
                        continue;
                    }

                    writer.WriteLine($"{nameValue}:{typeValue};");
                }
                writer.WriteLine("}");

                writer.Flush();
            }
        }
    }
}
