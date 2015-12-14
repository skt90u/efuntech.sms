using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class WebAuthorizationProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<WebAuthorization, WebAuthorizationModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.ControllerName, opt => opt.MapFrom(src => src.ControllerName))
                .ForMember(dst => dst.ActionName, opt => opt.MapFrom(src => src.ActionName))
                .ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dst => dst.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dst => dst.Remark, opt => opt.MapFrom(src => src.Remark));

			CreateMap<WebAuthorizationModel, WebAuthorization>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.ControllerName, opt => opt.MapFrom(src => src.ControllerName))
                .ForMember(dst => dst.ActionName, opt => opt.MapFrom(src => src.ActionName))
                .ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dst => dst.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dst => dst.Remark, opt => opt.MapFrom(src => src.Remark));
		}
	}
}
