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
        private #GEN_NAME#List _list;
        private readonly Dictionary<uint, #DEFINE_NAME#> _idToData = new();

        string ITableData.FileName => FILE_NAME;

        public #DEFINE_NAME# GetById(uint id)
        {
            _idToData.TryGetValue(id, out var data);
            return data;
        }

        public #DEFINE_NAME#? GetByIndex(int index)
        {
            if (index < 0 || index >= _list.DatasLength)
                return null;
            var item = _list.Datas(index);
            if (item == null) return null;
            return GetById(item.Value.Id);
        }

        public int GetCount()
        {
            return _list.DatasLength;
        }

        void ITableData.Load(byte[] bytes)
        {
            _byteBuffer = new ByteBuffer(bytes);
            _list = #GEN_NAME#List.GetRootAs#GEN_NAME#List(_byteBuffer);

            _idToData.Clear();
            for (int i = 0; i < _list.DatasLength; i++)
            {
                var item = _list.Datas(i);
                if (item != null)
                    _idToData[item.Value.Id] = new #DEFINE_NAME#(item.Value);
            }
        }

        void ITableData.Release()
        {
            _byteBuffer = null;
            _list = default;
            _idToData.Clear();
        }
    }
}
