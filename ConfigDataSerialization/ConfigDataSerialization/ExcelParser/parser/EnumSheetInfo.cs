using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// 枚举值定义。仅服务于 EnumSheetInfo。
    /// </summary>
    public class EnumValueDefinition
    {
        public int Value;
        public string Name;
        public string Comment;
    }

    /// <summary>
    /// 类型3 — 枚举定义。
    /// </summary>
    public class EnumSheetInfo : SheetDataInfo
    {
        public List<EnumValueDefinition> Values;
    }
}
