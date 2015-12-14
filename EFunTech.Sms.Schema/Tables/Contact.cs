using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("聯絡人")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [Required]
        [ColumnDescription("姓名")]
        public string Name { get; set; }

        [Required]
        [ColumnDescription("手機號碼")]
        public string Mobile { get; set; }

        // 不需要必填，因為支援檔案上傳名單，而上傳資料手機號碼可能錯誤
        [ColumnDescription("E164格式的「手機門號」為必填欄位。")]
        public string E164Mobile { get; set; }

        // 不需要必填，因為支援檔案上傳名單，而上傳資料手機號碼可能錯誤
        [ColumnDescription("發送地區")]
        public string Region { get; set; }

        [ColumnDescription("家用電話(區碼)")]
        public string HomePhoneArea { get; set; }

        [ColumnDescription("家用電話")]
        public string HomePhone { get; set; }

        [ColumnDescription("公司電話(區碼)")]
        public string CompanyPhoneArea { get; set; }

        [ColumnDescription("公司電話")]
        public string CompanyPhone { get; set; }

        [ColumnDescription("公司電話(分機號碼)")]
        public string CompanyPhoneExt { get; set; }

        [ColumnDescription("電子郵件")]
        public string Email { get; set; }

        [ColumnDescription("即時通帳號(Msn)")]
        public string Msn { get; set; }

        [ColumnDescription("聯絡人概述")]
        public string Description { get; set; }

        [ColumnDescription("生日")]
        public string Birthday { get; set; }

        [ColumnDescription("重要日子")]
        public string ImportantDay { get; set; }

        [ColumnDescription("性別")]
        public Gender Gender { get; set; }

        // 快取目前聯絡人對應群組
        // 用以避免再ContactProfile重複查詢相同資料
        // .ForMember(dst => dst.Groups, opt => opt.MapFrom(src => string.Join(",", src.GroupContacts.Select(p => p.Group.Name))))
        // 解決方式是在 CommonContactsController.cs 以及 AllContactsController.cs 複寫 CrudApiController.ConvertModel 對於Groups為空的資料進行查詢並回存，而不使用AutoMapper
        public string Groups { get; set; }

        // 原本設定方式 (手動指定ForeignKey)
        //[Required]
        //[ColumnDescription("建立者")]
        //[MaxLength(256), ForeignKey("CreatedUser")]
        //public string CreatedUserId { get; set; }
        //public virtual ApplicationUser CreatedUser { get; set; } 

        // 後來設定方式 (Add-Migration 自動建立 ForeignKey)
        [Required]
        public virtual ApplicationUser CreatedUser { get; set; } 

        public virtual ICollection<GroupContact> GroupContacts { get; set; }
    }
}
