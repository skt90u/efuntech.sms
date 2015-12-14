using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class RecipientFromCommonContactProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<RecipientFromCommonContact, RecipientFromCommonContactModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.ContactIds, opt => opt.MapFrom(src => src.ContactIds));

			CreateMap<RecipientFromCommonContactModel, RecipientFromCommonContact>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.ContactIds, opt => opt.MapFrom(src => src.ContactIds));
		}
	}
}
