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
    [TableDescription("簽名檔")]
    public class Signature
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

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(256), ForeignKey("CreatedUser")]
        [Index("IX_CommonMessage_CreatedUserId")]
        public string CreatedUserId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }
    }
}
