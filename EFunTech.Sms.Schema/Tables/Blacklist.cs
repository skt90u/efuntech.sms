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
    [TableDescription("黑名單")]
    public class Blacklist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("姓名")]
        public string Name { get; set; }

        [Required]
        [ColumnDescription("手機號碼")]
        public string Mobile { get; set; }

        // 不需要必填，因為支援檔案上傳名單，而上傳資料手機號碼可能錯誤
        [ColumnDescription("E164格式的「手機門號」為必填欄位。")]
        public string E164Mobile { get; set; }

        // 不需要必填，因為支援檔案上傳名單，而上傳資料手機號碼可能錯誤
        [ColumnDescription("發送地區")]
        public string Region { get; set; }

        [ColumnDescription("開啟/關閉")]
        public bool Enabled { get; set; }

        [ColumnDescription("備註")]
        public string Remark { get; set; }

        [Required]
        [ColumnDescription("更新日期")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime UpdatedTime{ get; set; }

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(256), ForeignKey("CreatedUser")]
        [Index("IX_Blacklist_CreatedUserId")]
        public string CreatedUserId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }

        [Required]
        [ColumnDescription("設定者帳號")]
        public string UpdatedUserName { get; set; }

        [ColumnDescription("上傳檔案")]
        [ForeignKey("UploadedFile")]
        public int? UploadedFileId { get; set; }

        public virtual UploadedFile UploadedFile { get; set; } // 可能來自檔案上傳
    }
}
