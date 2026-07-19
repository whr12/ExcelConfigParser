using ConfigDataSerialization.ExcelParser;
using System.IO;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("ExcelConfigParser — Excel → FlatBuffer 导出工具");

        // 配置文件路径：命令行参数 > 默认相对路径
        string configPath;
        if (args.Length > 0)
        {
            configPath = args[0];
        }
        else
        {
            configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "../../../../Excel/json/parse_config.json");
        }

        configPath = Path.GetFullPath(configPath);
        Console.WriteLine($"[Config] {configPath}");

        if (!File.Exists(configPath))
        {
            Console.WriteLine($"[Error] 配置文件不存在: {configPath}");
            return;
        }

        string jsonText = File.ReadAllText(configPath);
        ParseConfig config = JsonSerializer.Deserialize<ParseConfig>(jsonText);

        var parser = ExcelParser.Create(config);

        Console.WriteLine("[Step 1/2] 生成代码...");
        parser.ParseCode();

        Console.WriteLine("[Step 2/2] 导出数据...");
        parser.ParseData();

        Console.WriteLine("完成。");
    }
}