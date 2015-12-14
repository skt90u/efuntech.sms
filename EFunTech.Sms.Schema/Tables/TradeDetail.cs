using EFunTech.Sms.Schema;
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
    [TableDescription("聯絡人")]
    public class TradeDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("交易時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        [Index] // 20151127 Norman, 加上Index看看速度會不會變快
        public DateTime TradeTime { get; set; }

        [Required]
        [ColumnDescription("交易類別")]
        public TradeType TradeType { get; set; }

        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("交易點數")]
        public Decimal Point { get; set; }

        [ColumnDescription("交易說明")]
        public string Remark { get; set; }

        [MaxLength(128)] // SQL Server 不允許未設定長度的字串指定為Index
        [Index] // 20151127 Norman, 加上Index看看速度會不會變快			
        [Required]
        public virtual string OwnerId { get; set; }

        [Required]
        public virtual string TargetId { get; set; } 
    }
}
