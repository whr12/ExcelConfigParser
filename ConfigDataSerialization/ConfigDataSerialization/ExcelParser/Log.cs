using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public static class Log
    {
        public static void Debug(string text)
        {
            Console.WriteLine(text);
        }

        public static void Warn(string text)
        {
            Console.WriteLine($"[Warning]{text}");
        }

        public static void Error(string text)
        {
            Console.WriteLine($"[Error]{text}");
        }
    }
}
