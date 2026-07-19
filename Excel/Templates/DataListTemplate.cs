using System.Collections.Generic;
using AIProject.Core;
using Google.FlatBuffers;

namespace #NAMESPACE#
{
#FIELD_COMMENTS#
    public class #DEFINE_NAME#Data : ITableData
    {
        private const string FILE_NAME = "#BINARY_FILE#";

        private ByteBuffer _byteBuffer;
        private #DEFINE_NAME#List _list;
        private readonly Dictionary<uint, #DEFINE_NAME#Row> _idToData = new();

        private bool _loaded;

        public string FileName => FILE_NAME;

        public #DEFINE_NAME#Row GetById(uint id)
        {
            _idToData.TryGetValue(id, out var data);
            return data;
        }

        public #DEFINE_NAME#Row? GetByIndex(int index)
        {
            if (index < 0 || index >= _list.DatasLength)
                return null;
            var item = _list.Datas(index);
            if (item == null) return null;
            return new #DEFINE_NAME#Row(item.Value);
        }

        public int GetCount()
        {
            return _list.DatasLength;
        }

        public void Load(byte[] bytes)
        {
            _byteBuffer = new ByteBuffer(bytes);
            _list = #DEFINE_NAME#List.GetRootAs#DEFINE_NAME#List(_byteBuffer);

            _idToData.Clear();
            for (int i = 0; i < _list.DatasLength; i++)
            {
                var item = _list.Datas(i);
                if (item != null)
                    _idToData[item.Value.Id] = new #DEFINE_NAME#Row(item.Value);
            }
        }

        public void Release()
        {
            _byteBuffer = null;
            _list = default;
            _idToData.Clear();
        }
    }

#ROW_CLASS#
}
