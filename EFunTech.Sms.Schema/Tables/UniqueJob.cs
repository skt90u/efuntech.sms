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
    /// <summary>
    /// 避免
    /// (1) 多個 BackgroundJob 發送相同的簡訊規則(sendMessageRuleId, sendTime)
    /// (2) 多個 BackgroundJob 接收相同的派送報表(requestId)
    /// </summary>
    [TableDescription("UniqueJobQueue")]
    public class UniqueJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [MaxLength(450)]
        [Required]
        [Index("IX_Signature", IsUnique = true)]
        [ColumnDescription("用來辨識Job，e.g. SendSMS(1, 2015/11/01 10:10:00)")]
        public string Signature { get; set; }

        [Required]
        [ColumnDescription("描述Job狀態，目前沒有甚麼作用")]
        public EfJobQueueStatus Status { get; set; }

        [Required]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }
    }
}
