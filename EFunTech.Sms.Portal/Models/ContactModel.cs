using EFunTech.Sms.Schema;

namespace EFunTech.Sms.Portal.Models
{
	public class ContactModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Mobile { get; set; }

        public string E164Mobile { get; set; }

        public string Region { get; set; }	

		public string HomePhoneArea { get; set; }

		public string HomePhone { get; set; }

		public string CompanyPhoneArea { get; set; }

		public string CompanyPhone { get; set; }

		public string CompanyPhoneExt { get; set; }

		public string Email { get; set; }

		public string Msn { get; set; }

		public string Description { get; set; }

		public string Birthday { get; set; }

		public string ImportantDay { get; set; }

		public Gender Gender { get; set; }

		public string Groups { get; set; }

        /// <summary>
        /// 從指定群組中移除
        /// </summary>
        public int RemoveFromGroupId { get; set; }

        /// <summary>
        /// 加入指定群組
        /// </summary>
        public int JoinToGroupId { get; set; }

	}
}
