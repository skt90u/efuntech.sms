using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class WebAuthorizationModel
	{
		public int Id { get; set; }

		public string ControllerName { get; set; }

		public string ActionName { get; set; }

		public string Roles { get; set; }

		public string Users { get; set; }

		public string Remark { get; set; }

	}
}
