using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
    public class ApplicationUserProfile1 : Profile
	{
		protected override void Configure()
		{
            CreateMap<ApplicationUser, ApplicationUserModel1>()
                //.ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dst => dst.FullName, opt => opt.MapFrom(src => src.Department == null ? "lalal" : "CCC"))
                //.ForMember(dst => dst.Gender, opt => opt.MapFrom(src => src.Gender))
                //.ForMember(dst => dst.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
                //.ForMember(dst => dst.ContactPhoneExt, opt => opt.MapFrom(src => src.ContactPhoneExt))
                //.ForMember(dst => dst.AddressCountry, opt => opt.MapFrom(src => src.AddressCountry))
                //.ForMember(dst => dst.AddressArea, opt => opt.MapFrom(src => src.AddressArea))
                //.ForMember(dst => dst.AddressZip, opt => opt.MapFrom(src => src.AddressZip))
                //.ForMember(dst => dst.AddressStreet, opt => opt.MapFrom(src => src.AddressStreet))
                //.ForMember(dst => dst.Department, opt => opt.MapFrom(src => src.Department))
                //.ForMember(dst => dst.EmployeeNo, opt => opt.MapFrom(src => src.EmployeeNo))
                //.ForMember(dst => dst.CreditWarning, opt => opt.MapFrom(src => src.CreditWarning))
                //.ForMember(dst => dst.ReplyCc, opt => opt.MapFrom(src => src.ReplyCc))
                //.ForMember(dst => dst.ForeignSmsEnabled, opt => opt.MapFrom(src => src.ForeignSmsEnabled))
                //.ForMember(dst => dst.SmsBalance, opt => opt.MapFrom(src => src.SmsBalance))
                //.ForMember(dst => dst.SmsBalanceExpireDate, opt => opt.MapFrom(src => src.SmsBalanceExpireDate))
                //.ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                //.ForMember(dst => dst.AllotSetting, opt => opt.MapFrom(src => src.AllotSetting))
                //.ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                //.ForMember(dst => dst.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                //.ForMember(dst => dst.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                //.ForMember(dst => dst.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                //.ForMember(dst => dst.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                //.ForMember(dst => dst.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
                //.ForMember(dst => dst.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                //.ForMember(dst => dst.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                //.ForMember(dst => dst.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                //.ForMember(dst => dst.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                //.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.UserName))
                ;

            CreateMap<ApplicationUserModel, ApplicationUser>()
                //.ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
                //.ForMember(dst => dst.FullName, opt => opt.MapFrom(src => src.FullName))
                //.ForMember(dst => dst.Gender, opt => opt.MapFrom(src => src.Gender))
                //.ForMember(dst => dst.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
                //.ForMember(dst => dst.ContactPhoneExt, opt => opt.MapFrom(src => src.ContactPhoneExt))
                //.ForMember(dst => dst.AddressCountry, opt => opt.MapFrom(src => src.AddressCountry))
                //.ForMember(dst => dst.AddressArea, opt => opt.MapFrom(src => src.AddressArea))
                //.ForMember(dst => dst.AddressZip, opt => opt.MapFrom(src => src.AddressZip))
                //.ForMember(dst => dst.AddressStreet, opt => opt.MapFrom(src => src.AddressStreet))
                //.ForMember(dst => dst.Department, opt => opt.MapFrom(src => src.Department))
                //.ForMember(dst => dst.EmployeeNo, opt => opt.MapFrom(src => src.EmployeeNo))
                //.ForMember(dst => dst.CreditWarning, opt => opt.MapFrom(src => src.CreditWarning))
                //.ForMember(dst => dst.ReplyCc, opt => opt.MapFrom(src => src.ReplyCc))
                //.ForMember(dst => dst.ForeignSmsEnabled, opt => opt.MapFrom(src => src.ForeignSmsEnabled))
                //.ForMember(dst => dst.SmsBalance, opt => opt.MapFrom(src => src.SmsBalance))
                //.ForMember(dst => dst.SmsBalanceExpireDate, opt => opt.MapFrom(src => src.SmsBalanceExpireDate))
                //.ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                //.ForMember(dst => dst.AllotSetting, opt => opt.MapFrom(src => src.AllotSetting))
                //.ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                //.ForMember(dst => dst.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                //.ForMember(dst => dst.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                //.ForMember(dst => dst.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                //.ForMember(dst => dst.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                //.ForMember(dst => dst.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
                //.ForMember(dst => dst.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                //.ForMember(dst => dst.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                //.ForMember(dst => dst.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                //.ForMember(dst => dst.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                //.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.UserName))
                ;
		}
	}
}
