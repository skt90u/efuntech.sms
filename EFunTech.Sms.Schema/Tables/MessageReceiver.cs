using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace EFunTech.Sms.Schema
{
    [TableDescription("指定簡訊規則對應要傳遞訊息的接收者")]
    public class MessageReceiver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [ColumnDescription("資料編號")]
        public int RowNo { get; set; }

        [ColumnDescription("「姓名」非必填欄位。但主要為了報表使用，若有報表需求，建議填寫。")]
        public string Name { get; set; }

        [Required]
        [ColumnDescription("「手機門號」為必填欄位。")]
        public string Mobile { get; set; }

        [Required]
        [ColumnDescription("E164格式的「手機門號」為必填欄位。")]
        public string E164Mobile { get; set; }

        [Required]
        [ColumnDescription("發送地區")]
        public string Region { get; set; }

        [ColumnDescription("「電子郵件」非必填欄位。如果你要同時發簡訊到Email，則可填寫。")]
        public string Email { get; set; }

        /// <summary>
        /// 「傳送時間」非必填欄位。如果您要傳給每個收訊者的發送時間不相同，則務必逐筆設定發送日期。
        ///     如果要設定傳送日期，則此欄位之接受訊息者的接收時間必須全部填寫不能空白。Ex：要傳給100個用戶，但其中有 5 個用戶發送時間不同，則需一一設定100個發簡訊的時間。		
        ///     其格式為西元年月日時分（除年份外，其餘皆2位數  ）。   ex:200802051330，表示2008年2月5日下午1點30分傳送。									
        /// 傳送日期以此 Excel 檔案所設定的時間為準。
        /// </summary>        [ColumnDescription("「傳送時間」非必填欄位。如果您要傳給每個收訊者的發送時間不相同，則務必逐筆設定發送日期。")]
        [ColumnDescription("傳送時間(一般來說都為NULL，但是上傳檔案有可能不為空)")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? SendTime { get; set; }

        [ColumnDescription("簡訊類別描述")]
        public string SendTitle { get; set; }

        [Required]
        [ColumnDescription("發送內容")]
        public string SendBody { get; set; }

        [Required]
        [ColumnDescription("發送訊息類型(手機簡訊|APP簡訊)")]
        public SendMessageType SendMessageType { get; set; }

        [ColumnDescription("簡訊接收者類型")]
        public RecipientFromType RecipientFromType { get; set; }

        [Required]
        public virtual ApplicationUser CreatedUser { get; set; }

        [Required]
        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        [Required]
        [ColumnDescription("更新日期")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime UpdatedTime { get; set; }

        [Required]
        [ColumnDescription("簡訊字數")]
        public int MessageLength { get; set; }

        [Required]
        [ColumnDescription("簡訊總共幾則")]
        public int MessageNum { get; set; }

        [DecimalPrecision(15, 2)] // 1000000000000.00
        [Required]
        [ColumnDescription("簡訊花費點數(發送扣點)")]
        public decimal MessageCost { get; set; }

        [ColumnDescription("簡訊格式錯誤說明")]
        public string MessageFormatError { get; set; }
    }
}
