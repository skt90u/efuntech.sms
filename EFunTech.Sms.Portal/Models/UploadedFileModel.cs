using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class UploadedFileModel
	{
		public int Id { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public UploadedFileType UploadedFileType { get; set; }

		public DateTime CreatedTime { get; set; }

	}
}
