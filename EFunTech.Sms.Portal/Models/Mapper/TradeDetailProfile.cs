using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class TradeDetailProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<TradeDetail, TradeDetailModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.TradeTime, opt => opt.MapFrom(src => src.TradeTime))
                .ForMember(dst => dst.TradeType, opt => opt.MapFrom(src => src.TradeType))
                .ForMember(dst => dst.Point, opt => opt.MapFrom(src => src.Point))
                .ForMember(dst => dst.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dst => dst.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dst => dst.TargetId, opt => opt.MapFrom(src => src.TargetId));

			CreateMap<TradeDetailModel, TradeDetail>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.TradeTime, opt => opt.MapFrom(src => src.TradeTime))
                .ForMember(dst => dst.TradeType, opt => opt.MapFrom(src => src.TradeType))
                .ForMember(dst => dst.Point, opt => opt.MapFrom(src => src.Point))
                .ForMember(dst => dst.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dst => dst.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dst => dst.TargetId, opt => opt.MapFrom(src => src.TargetId));
		}
	}
}
