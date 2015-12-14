using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class RecipientFromGroupContactModel
	{
		public int SendMessageRuleId { get; set; }

		public string ContactIds { get; set; }

	}
}
