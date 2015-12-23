using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFunTech.Sms.Schema
{
    [TableDescription("開啟回覆轉寄")]
    public class ReplyCc
    {
        [Key]
        [ColumnDescription("此設定擁有者")]
        [MaxLength(256), ForeignKey("Owner")]
        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public const bool DefaultValue_Enabled = false;
        public const bool DefaultValue_BySmsMessage = true;
        public const bool DefaultValue_ByEmail = false;

        [ColumnDescription("啟用|停用通知")]
        public bool Enabled { get; set; }

        [ColumnDescription("啟用簡訊轉寄")]
        public bool BySmsMessage { get; set; }

        [ColumnDescription("啟用Email轉寄")]
        public bool ByEmail { get; set; }
    }
}
