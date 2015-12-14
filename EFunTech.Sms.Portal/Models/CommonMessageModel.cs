using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class CommonMessageModel
	{
		public int Id { get; set; }

		public string Subject { get; set; }

		public string Content { get; set; }

		public DateTime UpdatedTime { get; set; }

	}
}
