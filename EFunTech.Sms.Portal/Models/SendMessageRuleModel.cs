using EFunTech.Sms.Core;
using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class SendMessageRuleModel
	{
		public int Id { get; set; }

		public string SendTitle { get; set; }

		public string SendBody { get; set; }

		public RecipientFromType RecipientFromType { get; set; }

		public RecipientFromFileUploadModel RecipientFromFileUpload { get; set; }

		public RecipientFromCommonContactModel RecipientFromCommonContact { get; set; }

		public RecipientFromGroupContactModel RecipientFromGroupContact { get; set; }

		public RecipientFromManualInputModel RecipientFromManualInput { get; set; }

		public SendTimeType SendTimeType { get; set; }

		public SendDeliverModel SendDeliver { get; set; }

		public SendCycleEveryDayModel SendCycleEveryDay { get; set; }

		public SendCycleEveryWeekModel SendCycleEveryWeek { get; set; }

		public SendCycleEveryMonthModel SendCycleEveryMonth { get; set; }

		public SendCycleEveryYearModel SendCycleEveryYear { get; set; }

		public SendCustType SendCustType { get; set; }

		public bool UseParam { get; set; }

		public SendMessageType SendMessageType { get; set; }

		public int TotalReceiverCount { get; set; }

		public Decimal TotalMessageCost { get; set; }

		public Decimal RemainingSmsBalance { get; set; }

		public SendMessageRuleStatus SendMessageRuleStatus { get; set; }

		public DateTime CreatedTime { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }

        public string SenderAddress { get; set; }

        // 預約簡訊
        public DateTime? SendTime { get; set; }  // 預約發送時間	

        // 週期簡訊
        public DateTime? StartDate { get; set; } // 起始時間
        public DateTime? EndDate { get; set; } // 結束時間
        public string CycleString { get; set; } // 週期

        public void UpdateClientTimezoneOffset(TimeSpan clientTimezoneOffset)
        {
            var model = this;

            model.ClientTimezoneOffset = clientTimezoneOffset;

            if (model.SendDeliver != null)
            {
                model.SendDeliver.ClientTimezoneOffset = clientTimezoneOffset;
            }

            if (model.SendCycleEveryDay != null)
            {
                model.SendCycleEveryDay.ClientTimezoneOffset = clientTimezoneOffset;
            }
            if (model.SendCycleEveryWeek != null)
            {
                model.SendCycleEveryWeek.ClientTimezoneOffset = clientTimezoneOffset;
            }
            if (model.SendCycleEveryMonth != null)
            {
                model.SendCycleEveryMonth.ClientTimezoneOffset = clientTimezoneOffset;
            }
            if (model.SendCycleEveryYear != null)
            {
                model.SendCycleEveryYear.ClientTimezoneOffset = clientTimezoneOffset;
            }
        }
    }
}
