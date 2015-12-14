using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class RecipientFromManualInputModel
	{
		public int SendMessageRuleId { get; set; }

		public string PhoneNumbers { get; set; }

	}
}
