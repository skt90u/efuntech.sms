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
    /// <summary>
    /// InfobipSmsProvider: 
    ///     (1) 當透過 InfobipSmsProvider 發送簡訊
    ///     (2) 取得發送簡訊回傳結果(Infobip_SendMessageResult)
    ///     (3) 將 Infobip_SendMessageResult 裡面的 ClientCorrelator 儲存到 DeliveryReportQueue.RequestId
    ///     (4) Hangfire 每固定時間依據 DeliveryReportQueue 去抓取發送簡訊派送結果，並寫入 Infobip_DeliveryReport
    /// Every8dSmsProvider: 
    ///     (1) 當透過 Every8dSmsProvider 發送簡訊
    ///     (2) 取得發送簡訊回傳結果(Every8dSmsProvider_SendMessageResult)
    ///     (3) 將 Every8dSmsProvider_SendMessageResult 裡面的 BatchId 儲存到 DeliveryReportQueue.RequestId
    ///     (4) Hangfire 每固定時間依據 DeliveryReportQueue 去抓取發送簡訊派送結果，並寫入 Every8d_DeliveryReport
    /// </summary>
    [TableDescription("簡訊派送結果等待取回序列")]
    public class DeliveryReportQueue
    {
        public static readonly TimeSpan QueryInterval = new TimeSpan(1, 0, 0, 0, 0); // 一天

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [Required]
        public int SourceTableId { get; set; }

        public SourceTable SourceTable { get; set; }
        

        [MaxLength(256)]
        [Required]
        [Index("IX_RequestId", IsUnique = true)]
        [ColumnDescription("發送簡訊識別碼(RequestId), 格式範例. 14348799713001264")]
        public string RequestId { get; set; }

        [MaxLength(256)]
        [Required]
        [ColumnDescription("11. 簡訊供應商, 目前有 InfobipNormalQuality、InfobipHighQuality、InfobipHighQuality")]
        [Index] // 20151128 Norman, 加上Index看看速度會不會變快
        public string ProviderName { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        public int SendMessageResultItemCount { get; set; }
        public int DeliveryReportCount { get; set; }
    }
}
