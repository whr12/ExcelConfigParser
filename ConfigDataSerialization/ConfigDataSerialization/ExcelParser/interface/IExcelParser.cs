namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// Excel 解析器接口（泛型）。每种数据类型对应一个实现。
    /// 无状态设计：AnalyseSheet 是纯函数，reader 进、SheetDataInfo 出。
    /// </summary>
    /// <typeparam name="T">解析结果类型（DataListSheetInfo / GlobalConfigSheetInfo / EnumSheetInfo）</typeparam>
    public interface IExcelParser<T> where T : SheetDataInfo
    {
        /// <summary>
        /// 解析 sheet 的元数据（字段定义、枚举值等）。
        /// 不做代码生成或序列化，仅提取结构化信息。
        /// </summary>
        T AnalyseSheet(IExcelSheetReader reader);
    }
}
