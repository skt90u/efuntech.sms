using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class ReplyCcModel
	{
		public string OwnerId { get; set; }

		public bool Enabled { get; set; }

		public bool BySmsMessage { get; set; }

		public bool ByEmail { get; set; }

	}
}
