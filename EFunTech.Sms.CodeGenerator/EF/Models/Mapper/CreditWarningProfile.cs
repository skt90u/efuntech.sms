using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class CreditWarningProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<CreditWarning, CreditWarningModel>()
                .ForMember(dst => dst.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                .ForMember(dst => dst.BySmsMessage, opt => opt.MapFrom(src => src.BySmsMessage))
                .ForMember(dst => dst.ByEmail, opt => opt.MapFrom(src => src.ByEmail))
                .ForMember(dst => dst.SmsBalance, opt => opt.MapFrom(src => src.SmsBalance));

			CreateMap<CreditWarningModel, CreditWarning>()
                .ForMember(dst => dst.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                .ForMember(dst => dst.BySmsMessage, opt => opt.MapFrom(src => src.BySmsMessage))
                .ForMember(dst => dst.ByEmail, opt => opt.MapFrom(src => src.ByEmail))
                .ForMember(dst => dst.SmsBalance, opt => opt.MapFrom(src => src.SmsBalance));
		}
	}
}
