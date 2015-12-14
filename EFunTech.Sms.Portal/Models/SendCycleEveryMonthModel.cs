using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class SendCycleEveryMonthModel
	{
		public int SendMessageRuleId { get; set; }

        public DateTime SendTime { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }
	}
}
