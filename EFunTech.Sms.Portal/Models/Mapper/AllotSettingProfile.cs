using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class AllotSettingProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<AllotSetting, AllotSettingModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.MonthlyAllot, opt => opt.MapFrom(src => src.MonthlyAllot))
                .ForMember(dst => dst.MonthlyAllotDay, opt => opt.MapFrom(src => src.MonthlyAllotDay))
                .ForMember(dst => dst.MonthlyAllotPoint, opt => opt.MapFrom(src => src.MonthlyAllotPoint))
                .ForMember(dst => dst.LastAllotTime, opt => opt.MapFrom(src => src.LastAllotTime))
                .ForMember(dst => dst.LimitMinPoint, opt => opt.MapFrom(src => src.LimitMinPoint))
                .ForMember(dst => dst.LimitMaxPoint, opt => opt.MapFrom(src => src.LimitMaxPoint));

			CreateMap<AllotSettingModel, AllotSetting>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.MonthlyAllot, opt => opt.MapFrom(src => src.MonthlyAllot))
                .ForMember(dst => dst.MonthlyAllotDay, opt => opt.MapFrom(src => src.MonthlyAllotDay))
                .ForMember(dst => dst.MonthlyAllotPoint, opt => opt.MapFrom(src => src.MonthlyAllotPoint))
                .ForMember(dst => dst.LastAllotTime, opt => opt.MapFrom(src => src.LastAllotTime))
                .ForMember(dst => dst.LimitMinPoint, opt => opt.MapFrom(src => src.LimitMinPoint))
                .ForMember(dst => dst.LimitMaxPoint, opt => opt.MapFrom(src => src.LimitMaxPoint));
		}
	}
}
