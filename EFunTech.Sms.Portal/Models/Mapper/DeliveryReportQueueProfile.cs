using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class DeliveryReportQueueProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<DeliveryReportQueue, DeliveryReportQueueModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SourceTableId, opt => opt.MapFrom(src => src.SourceTableId))
                .ForMember(dst => dst.SourceTable, opt => opt.MapFrom(src => src.SourceTable))
                .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dst => dst.ProviderName, opt => opt.MapFrom(src => src.ProviderName))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.SendMessageResultItemCount, opt => opt.MapFrom(src => src.SendMessageResultItemCount))
                .ForMember(dst => dst.DeliveryReportCount, opt => opt.MapFrom(src => src.DeliveryReportCount));

			CreateMap<DeliveryReportQueueModel, DeliveryReportQueue>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SourceTableId, opt => opt.MapFrom(src => src.SourceTableId))
                .ForMember(dst => dst.SourceTable, opt => opt.MapFrom(src => src.SourceTable))
                .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dst => dst.ProviderName, opt => opt.MapFrom(src => src.ProviderName))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.SendMessageResultItemCount, opt => opt.MapFrom(src => src.SendMessageResultItemCount))
                .ForMember(dst => dst.DeliveryReportCount, opt => opt.MapFrom(src => src.DeliveryReportCount));
		}
	}
}
