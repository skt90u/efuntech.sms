using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
    public class SystemAnnouncementModel
	{
        public int Id { get; set; }

        public bool IsVisible { get; set; }

        public DateTime PublishDate { get; set; }

        public string Announcement { get; set; }

        public DateTime CreatedTime { get; set; }

        public string PublishDateString { get; set; }

	}
}
