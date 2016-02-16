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
    [TableDescription("簡訊發送結果歷史紀錄")]
    public class SendMessageHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        ////////////////////////////////////////
        // 01 ~ 05

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendMessageRule.CreatedUser.Department.Id
        /// </remarks>
        [ColumnDescription("01. 部門編號")]
        public int? DepartmentId { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendMessageRule.CreatedUser.Id
        /// </remarks>
        [Required]
        [ColumnDescription("02. 建立簡訊規則的使用者編號")]
        [MaxLength(128)] // SQL Server 不允許未設定長度的字串指定為Index
        [Index] // 20151127 Norman, 加上Index看看速度會不會變快
        public string CreatedUserId { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendMessageRuleId
        /// </remarks>
        [Required]
        [ColumnDescription("03. 簡訊發送規則編號")]
        public int SendMessageRuleId { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.Id
        /// </remarks>
        [Required]
        [ColumnDescription("04. 簡訊發送任務編號")]
        public int SendMessageQueueId { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendMessageType
        /// </remarks>
        [Required]
        [ColumnDescription("05. 發送訊息類型(手機簡訊|APP簡訊)")]
        public SendMessageType SendMessageType { get; set; }

        ////////////////////////////////////////
        // 06 ~ 10

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendTime
        /// </remarks>
        [Required]
        [ColumnDescription("06. 預定發送時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        [Index] // 20151126 Norman, 加上Index看看速度會不會變快
        public DateTime SendTime { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendTitle
        /// </remarks>
        [ColumnDescription("07. 簡訊類別描述")]
        public string SendTitle { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendBody
        /// </remarks>
        [Required]
        [ColumnDescription("08. 發送內容")]
        public string SendBody { get; set; }

        /// <remarks>
        /// 資料來源： SendMessageQueue.SendCustType
        /// </remarks>
        [Required]
        [ColumnDescription("09. 單向|雙向 簡訊發送")]
        public SendCustType SendCustType { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.SendMessageResult.ClientCorrelator
        ///     Every8d: 
        /// </remarks>
        [MaxLength(256)]
        [Required]
        [ColumnDescription("10. 發送簡訊識別碼(RequestId), 格式範例. 14348799713001264")]
        public string RequestId { get; set; }

        ////////////////////////////////////////
        // 11 ~ 15

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
        public String MessageStatusString { get; set; }

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.SenderAddress
        ///     Every8d: Every8d_SendMessageResultItem.SenderAddress
        /// </remarks>
        [ColumnDescription("15. 發送來源")]
        public String SenderAddress { get; set; }

        ////////////////////////////////////////
        // 16 ~ 20

        /// <remarks>
        /// 資料來源： 
        ///     Infobip: Infobip_SendMessageResultItem.DestinationAddress
        ///     Every8d: Every8d_SendMessageResultItem.DestinationAddress
        /// </remarks>
        [ColumnDescription("16. 發送目的手機門號(E164格式)")]
        public String DestinationAddress { get; set; }

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

        /// <remarks>
        /// 資料來源： 使用 MessageCostInfo(SendBody, DestinationAddress) 重新計算 
        /// </remarks>
        [Required]
        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("24. 簡訊平台的花費點數(發送所需點數)")]
        public decimal MessageCost { get; set; }

        [ColumnDescription("25. 總結派送結果是否成功送達")]
        public bool Delivered { get; set; }

        [ColumnDescription("26. 收訊者姓名")]
        public String DestinationName { get; set; }

        [ColumnDescription("27. 發送地區")]
        public String Region { get; set; }

        [ColumnDescription("28. 建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        [Index] // 20151128 Norman, 加上Index看看速度會不會變快
        public DateTime CreatedTime { get; set; }
    }
}
