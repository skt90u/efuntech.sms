using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊規則目前狀態")]
    public enum SendMessageRuleStatus
    {
        [ColumnDescription("正在建立簡訊規則以及相關資料")]
        Prepare, 

        [ColumnDescription("簡訊規則以及相關資料已經備妥，可以準備發送")]
        Ready,

        [ColumnDescription("正在發送簡訊規則")]
        Sending,

        [ColumnDescription("簡訊規則已發送完畢")]
        Sent, // 判斷是否為預扣類型，如果是的話在執行完畢就可以將狀態直接改成 Finish

        [ColumnDescription("簡訊規則發送任務已完成")]
        Finish,

        [ColumnDescription("簡訊規則正在更新")]
        Updating,

        [ColumnDescription("簡訊規則正在刪除")]
        Deleting, 

        // 生命週期
        //  - 立即發送
        //      Prepare -> Ready -> Sending -> Sent -> Finish
        //  - 預約發送
        //      Prepare -> Ready -> Sending -> Sent -> Finish
        //  - 週期發送
        //      Prepare -> n 個 (Ready -> Sending -> Sent) -> Finish
    }
}
