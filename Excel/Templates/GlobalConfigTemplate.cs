using AIProject.Core;
using Google.FlatBuffers;

namespace #NAMESPACE#
{
    public class #DEFINE_NAME#Data : ITableData
    {
        private const string FILE_NAME = "#BINARY_FILE#";

        private ByteBuffer _byteBuffer;
        private #DEFINE_NAME# _data;

        public string FileName => FILE_NAME;

#PROPERTIES#

        public void Load(byte[] bytes)
        {
            _byteBuffer = new ByteBuffer(bytes);
            _data = #DEFINE_NAME#.GetRootAs#DEFINE_NAME#(_byteBuffer);
        }

        public void Release()
        {
            _byteBuffer = null;
            _data = default;
        }
    }
}
