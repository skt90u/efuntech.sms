using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class BlacklistProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Blacklist, BlacklistModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                .ForMember(dst => dst.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dst => dst.UpdatedTime, opt => opt.MapFrom(src => src.UpdatedTime))
                ;

			CreateMap<BlacklistModel, Blacklist>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => src.Enabled))
                .ForMember(dst => dst.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dst => dst.UpdatedTime, opt => opt.MapFrom(src => src.UpdatedTime))
                .ForMember(dst => dst.UpdatedUserName, opt => opt.MapFrom(src => src.UpdatedUserName))
                ;
		}
	}
}
