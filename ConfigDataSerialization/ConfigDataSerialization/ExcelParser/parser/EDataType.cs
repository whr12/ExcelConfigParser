using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDataSerialization.ExcelParser
{
    public enum EDataType
    {
        Int32,
        Int64,
        Float,
        Double,
        Boolean,
        String,
        Array,
        Map,
        Enum,
        CustomType,
    }
}
