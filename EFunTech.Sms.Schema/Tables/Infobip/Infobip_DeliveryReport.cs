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
    // https://developer.infobip.com/api#!/SendSMS/GET_1_smsmessaging_outbound_requests_deliveryReports
    [TableDescription("簡訊派送結果")]
    public class Infobip_DeliveryReport
    {
        [Required]
        [ColumnDescription("就是 Infobip_SendMessageResult.ClientCorrelator")]
        public string RequestId { get; set; }
        
        [Key]
        [ColumnDescription("MessageId")]
        [MaxLength(256), ForeignKey("SendMessageResultItem")]
        public string MessageId { get; set; }

        public virtual Infobip_SendMessageResultItem SendMessageResultItem { get; set; }

        [ColumnDescription("電信商實際發送時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SentDate { get; set; }

        [ColumnDescription("電信商實際發送完成時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime DoneDate { get; set; }

        [ColumnDescription("發送結果 ['DeliveredToTerminal' or 'DeliveryUncertain' or 'DeliveryImpossible' or 'MessageWaiting' or 'DeliveredToNetwork']")]
        public string StatusString { get; set; }

        public DeliveryReportStatus Status { get; set; }

        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("簡訊費用")]
        public decimal? Price { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }
    }
}
