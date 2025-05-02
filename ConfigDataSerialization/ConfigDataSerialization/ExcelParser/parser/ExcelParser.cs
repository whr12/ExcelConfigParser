using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    internal class ExcelParser
    {
        private ParseConfig config;

        public static ExcelParser Create(ParseConfig config)
        {
            if (string.IsNullOrEmpty(config.ExcelPath))
            {
                throw new NullReferenceException("Input excel path is null or empty!!!");
            }

            if(!Directory.Exists(config.ExcelPath))
            {
                throw new IOException($"Input excel path({config.ExcelPath}) doesn't exist!!!");
            }            


            ExcelParser parser = new ExcelParser();
            parser.config = config;

            return parser;
        }
        private ExcelParser() { }


    }
}
