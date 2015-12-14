using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SendMessageStatisticProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SendMessageStatistic, SendMessageStatisticModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dst => dst.CreatedUserId, opt => opt.MapFrom(src => src.CreatedUserId))
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendMessageQueueId, opt => opt.MapFrom(src => src.SendMessageQueueId))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.SendCustType, opt => opt.MapFrom(src => src.SendCustType))
                .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dst => dst.ProviderName, opt => opt.MapFrom(src => src.ProviderName))
                .ForMember(dst => dst.TotalReceiverCount, opt => opt.MapFrom(src => src.TotalReceiverCount))
                .ForMember(dst => dst.TotalMessageCost, opt => opt.MapFrom(src => src.TotalMessageCost))
                .ForMember(dst => dst.TotalSuccess, opt => opt.MapFrom(src => src.TotalSuccess))
                .ForMember(dst => dst.TotalSending, opt => opt.MapFrom(src => src.TotalSending))
                .ForMember(dst => dst.TotalTimeout, opt => opt.MapFrom(src => src.TotalTimeout))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));

			CreateMap<SendMessageStatisticModel, SendMessageStatistic>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dst => dst.CreatedUserId, opt => opt.MapFrom(src => src.CreatedUserId))
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.SendMessageQueueId, opt => opt.MapFrom(src => src.SendMessageQueueId))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.SendCustType, opt => opt.MapFrom(src => src.SendCustType))
                .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dst => dst.ProviderName, opt => opt.MapFrom(src => src.ProviderName))
                .ForMember(dst => dst.TotalReceiverCount, opt => opt.MapFrom(src => src.TotalReceiverCount))
                .ForMember(dst => dst.TotalMessageCost, opt => opt.MapFrom(src => src.TotalMessageCost))
                .ForMember(dst => dst.TotalSuccess, opt => opt.MapFrom(src => src.TotalSuccess))
                .ForMember(dst => dst.TotalSending, opt => opt.MapFrom(src => src.TotalSending))
                .ForMember(dst => dst.TotalTimeout, opt => opt.MapFrom(src => src.TotalTimeout))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
		}
	}
}
