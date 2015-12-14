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
    [TableDescription("預約發送")]
    public class SendDeliver
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [ColumnDescription("發送時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SendTime { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }
    }
}
