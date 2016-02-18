using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊重送結果")]
    public enum RetryResult
    {
        [ColumnDescription("未知(尚未重送|重送中，尚未得知結果)")]
        Unknown = 0,
        [ColumnDescription("重送成功")]
        Success = 1,
        [ColumnDescription("重送失敗，已達重送上限")]
        Failure = 2,
    }
}
