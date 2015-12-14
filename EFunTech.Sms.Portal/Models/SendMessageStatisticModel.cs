using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class SendMessageStatisticModel
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

		public int TotalReceiverCount { get; set; }

		public Decimal TotalMessageCost { get; set; }

		public int TotalSuccess { get; set; }

		public int TotalSending { get; set; }

		public int TotalTimeout { get; set; }

		public DateTime CreatedTime { get; set; }

        //----------------------------------------
        // 針對 SectorStatistics 畫面所增加的欄位
        //----------------------------------------

        //部門	
        public string DepartmentName { get; set; }

        //姓名
        public string FullName { get; set; }

        //public int RowNo { get; set; }
        
	}
}
