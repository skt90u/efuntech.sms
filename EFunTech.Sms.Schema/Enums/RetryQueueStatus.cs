using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊重送序列目前狀態")]
    public enum RetryQueueStatus
    {
        [ColumnDescription("等待發送")]
        Pending = 0,
        [ColumnDescription("發送中，等待接收結果")]
        Sending = 1,
        [ColumnDescription("發送成功")]
        Success = 2,
        [ColumnDescription("發送失敗")]
        Failure = 3,
        [ColumnDescription("Timeout")]
        Timeout = 4,
    }
}
