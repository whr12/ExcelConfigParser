using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// 类型2 — 全局配置。按行读取，数据在固定列。
    /// </summary>
    public class GlobalConfigSheetInfo : SheetDataInfo
    {
        public List<SingleDataDefine> Fields;

        /// <summary>数值所在列（如第5列）</summary>
        public int ValueColumn;
    }
}
