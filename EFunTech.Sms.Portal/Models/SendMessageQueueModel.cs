using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class SendMessageQueueModel
	{
        public int RowNo { get; set; }
        public int Id { get; set; }

		public SendMessageType SendMessageType { get; set; }

        public DateTime SendTime { get; set; }

		public string SendTitle { get; set; }

		public string SendBody { get; set; }

		public SendCustType SendCustType { get; set; }

		public int TotalReceiverCount { get; set; }

		public int TransmissionCount { get; set; }

		public int SuccessCount { get; set; }

		public int FailureCount { get; set; }

		public Decimal TotalMessageCost { get; set; }

		public int SendMessageRuleId { get; set; }

	}
}
