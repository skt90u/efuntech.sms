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
    [TableDescription("簡訊發送規則")]
    public class SendMessageRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        // 01 ~ 05

        [ColumnDescription("簡訊類別描述")]
        public string SendTitle { get; set; }

        [Required]
        [ColumnDescription("發送內容")]
        public string SendBody { get; set; }

        #region 簡訊接收者
        [Required]
        [ColumnDescription("簡訊接收者類型")]
        public RecipientFromType RecipientFromType { get; set; }

        /// <summary>
        /// 收訊人來源
        ///     檔案: 檔案路徑
        /// </summary>
        public virtual RecipientFromFileUpload RecipientFromFileUpload { get; set; }

        /// <summary>
        /// 收訊人來源
        ///     聯絡人: 聯絡人編號
        /// </summary>
        public virtual RecipientFromCommonContact RecipientFromCommonContact { get; set; }

        /// <summary>
        /// 收訊人來源
        ///     群組: 群組編號
        /// </summary>
        public virtual RecipientFromGroupContact RecipientFromGroupContact { get; set; }

        /// <summary>
        /// 收訊人來源
        ///     手動輸入: 手機門號(以逗號隔開)
        /// </summary>
        public virtual RecipientFromManualInput RecipientFromManualInput { get; set; }

        #endregion

        #region 簡訊發送時間
        [Required]
        [ColumnDescription("簡訊發送時間類型")]
        public SendTimeType SendTimeType { get; set; }

        /// <summary>
        /// 預約發送
        ///     日期時間
        /// </summary>
        [ColumnDescription("預約發送時間參數")]
        public virtual SendDeliver SendDeliver { get; set; }

        /// <summary>
		/// 週期發送
        ///     每天 + {發送時間, 起始日期, 結束日期}
        ///     每週 + {發送時間, 起始日期, 結束日期, 星期一到日(可複選)}
        ///     每月 + {發送時間, 起始日期, 結束日期, 發送日(01~31)}
        ///     每年 + {發送時間, 起始日期, 結束日期, 發送日期}
        /// </summary>
        [ColumnDescription("簡訊發送時間參數(每天)")]
        public virtual SendCycleEveryDay SendCycleEveryDay { get; set; }

        [ColumnDescription("簡訊發送時間參數(每周)")]
        public virtual SendCycleEveryWeek SendCycleEveryWeek { get; set; }

        [ColumnDescription("簡訊發送時間參數(每月)")]
        public virtual SendCycleEveryMonth SendCycleEveryMonth { get; set; }

        [ColumnDescription("簡訊發送時間參數(每年)")]
        public virtual SendCycleEveryYear SendCycleEveryYear { get; set; }
        
        #endregion

        [Required]
        [ColumnDescription("單向|雙向 簡訊發送")]
        public SendCustType SendCustType { get; set; }

        [Required]
        [ColumnDescription("是否為參數式")]
        public bool UseParam{ get; set; }

        [Required]
        [ColumnDescription("發送訊息類型(手機簡訊|APP簡訊)")]
        public SendMessageType SendMessageType { get; set; }

        [ColumnDescription("發送通數")]
        public int TotalReceiverCount { get; set; }

        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("花費點數")]
        public decimal TotalMessageCost { get; set; }

        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("剩餘點數(新增簡訊規則當下剩餘點數)")]
        public decimal RemainingSmsBalance { get; set; }

        public SendMessageRuleStatus SendMessageRuleStatus { get; set; }

        [Required]
        public virtual ApplicationUser CreatedUser { get; set; }

        [Required]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        // 20150930 Norman, 由於 SendMessageRule 可以被刪除，因此不再跟 SendMessageRule 綁在一起，否則連發送紀錄都必須刪除(ForeignKey)
        //public virtual ICollection<SendMessageQueue> SendMessageQueues { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }

        [ColumnDescription("發送來源")]
        public string SenderAddress { get; set; }

        public const string DefaultSenderAddress = "Zutech";
    }
}
