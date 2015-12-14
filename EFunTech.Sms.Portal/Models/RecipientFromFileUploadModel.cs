using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class RecipientFromFileUploadModel
	{
		public int SendMessageRuleId { get; set; }

		public int UploadedFileId { get; set; }

        public bool AddSelfToMessageReceiverList { get; set; }

	}
}
