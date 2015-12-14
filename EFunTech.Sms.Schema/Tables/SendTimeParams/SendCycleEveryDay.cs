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
    [TableDescription("每天發送參數")]
    public class SendCycleEveryDay
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [ColumnDescription("發送時間(Hour + Minute)")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SendTime { get; set; }

        [ColumnDescription("起始日期")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime StartDate { get; set; } 

        [ColumnDescription("結束日期")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime EndDate { get; set; } 

        public TimeSpan ClientTimezoneOffset { get; set; }
    }
}
