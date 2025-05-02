using ConfigDataSerialization.ExcelParser;
using OfficeOpenXml;
using System.IO;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Start Program");

        //string license = System.IO.File.ReadAllText("F:\\Develop\\ThirdCode\\ConfigDataSerialization\\Excel\\PolyForm-Noncommercial-1.0.0.txt");
        //ExcelPackage.License.SetNonCommercialPersonal("Personal");

        //ExcelReader reader = ExcelReader.CreateExcelReader("F:\\Develop\\ThirdCode\\ConfigDataSerialization\\Excel\\BattleUnit.xlsx");
        //IExcelTypeParser typeParser = new DefaultExcelTypeParser();
        //typeParser.ParseToCode(reader, "F:\\Develop\\ThirdCode\\ConfigDataSerialization\\Excel\\parse\\BattleUnit.fbs");

        //reader.Dispose();

        string jsonText = File.ReadAllText("F:\\Develop\\ThirdCode\\ConfigDataSerialization\\Excel\\json\\parse_config.json");
        using System.Text.Json.JsonDocument json = System.Text.Json.JsonDocument.Parse(jsonText);

        JsonElement jsonElement = json.RootElement;

        foreach(var i in jsonElement.EnumerateObject())
        {
            Log.Debug($"{i.Name} : {i.Value} ----> {i.Value.ValueKind}");
        }

        Console.WriteLine("EndProgram");
    }
}