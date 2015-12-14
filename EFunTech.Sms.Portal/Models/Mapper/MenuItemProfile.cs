using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class MenuItemProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<MenuItem, MenuItemModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.MapRouteUrl, opt => opt.MapFrom(src => src.MapRouteUrl))
                .ForMember(dst => dst.WebAuthorization, opt => opt.MapFrom(src => src.WebAuthorization));

			CreateMap<MenuItemModel, MenuItem>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dst => dst.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.MapRouteUrl, opt => opt.MapFrom(src => src.MapRouteUrl))
                .ForMember(dst => dst.WebAuthorization, opt => opt.MapFrom(src => src.WebAuthorization));
		}
	}
}
