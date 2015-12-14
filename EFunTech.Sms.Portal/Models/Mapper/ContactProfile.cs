using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class ContactProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Contact, ContactModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.HomePhoneArea, opt => opt.MapFrom(src => src.HomePhoneArea))
                .ForMember(dst => dst.HomePhone, opt => opt.MapFrom(src => src.HomePhone))
                .ForMember(dst => dst.CompanyPhoneArea, opt => opt.MapFrom(src => src.CompanyPhoneArea))
                .ForMember(dst => dst.CompanyPhone, opt => opt.MapFrom(src => src.CompanyPhone))
                .ForMember(dst => dst.CompanyPhoneExt, opt => opt.MapFrom(src => src.CompanyPhoneExt))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.Msn, opt => opt.MapFrom(src => src.Msn))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dst => dst.ImportantDay, opt => opt.MapFrom(src => src.ImportantDay))
                .ForMember(dst => dst.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dst => dst.Groups, opt => opt.MapFrom(src => src.Groups));

			CreateMap<ContactModel, Contact>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.HomePhoneArea, opt => opt.MapFrom(src => src.HomePhoneArea))
                .ForMember(dst => dst.HomePhone, opt => opt.MapFrom(src => src.HomePhone))
                .ForMember(dst => dst.CompanyPhoneArea, opt => opt.MapFrom(src => src.CompanyPhoneArea))
                .ForMember(dst => dst.CompanyPhone, opt => opt.MapFrom(src => src.CompanyPhone))
                .ForMember(dst => dst.CompanyPhoneExt, opt => opt.MapFrom(src => src.CompanyPhoneExt))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.Msn, opt => opt.MapFrom(src => src.Msn))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dst => dst.ImportantDay, opt => opt.MapFrom(src => src.ImportantDay))
                .ForMember(dst => dst.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dst => dst.Groups, opt => opt.MapFrom(src => src.Groups));
		}
	}
}
