using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class RecipientFromManualInputProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<RecipientFromManualInput, RecipientFromManualInputModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers));

			CreateMap<RecipientFromManualInputModel, RecipientFromManualInput>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.PhoneNumbers, opt => opt.MapFrom(src => src.PhoneNumbers));
		}
	}
}
