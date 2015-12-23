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
    [TableDescription("系統公告")]
    public class SystemAnnouncement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("顯示隱藏此公告")]
        public bool IsVisible { get; set; }

        [Required]
        [ColumnDescription("公告日期")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime PublishDate { get; set; }

        [Required]
        [ColumnDescription("公告內容")]
        [DateTimeKind(DateTimeKind.Utc)]
        public string Announcement { get; set; }

        [Required]
        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(256), ForeignKey("CreatedUser")]
        [Index("IX_SystemAnnouncement_CreatedUserId")]
        public string CreatedUserId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }
    }
}
