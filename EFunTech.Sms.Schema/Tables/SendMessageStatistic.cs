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
    [TableDescription("簡訊發送結果統計表")]
    public class SendMessageStatistic
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
        [Index] // 20151127 Norman, 加上Index看看速度會不會變快
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
        [Index] // 20151127 Norman, 加上Index看看速度會不會變快
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

        [Required]
        [ColumnDescription("12. 發送通數")]
        public int TotalReceiverCount { get; set; } // TODO: 這個名稱不好，會誤會成總接收通數，要改掉

        [Required]
        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("13. 花費點數(發送所需點數)")]
        public decimal TotalMessageCost { get; set; }

        [Required]
        [ColumnDescription("14. 成功接收")]
        public int TotalSuccess { get; set; } 

        [Required]
        [ColumnDescription("15. 傳送中通數")]
        public int TotalSending { get; set; }

        ////////////////////////////////////////
        // 16 ~ 17

        [Required]
        //[ColumnDescription("16. 逾期收訊")]
        [ColumnDescription("16. 傳送失敗")]
        public int TotalTimeout { get; set; }

        [ColumnDescription("17. 資料建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }
    }
}
