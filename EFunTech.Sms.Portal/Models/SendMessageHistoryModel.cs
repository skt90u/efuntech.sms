using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class SendMessageHistoryModel
	{
		public int Id { get; set; }

        public int? DepartmentId { get; set; }

		public string CreatedUserId { get; set; }

		public int SendMessageRuleId { get; set; }

		public int SendMessageQueueId { get; set; }

		public SendMessageType SendMessageType { get; set; }

		public DateTime SendTime { get; set; }

		public string SendTitle { get; set; }

		public string SendBody { get; set; }

		public SendCustType SendCustType { get; set; }

		public string RequestId { get; set; }

		public string ProviderName { get; set; }

		public string MessageId { get; set; }

		public MessageStatus MessageStatus { get; set; }

		public string MessageStatusString { get; set; }

		public string SenderAddress { get; set; }

		public string DestinationAddress { get; set; }

		public DateTime SendMessageResultCreatedTime { get; set; }

		public DateTime SentDate { get; set; }

		public DateTime DoneDate { get; set; }

		public DeliveryReportStatus DeliveryStatus { get; set; }

		public string DeliveryStatusString { get; set; }

		public Decimal Price { get; set; }

		public DateTime DeliveryReportCreatedTime { get; set; }

		public Decimal MessageCost { get; set; }

		public bool Delivered { get; set; }

        public string DestinationName { get; set; }

        public String Region { get; set; }

        ////////////////////////////////////////
        // 擴充欄位

        //public int RowNo { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string DeliveryStatusChineseString { get; set; }
	}
}
