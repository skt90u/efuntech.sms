using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class DeliveryReportQueueModel
	{
		public int Id { get; set; }

		public int SendMessageQueueId { get; set; }

		public string RequestId { get; set; }

		public string ProviderName { get; set; }

		public DateTime CreatedTime { get; set; }

		public int SendMessageResultItemCount { get; set; }

		public int DeliveryReportCount { get; set; }

	}
}
