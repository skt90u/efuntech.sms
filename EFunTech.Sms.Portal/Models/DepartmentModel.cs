using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;

namespace EFunTech.Sms.Portal.Models
{
	public class DepartmentModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

        //public List<ApplicationUserModel> Users{ get; set; }

        // ¤@¦|(3) = Department.Name + Department.Users.Count
        public string Summary { get; set; }
	}
}
