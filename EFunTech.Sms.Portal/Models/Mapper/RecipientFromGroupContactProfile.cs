using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class RecipientFromGroupContactProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<RecipientFromGroupContact, RecipientFromGroupContactModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.ContactIds, opt => opt.MapFrom(src => src.ContactIds));

			CreateMap<RecipientFromGroupContactModel, RecipientFromGroupContact>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.ContactIds, opt => opt.MapFrom(src => src.ContactIds));
		}
	}
}
