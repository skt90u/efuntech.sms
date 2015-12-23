using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace EFunTech.Sms.Schema
{
    [TableDescription("上傳的訊息接收者")]
    public class UploadedMessageReceiver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("資料編號")]
        public int RowNo { get; set; }

        [ColumnDescription("「姓名」非必填欄位。但主要為了報表使用，若有報表需求，建議填寫。")]
        public string Name { get; set; }

        [Required]
        [ColumnDescription("「手機門號」為必填欄位。")]
        public string Mobile { get; set; }

        // 不需要必填，因為支援檔案上傳名單，而上傳資料手機號碼可能錯誤
        [ColumnDescription("E164格式的「手機門號」為必填欄位。")]
        public string E164Mobile { get; set; }

        // 不需要必填，因為支援檔案上傳名單，而上傳資料手機號碼可能錯誤
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
        [ColumnDescription("傳送時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? SendTime { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }

        [ColumnDescription("傳送時間字串(可為空字串或者200802051330這樣的格式)")]
        public string SendTimeString { get; set; }

        public bool UseParam { get; set; }

        [ColumnDescription("參數一")]
        public string Param1 { get; set; }
        [ColumnDescription("參數二")]
        public string Param2 { get; set; }
        [ColumnDescription("參數三")]
        public string Param3 { get; set; }
        [ColumnDescription("參數四")]
        public string Param4 { get; set; }
        [ColumnDescription("參數五")]
        public string Param5 { get; set; }

        [ColumnDescription("是否為有效名單")]
        public bool IsValid { get; set; }

        [ColumnDescription("此筆資料無效原因")]
        public string InvalidReason { get; set; }

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(256), ForeignKey("CreatedUser")]
        [Index("IX_UploadedMessageReceiver_CreatedUserId")]
        public string CreatedUserId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }

        [Required]
        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        [ColumnDescription("上傳檔案")]
        [ForeignKey("UploadedFile")]
        public int? UploadedFileId { get; set; }

        public virtual UploadedFile UploadedFile { get; set; } // 可能來自檔案上傳

        /// <summary>
        /// 上傳名單識別碼
        /// 
        /// 由於每個允許使用者針對上傳名單，再進行新增
        /// 因此會發生沒有對應 UploadedFile 的狀況
        /// 
        /// 這時候，如何判斷是否為同一批上傳名單
        /// 我想到的方式是根據既有的 UploadedFile 的 Id 作為 識別碼
        /// </summary>
        [Required]
        public int UploadedSessionId { get; set; }

    }
}
