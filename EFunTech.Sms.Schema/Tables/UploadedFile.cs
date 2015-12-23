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
    [TableDescription("上傳檔案")]
    public class UploadedFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [Required]
        [ColumnDescription("上傳檔案名稱")]
        public string FileName { get; set; }

        [Required] // 由於未來是將伺服器放置在 Azure 中， 因此暫時沒有計畫要儲存檔案，若未來需要修改，請搜尋 attachment.SaveAs(filePath)
        [ColumnDescription("檔案儲存位置")]
        public string FilePath { get; set; }

        [Required]
        [ColumnDescription("檔案用途")]
        public UploadedFileType UploadedFileType { get; set; }

        [Required]
        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(256), ForeignKey("CreatedUser")]
        [Index("IX_UploadedFile_CreatedUserId")]
        public string CreatedUserId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; } 
    }
}
