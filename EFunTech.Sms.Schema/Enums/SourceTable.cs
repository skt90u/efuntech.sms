using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("資料表來源")]
    public enum SourceTable
    {
        [ColumnDescription("簡訊發送任務")]
        SendMessageQueue = 0,

        [ColumnDescription("簡訊發送結果歷史紀錄")]
        SendMessageHistory = 1,
    }
}
