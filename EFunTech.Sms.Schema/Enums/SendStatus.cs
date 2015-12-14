using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("發送狀態(簡訊處理狀態)")]
    public enum SendStatus
    {
        [ColumnDescription("傳送中")]
        OneWay,

        [ColumnDescription("成功接收")]
        TwoWay
    }
}
