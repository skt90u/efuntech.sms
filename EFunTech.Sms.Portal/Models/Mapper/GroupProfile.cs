using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class GroupProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<Group, GroupModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.CreatedUserId, opt => opt.MapFrom(src => src.CreatedUserId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.Deletable, opt => opt.MapFrom(src => src.Deletable));

			CreateMap<GroupModel, Group>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.CreatedUserId, opt => opt.MapFrom(src => src.CreatedUserId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.Deletable, opt => opt.MapFrom(src => src.Deletable));
		}
	}
}
