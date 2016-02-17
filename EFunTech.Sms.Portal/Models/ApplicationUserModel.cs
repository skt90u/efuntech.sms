using EFunTech.Sms.Schema;
using System;

namespace EFunTech.Sms.Portal.Models
{
	public class ApplicationUserModel
	{
		public string ParentId { get; set; }

		public string FullName { get; set; }

		public Gender Gender { get; set; }

		public string ContactPhone { get; set; }

		public string ContactPhoneExt { get; set; }

		public string AddressCountry { get; set; }

		public string AddressArea { get; set; }

		public string AddressZip { get; set; }

		public string AddressStreet { get; set; }

		public DepartmentModel Department { get; set; }

		public string EmployeeNo { get; set; }

		public CreditWarningModel CreditWarning { get; set; }

		public ReplyCcModel ReplyCc { get; set; }

		public bool ForeignSmsEnabled { get; set; }

		public Decimal SmsBalance { get; set; }

		public DateTime SmsBalanceExpireDate { get; set; }

		public bool Enabled { get; set; }

		public AllotSettingModel AllotSetting { get; set; }

		public string Email { get; set; }

		public bool EmailConfirmed { get; set; }

		public string PasswordHash { get; set; }

		public string SecurityStamp { get; set; }

		public string PhoneNumber { get; set; }

		public bool PhoneNumberConfirmed { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public DateTime? LockoutEndDateUtc { get; set; }

		public bool LockoutEnabled { get; set; }

		public int AccessFailedCount { get; set; }

		public string Id { get; set; }

		public string UserName { get; set; }

        public bool CanEditDepartment { get; set; }
        public bool CanEditSmsProviderType { get; set; }

        public SmsProviderType SmsProviderType { get; set; }

        //----------------------------------------
        // 針對 DepartmentManager 畫面所增加的欄位
        //----------------------------------------

        /// <summary>
        /// 是否可執行 啟用/關閉
        /// </summary>
        public bool Activatable { get; set; }

        /// <summary>
        /// 是否可執行 修改帳號 
        /// </summary>
        public bool Maintainable { get; set; }

        /// <summary>
        /// 是否可執行 刪除帳號
        /// </summary>
        public bool Deletable { get; set; }

        public int? DepartmentId { get; set; } // DepartmentId = 0 stand for no Department

        public string RoleId { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordConfirmed { get; set; }

        public string CreatedUserName { get; set; }

        //----------------------------------------
        // 針對 DepartmentPointManager 畫面所增加的欄位
        //----------------------------------------

        /// <summary>
        /// 是否可執行 播點動作
        /// </summary>
        public bool CanAllotPoint { get; set; }
        /// <summary>
        /// 原本重新載入資料時，無法重置checkbox，直到將這個屬性指定給ng-model=row.entity.checkbox
        /// </summary>
        public bool Checked { get; set; }

        public string AllotSettingDesc { get; set; }

        //----------------------------------------
        // 針對 分享群組聯絡人 畫面所增加的欄位
        //----------------------------------------

        /// <summary>
        /// 分享群組
        /// </summary>
        public int? SharedGroupId { get; set; }

	}
}
