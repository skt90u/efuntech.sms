using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SignatureProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Signature, SignatureModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Subject, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dst => dst.UpdatedTime, opt => opt.MapFrom(src => src.UpdatedTime));

			CreateMap<SignatureModel, Signature>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Subject, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dst => dst.UpdatedTime, opt => opt.MapFrom(src => src.UpdatedTime));
		}
	}
}
