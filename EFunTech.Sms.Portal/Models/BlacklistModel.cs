using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class BlacklistModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Mobile { get; set; }

        public string E164Mobile { get; set; }

        public string Region { get; set; }	

		public bool Enabled { get; set; }

		public string Remark { get; set; }

		public DateTime UpdatedTime { get; set; }

        public string UpdatedUserName { get; set; }

        public string DecoratedValue_SequenceNo { get; set; }
        public string DecoratedValue_Enabled { get; set; }
        public string DecoratedValue_UpdatedTime { get; set; }
	}
}
