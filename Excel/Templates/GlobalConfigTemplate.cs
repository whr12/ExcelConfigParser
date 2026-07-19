using AIProject.Core;
using Google.FlatBuffers;

namespace #NAMESPACE#
{
    public class #DEFINE_NAME#Data : ITableData
    {
        private const string FILE_NAME = "#BINARY_FILE#";

        private ByteBuffer _byteBuffer;
        private #GEN_NAME# _data;

        string ITableData.FileName => FILE_NAME;

#PROPERTIES#

        void ITableData.Load(byte[] bytes)
        {
            _byteBuffer = new ByteBuffer(bytes);
            _data = #GEN_NAME#.GetRootAs#GEN_NAME#(_byteBuffer);
        }

        void ITableData.Release()
        {
            _byteBuffer = null;
            _data = default;
        }
    }
}
