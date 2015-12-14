using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class SendDeliverModel
	{
		public int SendMessageRuleId { get; set; }

        public DateTime SendTime { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }
	}
}
