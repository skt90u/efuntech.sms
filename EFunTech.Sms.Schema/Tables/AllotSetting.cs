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
    [TableDescription("撥點設定")]
    public class AllotSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("是否每月固定撥入(是 = 每月固定撥入，否 = 低於設定點數時，自動撥點)")]
        public bool MonthlyAllot { get; set; }

        [ColumnDescription("每月撥入日期")]
        public int MonthlyAllotDay { get; set; }

        [ColumnDescription("每月撥入點數")]
        public Decimal MonthlyAllotPoint { get; set; }

        [ColumnDescription("撥入點數當時的時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime? LastAllotTime { get; set; }

        [ColumnDescription("額度下限點數")]
        public Decimal LimitMinPoint { get; set; }

        [ColumnDescription("自動補足點數")]
        public Decimal LimitMaxPoint { get; set; }

        [Required]
        public virtual ApplicationUser Owner { get; set; }
    }
}