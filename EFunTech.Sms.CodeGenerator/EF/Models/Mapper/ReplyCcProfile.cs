using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class ReplyCcProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<ReplyCc, ReplyCcModel>()
                .ForMember(dst => dst.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                .ForMember(dst => dst.BySmsMessage, opt => opt.MapFrom(src => src.BySmsMessage))
                .ForMember(dst => dst.ByEmail, opt => opt.MapFrom(src => src.ByEmail));

			CreateMap<ReplyCcModel, ReplyCc>()
                .ForMember(dst => dst.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                .ForMember(dst => dst.BySmsMessage, opt => opt.MapFrom(src => src.BySmsMessage))
                .ForMember(dst => dst.ByEmail, opt => opt.MapFrom(src => src.ByEmail));
		}
	}
}
