using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SendDeliverProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SendDeliver, SendDeliverModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                ;

			CreateMap<SendDeliverModel, SendDeliver>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                ;
		}
	}
}
