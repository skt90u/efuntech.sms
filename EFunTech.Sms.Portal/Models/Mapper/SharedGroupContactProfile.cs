using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SharedGroupContactProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SharedGroupContact, SharedGroupContactModel>()
                .ForMember(dst => dst.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dst => dst.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dst => dst.ShareToUserId, opt => opt.MapFrom(src => src.ShareToUserId));

			CreateMap<SharedGroupContactModel, SharedGroupContact>()
                .ForMember(dst => dst.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dst => dst.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dst => dst.ShareToUserId, opt => opt.MapFrom(src => src.ShareToUserId));
		}
	}
}
