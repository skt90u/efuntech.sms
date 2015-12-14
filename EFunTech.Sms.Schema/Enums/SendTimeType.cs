using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊發送時間類型")]
    public enum SendTimeType
    {
        [ColumnDescription("立即發送簡訊")]
        Immediately = 0,

        [ColumnDescription("預約發送簡訊")]
        Deliver = 1,

        [ColumnDescription("周期發送簡訊")]
        Cycle = 2
    }
}
