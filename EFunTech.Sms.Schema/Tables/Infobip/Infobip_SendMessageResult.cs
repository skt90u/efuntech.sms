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
    public class Infobip_SendMessageResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        public SourceTable SourceTable { get; set; }

        public int SourceTableId { get; set; }

        [MaxLength(256)]
        [Required]
        [Index("IX_ClientCorrelator", IsUnique = true)]
        [ColumnDescription("發送簡訊識別碼(RequestId), 格式範例. 14348799713001264")]
        public string ClientCorrelator { get; set; }
        // ClientCorrelator 用途在於取得簡訊派送報表, 使用範例如下: 
        // DeliveryReportList deliveryReportList = smsClient.SmsMessagingClient.GetDeliveryReportsByRequestId(sendMessageResult.ClientCorrelator);

        public virtual ICollection<Infobip_SendMessageResultItem> SendMessageResults { get; set; }

        public virtual Infobip_ResourceReference ResourceRef { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        [ColumnDescription("可用餘額")]
        public decimal Balance { get; set; }

        // TODO: Migration 使用，未來要移除
        public int CopyId { get; set; }
        
    }
}
