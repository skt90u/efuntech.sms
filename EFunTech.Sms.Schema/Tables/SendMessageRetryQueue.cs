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
    [TableDescription("簡訊重送序列")]
    public class SendMessageRetryQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("簡訊發送結果歷史紀錄")]
        [ForeignKey("SendMessageHistory")]
        public int SendMessageHistoryId { get; set; }

        public virtual SendMessageHistory SendMessageHistory { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        [Index] // 20151128 Norman, 加上Index看看速度會不會變快
        public DateTime CreatedTime { get; set; }

        [MaxLength(256)]
        [Required]
        [ColumnDescription("簡訊供應商, 目前有 InfobipNormalQuality、InfobipHighQuality、InfobipHighQuality")]
        public string ProviderName { get; set; }

        [ColumnDescription("簡訊重送序列目前狀態")]
        public RetryQueueStatus RetryQueueStatus { get; set; }
    }
}
