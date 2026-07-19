using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// 类型1 — 数据列表。按列读取，数据从 DataStartRow 开始逐行排列。
    /// </summary>
    public class DataListSheetInfo : SheetDataInfo
    {
        public List<SingleDataDefine> Fields;

        /// <summary>字段名所在行（配置：DataNameRow）</summary>
        public int DataNameRow;

        /// <summary>字段类型所在行（配置：DataTypeRow）</summary>
        public int DataTypeRow;

        /// <summary>数据起始行（配置：DataStartRow）</summary>
        public int DataStartRow;
    }
}
