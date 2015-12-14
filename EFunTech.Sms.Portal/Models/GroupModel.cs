using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class GroupModel
	{
		public int Id { get; set; }

		public string CreatedUserId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool Deletable { get; set; }

        public bool Editable { get; set; }
	}
}
