using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("行動電話(國碼)")]
    public enum MobileCountry
    {
        [ColumnDescription("台灣")]
        Taiwan = 886,

        [ColumnDescription("中國大陸")]
        Chinese = 86,
    }
}
