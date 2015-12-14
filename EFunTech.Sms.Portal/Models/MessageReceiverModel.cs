using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class MessageReceiverModel
	{
		public int Id { get; set; }

		public int SendMessageRuleId { get; set; }

		public int RowNo { get; set; }

		public string Name { get; set; }

        public string Mobile { get; set; }

        public string E164Mobile { get; set; }

        public string Region { get; set; }

		public string Email { get; set; }

		public DateTime? SendTime { get; set; }

		public string SendTitle { get; set; }

		public string SendBody { get; set; }

		public SendMessageType SendMessageType { get; set; }

		public RecipientFromType RecipientFromType { get; set; }

		public DateTime CreatedTime { get; set; }

		public DateTime UpdatedTime { get; set; }

		public int MessageLength { get; set; }

		public int MessageNum { get; set; }

		public Decimal MessageCost { get; set; }

		public string MessageFormatError { get; set; }

	}
}
