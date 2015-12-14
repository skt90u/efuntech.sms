using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("發送訊息類型")]
    public enum SendMessageType
    {
        [ColumnDescription("SMS")]
        SmsMessage = 0,

        [ColumnDescription("App")]
        AppMessage = 1
    }
}
