using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class LogItemModel
	{
		public int Id { get; set; }

		public string EntryAssembly { get; set; }

		public string Class { get; set; }

		public string Method { get; set; }

		public LogLevel LogLevel { get; set; }

		public string Message { get; set; }

		public string StackTrace { get; set; }

		public DateTime CreatedTime { get; set; }

		public string UserName { get; set; }

		public string Host { get; set; }

        public string FileName { get; set; }

        public int FileLineNumber { get; set; }
	}
}
