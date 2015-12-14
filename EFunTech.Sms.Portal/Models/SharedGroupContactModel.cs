using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class SharedGroupContactModel
	{
		public int GroupId { get; set; }

		public GroupModel Group { get; set; }

		public string ShareToUserId { get; set; }

	}
}
