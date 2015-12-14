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
    [TableDescription("常用簡訊")]
    public class CommonMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("標題")]
        public string Subject { get; set; }

        [Required]
        [ColumnDescription("內容")]
        public string Content { get; set; }

        [Required]
        [ColumnDescription("更新日期")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime UpdatedTime { get; set; }

        // 原本設定方式 (手動指定ForeignKey)
        //[Required]
        //[ColumnDescription("建立者")]
        //[MaxLength(256), ForeignKey("CreatedUser")]
        //public string CreatedUserId { get; set; }
        //public virtual ApplicationUser CreatedUser { get; set; } 

        // 後來設定方式 (Add-Migration 自動建立 ForeignKey)
        [Required]
        public virtual ApplicationUser CreatedUser { get; set; } 
    }
}
