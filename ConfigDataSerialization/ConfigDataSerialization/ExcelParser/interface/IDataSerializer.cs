namespace ConfigDataSerialization.ExcelParser
{
    /// <summary>
    /// 数据序列化器接口（泛型）。
    /// 收具体 SheetDataInfo + Excel 原始数据 → 产出可序列化的中间格式（.json）。
    /// </summary>
    /// <typeparam name="T">SheetDataInfo 的具体子类型</typeparam>
    public interface IDataSerializer<T> where T : SheetDataInfo
    {
        /// <summary>
        /// 从 Excel 读取数据行，生成用于 flatc 序列化的中间文件。
        /// </summary>
        void Serialize(T info, IExcelSheetReader reader, string outputPath);
    }
}
