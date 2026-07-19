namespace ConfigDataSerialization.ExcelParser
{
    public struct ParseConfig
    {
        public string Namespace { get; set; }
        public string ExcelPath { get; set; }
        public string OutputPath { get; set; }

        /// <summary>flatc.exe 的文件路径（相对或绝对）</summary>
        public string FlatcPath { get; set; }

        /// <summary>类型1 数据列表 wrapper 模板路径</summary>
        public string DataListTemplate { get; set; }

        /// <summary>类型2 全局配置 wrapper 模板路径</summary>
        public string GlobalConfigTemplate { get; set; }

        public string InvalidPrefix { get; set; }
        public string EnumPrefix { get; set; }
        public string GlobalPrefix { get; set; }

        public int DataNameRow { get; set; }
        public int DataTypeRow { get; set; }
        public int DataStartRow { get; set; }
    }
}
