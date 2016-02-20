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
    public class Every8d_SendMessageResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        public SourceTable SourceTable { get; set; }

        public int SourceTableId { get; set; }

        [Required]
        [ColumnDescription("預定發送時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SendTime { get; set; }

        [ColumnDescription("簡訊類別描述")]
        public string Subject { get; set; }

        [ColumnDescription("發送內容")]
        public string Content { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        [ColumnDescription("Every8d 發送後剩餘點數")]
        public double? CREDIT { get; set; }

        [ColumnDescription("Every8d 發送通數")]
        public int? SENDED { get; set; }

        [ColumnDescription("Every8d 本次發送扣除點數")]
        public double? COST { get; set; }

        [ColumnDescription("Every8d 無額度時發送的通數")]
        public int? UNSEND { get; set; }

        [MaxLength(256)]
        [Required]
        [Index("IX_BATCH_ID", IsUnique = true)]
        [ColumnDescription("Every8d 發送識別碼(可藉由本識別碼查詢發送狀態)")]
        public string BATCH_ID { get; set; }

        public virtual ICollection<Every8d_DeliveryReport> DeliveryReports { get; set; }
    }
}
