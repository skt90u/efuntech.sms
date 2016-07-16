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
    [TableDescription("簡訊重送結果歷史紀錄")]
    public class SendMessageRetryHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        public int SendMessageHistoryId { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.SendMessageResult.ClientCorrelator
        ///     Every8d: 
        /// </remarks>
        [MaxLength(256)]
        [Required]
        [ColumnDescription("10. 發送簡訊識別碼(RequestId), 格式範例. 14348799713001264")]
        public string RequestId { get; set; }

        [MaxLength(256)]
        [Required]
        [ColumnDescription("11. 簡訊供應商, 目前有 InfobipNormalQuality、InfobipHighQuality、InfobipHighQuality")]
        public string ProviderName { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.MessageId
        ///         對於 Infobip 而言，使用 GetDeliveryReportsByRequestId 回傳 DeliveryReport, 包含 MessageId，這個編號是對應 Infobip_SendMessageResultItem.MessageId
        ///     Every8d: null
        /// </remarks>
        [MaxLength(256)]
        [ColumnDescription("12. MessageId")]
        public string MessageId { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.MessageStatus
        ///     Every8d: null
        /// </remarks>
        [ColumnDescription("13. MessageStatus")]
        public MessageStatus MessageStatus { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.MessageStatusString
        ///     有以下幾種類型
        ///         MessageNotSent
        ///         MessageSent
        ///         MessageWaitingForDelivery
        ///         MessageNotDelivered
        ///         MessageDelivered
        ///         NetworkNotAllowed
        ///         NetworkNotAvailable
        ///         InvalidDestinationAddress
        ///         MessageDeliveryUnknown
        ///         RouteNotAvailable
        ///         InvalidSourceAddress
        ///         NotEnoughCredits
        ///         MessageRejected
        ///         MessageExpired
        ///         SystemError
        ///         MessageAccepted
        ///     Every8d: null
        /// </remarks>
        [ColumnDescription("14. 描述伺服器接收傳送命令的回傳結果")]
        public string MessageStatusString { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.SenderAddress
        ///     Every8d: Every8d_SendMessageResultItem.SenderAddress
        /// </remarks>
        [ColumnDescription("15. 發送來源")]
        public string SenderAddress { get; set; }

        ////////////////////////////////////////
        // 16 ~ 20

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.DestinationAddress
        ///     Every8d: Every8d_SendMessageResultItem.DestinationAddress
        /// </remarks>
        [ColumnDescription("16. 發送目的手機門號(E164格式)")]
        public string DestinationAddress { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResult.CreatedTime
        ///     Every8d: Every8d_SendMessageResult.CreatedTime
        /// </remarks>
        [ColumnDescription("17. 簡訊訂閱結果建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SendMessageResultCreatedTime { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_DeliveryReport.SentDate
        ///     Every8d: 
        /// </remarks>
        [ColumnDescription("18. 電信商實際發送時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? SentDate { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_DeliveryReport.DoneDate
        ///     Every8d:
        /// </remarks>
        [ColumnDescription("19. 電信商實際發送完成時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DoneDate { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_DeliveryReport.Status
        ///     Every8d: 
        /// </remarks>
        [ColumnDescription("20. 發送結果")]
        public DeliveryReportStatus DeliveryStatus { get; set; }

        ////////////////////////////////////////
        // 21 ~ 25

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_DeliveryReport.StatusString
        ///     有以下幾種類型
        ///         DeliveredToTerminal
        ///         DeliveryUncertain
        ///         DeliveryImpossible
        ///         MessageWaiting
        ///         DeliveredToNetwork
        ///     Every8d: 
        /// </remarks>
        [ColumnDescription("21. 發送結果")]
        public string DeliveryStatusString { get; set; }

        /// <remarks>
        /// 資料來源： Infobip_DeliveryReport.Price
        /// </remarks>
        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("22. 簡訊供應商回傳的簡訊費用")]
        public decimal Price { get; set; }

        /// <remarks>
        /// 資料來源： Infobip_DeliveryReport.CreatedTime
        /// </remarks>
        [ColumnDescription("23. 簡訊派送結果建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? DeliveryReportCreatedTime { get; set; }

        [ColumnDescription("25. 總結派送結果是否成功送達")]
        public bool Delivered { get; set; }

        [ColumnDescription("28. 建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        [Index] // 20151128 Norman, 加上Index看看速度會不會變快
        public DateTime CreatedTime { get; set; }

        public string Email { get; set; }
    }
}
