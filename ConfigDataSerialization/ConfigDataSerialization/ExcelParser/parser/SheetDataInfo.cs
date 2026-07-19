using System.Collections.Generic;

namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// 字段定义（类型1 + 类型2 使用）。
    /// </summary>
    public class FieldDefinition
    {
        /// <summary>字段名，如 "MoveSpeed"</summary>
        public string Name;

        /// <summary>flatbuffer 类型名，如 "uint", "float", "string", "[int]"</summary>
        public string TypeName;

        /// <summary>数据在 Excel 中的列号（类型1 用）或行号（类型2 用）</summary>
        public int DataIndex;
    }

    /// <summary>
    /// 枚举值定义（仅类型3 使用）。
    /// </summary>
    public class EnumValueDefinition
    {
        public int Value;
        public string Name;
        public string Comment;
    }

    /// <summary>
    /// Sheet 解析结果基类。三种类型共享的公共信息。
    /// </summary>
    public abstract class SheetDataInfo
    {
        /// <summary>所属 Excel 文件名</summary>
        public string ExcelFileName;

        /// <summary>sheet 原始名，如 "battle_unit"</summary>
        public string SheetName;

        /// <summary>PascalCase 类名，如 "BattleUnit"</summary>
        public string DefineName;

        /// <summary>二进制文件名，如 "battle_unit.bin"</summary>
        public string BinaryFileName;

        /// <summary>flatbuffer namespace</summary>
        public string Namespace;
    }

    /// <summary>
    /// 类型1 — 数据列表（默认）。按列读取，数据从 DataStartRow 开始逐行排列。
    /// </summary>
    public class DataListSheetInfo : SheetDataInfo
    {
        public List<FieldDefinition> Fields;

        /// <summary>字段名所在行（配置：DataNameRow）</summary>
        public int DataNameRow;

        /// <summary>字段类型所在行（配置：DataTypeRow）</summary>
        public int DataTypeRow;

        /// <summary>数据起始行（配置：DataStartRow）</summary>
        public int DataStartRow;
    }

    /// <summary>
    /// 类型2 — 全局配置。按行读取，数据在固定列。
    /// </summary>
    public class GlobalConfigSheetInfo : SheetDataInfo
    {
        public List<FieldDefinition> Fields;

        /// <summary>数值所在列（如第5列）</summary>
        public int ValueColumn;
    }

    /// <summary>
    /// 类型3 — 枚举定义。
    /// </summary>
    public class EnumSheetInfo : SheetDataInfo
    {
        public List<EnumValueDefinition> Values;
    }
}
