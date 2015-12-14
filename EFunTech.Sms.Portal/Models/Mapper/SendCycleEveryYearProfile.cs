using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SendCycleEveryYearProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SendCycleEveryYear, SendCycleEveryYearModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                ;

			CreateMap<SendCycleEveryYearModel, SendCycleEveryYear>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                ;
		}
	}
}
