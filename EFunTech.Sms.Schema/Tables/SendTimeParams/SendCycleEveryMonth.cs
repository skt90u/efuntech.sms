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
    /// <example>
    /// SendCycleEveryDay data = new SendCycleEveryDay
    /// {
    ///     SendMessageRuleId = 1,
    ///     Hour = 2,
    ///     Minute = 3,
    ///     DayOfWeeks = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday},
    ///     StartDate = DateTime.MinValue,
    ///     EndDate = DateTime.MaxValue
    /// };
    /// 
    /// {  
    ///    "SendMessageRuleId":1,
    ///    "Hour":2,
    ///    "Minute":3,
    ///    "DayOfWeeks":[  
    ///       1,
    ///       3
    ///    ],
    ///     "StartDate":"0001-01-01T00:00:00",
    ///     "EndDate":"9999-12-31T23:59:59.9999999"
    ///  }
    /// </example>
    [TableDescription("每月發送參數")]
    public class SendCycleEveryMonth
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [ColumnDescription("發送時間(Day + Hour + Minute)")]
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
