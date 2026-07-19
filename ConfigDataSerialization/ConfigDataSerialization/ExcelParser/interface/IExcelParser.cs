namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// Excel 解析器门面接口。
    /// 每种数据类型对应一个实现，封装该类型的全部处理逻辑：
    /// 元数据提取 → .fbs 生成 → .json 生成 → wrapper 生成。
    ///
    /// 编排器只依赖此接口，不感知具体实现。
    /// </summary>
    public interface IExcelParser
    {
        /// <summary>
        /// 解析 sheet 元数据（字段定义 / 枚举值等）。
        /// </summary>
        SheetDataInfo AnalyseSheet(IExcelSheetReader reader);

        /// <summary>
        /// 生成 .fbs schema 文件。
        /// </summary>
        void GenerateFbs(SheetDataInfo info, string outputPath);

        /// <summary>
        /// 生成 .json 中间数据文件。枚举类型可为空实现。
        /// </summary>
        void SerializeJson(SheetDataInfo info, IExcelSheetReader reader, string outputPath);

        /// <summary>
        /// 生成 wrapper C# 代码。枚举类型可为空实现。
        /// </summary>
        void CreateWrapper(SheetDataInfo info, string outputPath);
    }
}
