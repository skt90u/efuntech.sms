using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SendMessageQueueProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SendMessageQueue, SendMessageQueueModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.SendCustType, opt => opt.MapFrom(src => src.SendCustType))
                .ForMember(dst => dst.TotalReceiverCount, opt => opt.MapFrom(src => src.TotalReceiverCount))
                //.ForMember(dst => dst.TransmissionCount, opt => opt.MapFrom(src => src.TransmissionCount))
                //.ForMember(dst => dst.SuccessCount, opt => opt.MapFrom(src => src.SuccessCount))
                //.ForMember(dst => dst.FailureCount, opt => opt.MapFrom(src => src.FailureCount))
                .ForMember(dst => dst.TotalMessageCost, opt => opt.MapFrom(src => src.TotalMessageCost))
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId));

			CreateMap<SendMessageQueueModel, SendMessageQueue>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.SendCustType, opt => opt.MapFrom(src => src.SendCustType))
                .ForMember(dst => dst.TotalReceiverCount, opt => opt.MapFrom(src => src.TotalReceiverCount))
                //.ForMember(dst => dst.TransmissionCount, opt => opt.MapFrom(src => src.TransmissionCount))
                //.ForMember(dst => dst.SuccessCount, opt => opt.MapFrom(src => src.SuccessCount))
                //.ForMember(dst => dst.FailureCount, opt => opt.MapFrom(src => src.FailureCount))
                .ForMember(dst => dst.TotalMessageCost, opt => opt.MapFrom(src => src.TotalMessageCost))
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId));
		}
	}
}
