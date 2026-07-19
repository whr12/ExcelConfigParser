using ConfigDataSerialization.ExcelParser;
using ConfigDataSerialization.ExcelParser.parser;
using System;
using System.IO;
using System.Text.Json;

internal class Program
{
    private enum ExportMode { All, CodeOnly, DataOnly }

    private static void Main(string[] args)
    {
        Console.WriteLine("ExcelConfigParser — Excel → FlatBuffer 导出工具");

        // 解析参数
        var mode = ExportMode.All;
        string configPath = null;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "-c":
                case "--code":
                    mode = ExportMode.CodeOnly;
                    break;
                case "-d":
                case "--data":
                    mode = ExportMode.DataOnly;
                    break;
                case "-a":
                case "--all":
                    mode = ExportMode.All;
                    break;
                default:
                    if (!arg.StartsWith("-"))
                        configPath = arg;
                    break;
            }
        }

        if (configPath == null)
        {
            Console.WriteLine("[Error] 必须指定配置文件路径");
            Console.WriteLine("用法: ConfigDataSerialization.exe <config.json> [--code|--data|--all]");
            return;
        }

        configPath = Path.GetFullPath(configPath);
        Console.WriteLine($"[Config] {configPath}");
        Console.WriteLine($"[Mode] {(mode == ExportMode.All ? "全部" : mode == ExportMode.CodeOnly ? "仅代码" : "仅数据")}");

        if (!File.Exists(configPath))
        {
            Console.WriteLine($"[Error] 配置文件不存在: {configPath}");
            return;
        }

        string jsonText = File.ReadAllText(configPath);
        ParseConfig config = JsonSerializer.Deserialize<ParseConfig>(jsonText);

        var parser = ExcelParser.Create(config);

        if (mode != ExportMode.DataOnly)
        {
            Console.WriteLine("[1/2] 生成代码...");
            parser.ParseCode();
        }

        if (mode != ExportMode.CodeOnly)
        {
            Console.WriteLine(mode == ExportMode.All ? "[2/2] 导出数据..." : "[1/1] 导出数据...");
            parser.ParseData();
        }

        Console.WriteLine("完成。");
    }
}
