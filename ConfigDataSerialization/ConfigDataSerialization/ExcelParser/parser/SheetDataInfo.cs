namespace ConfigDataSerialization.ExcelParser
{
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

        /// <summary>二进制文件名，如 "battle_unit.bin"（枚举类型为 null）</summary>
        public string BinaryFileName;

        /// <summary>flatbuffer namespace</summary>
        public string Namespace;
    }
}
