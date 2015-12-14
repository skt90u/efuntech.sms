using EFunTech.Sms.Portal.Models.Common;
using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Criteria
{
    public class MemberSendMessageHistoryCriteriaModel : SearchTextCriteriaModel
    {
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public string Mobile { get; set; }

        //// 以逗號隔開, e.g. 
        //// 傳送中 = 0
        //// 成功   = 99,100,900
        //// 空號 = 103
        //// 電話號碼格式錯誤 = -3
        //// 逾時 = 101,102,104,105,106,107|-1|-2|-4|-5|-6|-8|-9|-32|-100|-101|-201|-202|-203
        //public string ReceiptStatus { get; set; }

        public int SendMessageQueueId { get; set; }
        public string ReceiptStatus { get; set; }
    }
}