namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// 代码生成器接口（泛型）。
    /// 收具体 SheetDataInfo → 产出代码文件（.fbs schema）。
    /// </summary>
    /// <typeparam name="T">SheetDataInfo 的具体子类型</typeparam>
    public interface ICodeGenerator<T> where T : SheetDataInfo
    {
        /// <summary>
        /// 根据 SheetDataInfo 生成代码文件。
        /// </summary>
        void Generate(T info, string outputPath);
    }
}
