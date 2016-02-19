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
    [TableDescription("每周發送參數")]
    public class SendCycleEveryWeek
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        public string DayOfWeeks { get; set; } 

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

        public List<DayOfWeek> GetDayOfWeeks()
        {
            var result = new List<DayOfWeek>();

            string dayOfWeeks = this.DayOfWeeks ?? string.Empty;

            // dayOfWeeks 對應關係
            // 0000000 <- 每一天都沒選
            // 1000001 <- [Sunday, Saturday]

            for (int i = 0; i < dayOfWeeks.Length; i++)
            {
                if (dayOfWeeks[i] == '1')
                {
                    var dayOfWeek = (DayOfWeek)i;
                    result.Add(dayOfWeek);
                }
            }

            return result;
        }

        public void SetDayOfWeeks(List<DayOfWeek> value)
        {
            string result = string.Empty;

            for (int i = 0; i < 7; i++)
                result += value.Contains((DayOfWeek)i) ? "1" : "0";
            
            this.DayOfWeeks = result;
        }
    }
}
