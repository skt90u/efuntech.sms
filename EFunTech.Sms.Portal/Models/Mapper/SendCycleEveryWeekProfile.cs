using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SendCycleEveryWeekProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SendCycleEveryWeek, SendCycleEveryWeekModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.DayOfWeeks, opt => opt.MapFrom(src => src.DayOfWeeks))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                ;

			CreateMap<SendCycleEveryWeekModel, SendCycleEveryWeek>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.DayOfWeeks, opt => opt.MapFrom(src => src.DayOfWeeks))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                ;
		}
	}
}
