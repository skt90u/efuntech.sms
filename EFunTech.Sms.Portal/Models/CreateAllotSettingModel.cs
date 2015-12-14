using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class CreateAllotSettingModel
	{
        public string[] ids { get; set; }

		public bool MonthlyAllot { get; set; }

		public int MonthlyAllotDay { get; set; }

		public Decimal MonthlyAllotPoint { get; set; }

        public DateTime? LastAllotTime { get; set; }

		public Decimal LimitMinPoint { get; set; }

		public Decimal LimitMaxPoint { get; set; }

	}
}
