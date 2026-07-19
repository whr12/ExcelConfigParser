using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ConfigDataSerialization.ExcelParser.parser
{
    /// <summary>
    /// flatc.exe 编译器封装。将 .fbs 编译为 C# 代码，或将 .json 编译为 .bin。
    /// </summary>
    public class FlatBufferCompiler
    {
        private readonly string _flatcPath;

        public FlatBufferCompiler(string flatcPath)
        {
            _flatcPath = flatcPath;
        }

        /// <summary>
        /// 将 .fbs schema 编译为 C# 代码。
        /// </summary>
        public void CompileFbsToCSharp(List<string> fbsFiles, string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            foreach (var fbs in fbsFiles)
            {
                RunFlatc($"-o \"{outputDir}\" --csharp \"{fbs}\"");
            }
        }

        /// <summary>
        /// 将 .fbs + .json 编译为 .bin 二进制数据。
        /// </summary>
        public void CompileJsonToBinary(List<string> jsonFiles, List<string> fbsFiles, string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            for (int i = 0; i < jsonFiles.Count; i++)
            {
                var jsonFile = jsonFiles[i];
                var fbsFile = fbsFiles[i];

                RunFlatc($"-o \"{outputDir}\" -b \"{fbsFile}\" \"{jsonFile}\"");
            }
        }

        private void RunFlatc(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _flatcPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Console.WriteLine($"[flatc] {Path.GetFileName(_flatcPath)} {arguments}");

            var process = Process.Start(psi);
            if (process == null)
            {
                Console.Error.WriteLine($"[flatc] 启动失败: {_flatcPath}");
                return;
            }
            process.WaitForExit();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(stdout))
                Console.WriteLine(stdout);
            if (!string.IsNullOrWhiteSpace(stderr))
                Console.Error.WriteLine($"[flatc Error] {stderr}");
            if (process.ExitCode != 0)
                Console.Error.WriteLine($"[flatc] 退出码: {process.ExitCode}");
        }
    }
}
