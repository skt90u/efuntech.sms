using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;

namespace EFunTech.Sms.Portal.Models
{
    public class SendCycleEveryWeekModel
	{
		public int SendMessageRuleId { get; set; }

		public string DayOfWeeks { get; set; }

        public DateTime SendTime { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }

        public List<DayOfWeek> GetDayOfWeeks()
        {
            List<DayOfWeek> result = new List<DayOfWeek>();

            string dayOfWeeks = this.DayOfWeeks ?? string.Empty;

            // dayOfWeeks 對應關係
            // 0000000 <- 每一天都沒選
            // 1000001 <- [Sunday, Saturday]

            for (int i = 0; i < dayOfWeeks.Length; i++)
            {
                if (dayOfWeeks[i] == '1')
                {
                    DayOfWeek dayOfWeek = (DayOfWeek)i;
                    result.Add(dayOfWeek);
                }
            }

            return result;
        }

	}
}
