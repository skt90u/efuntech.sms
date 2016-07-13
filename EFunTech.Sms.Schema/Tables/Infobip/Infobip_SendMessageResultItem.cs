using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    public class Infobip_SendMessageResultItem
    {
        [Key]
        [MaxLength(256)]
        [ColumnDescription("MessageId <- 使用 GetDeliveryReportsByRequestId 回傳 DeliveryReport, 包含MessageId")]
        public string MessageId { get; set; }

        [Required]
        [ColumnDescription("簡訊發送回報")]
        public virtual Infobip_SendMessageResult SendMessageResult { get; set; }

        public MessageStatus MessageStatus { get; set; }

        public string MessageStatusString { get; set; }
        
        public String SenderAddress { get; set; }

        public String DestinationAddress { get; set; } // Mobile

        //ProcessLogKey
        //Price
        //ErrorMessageId

        public virtual Infobip_DeliveryReport DeliveryReport { get; set; }

        public string DestinationName { get; set; }

        public string Email { get; set; }
    }
}
