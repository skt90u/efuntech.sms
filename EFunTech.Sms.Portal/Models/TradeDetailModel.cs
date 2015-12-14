using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class TradeDetailModel
	{
        //public int RowNo { get; set; }

		public int Id { get; set; }

        public DateTime TradeTime { get; set; }

        public TradeType TradeType { get; set; }
        public string TradeTypeString { get; set; }

		public Decimal Point { get; set; }

		public string Remark { get; set; }

		public string OwnerId { get; set; }

		public string TargetId { get; set; }

	}
}
