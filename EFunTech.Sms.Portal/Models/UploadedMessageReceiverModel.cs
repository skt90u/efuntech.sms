using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class UploadedMessageReceiverModel
	{
		public int Id { get; set; }

		public int RowNo { get; set; }

		public string Name { get; set; }

		public string Mobile { get; set; }

        public string E164Mobile { get; set; }

        public string Region { get; set; }

		public string Email { get; set; }

        public DateTime? SendTime { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }
        
        public string SendTimeString { get; set; } // 201512312359

        public bool UseParam { get; set; }

        public string Param1 { get; set; }

        public string Param2 { get; set; }

        public string Param3 { get; set; }

        public string Param4 { get; set; }

        public string Param5 { get; set; }

		public bool IsValid { get; set; }

		public string InvalidReason { get; set; }

		public DateTime CreatedTime { get; set; }

		public UploadedFileModel UploadedFile { get; set; }

        public int UploadedSessionId { get; set; }

	}
}
