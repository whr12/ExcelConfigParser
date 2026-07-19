using System.Collections.Generic;
using AIProject.Core;
using Google.FlatBuffers;

namespace GameConfig
{
#FIELD_COMMENTS#
    public class #DEFINE_NAME#Data : ITableData
    {
        private const string FILE_NAME = "#BINARY_FILE#";

        private ByteBuffer _byteBuffer;
        private #DEFINE_NAME#List _list;
        private readonly Dictionary<uint, #DEFINE_NAME#> _idToData;

        public string FileName => FILE_NAME;

        public #DEFINE_NAME#Data()
        {
            _idToData = new Dictionary<uint, #DEFINE_NAME#>();
        }

        public #DEFINE_NAME# GetById(uint id)
        {
            _idToData.TryGetValue(id, out var data);
            return data;
        }

        public #DEFINE_NAME#? GetByIndex(int index)
        {
            if (index < 0 || index >= _list.DatasLength)
                return null;
            return _list.Datas(index);
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
                    _idToData[item.Value.Id] = item.Value;
            }
        }

        public void Release()
        {
            _byteBuffer = null;
            _list = default;
            _idToData.Clear();
        }
    }
}
