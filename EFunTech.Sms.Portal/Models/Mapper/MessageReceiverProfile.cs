using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class MessageReceiverProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<MessageReceiver, MessageReceiverModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.RowNo, opt => opt.MapFrom(src => src.RowNo))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.RecipientFromType, opt => opt.MapFrom(src => src.RecipientFromType))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.UpdatedTime, opt => opt.MapFrom(src => src.UpdatedTime))
                .ForMember(dst => dst.MessageLength, opt => opt.MapFrom(src => src.MessageLength))
                .ForMember(dst => dst.MessageNum, opt => opt.MapFrom(src => src.MessageNum))
                .ForMember(dst => dst.MessageCost, opt => opt.MapFrom(src => src.MessageCost))
                .ForMember(dst => dst.MessageFormatError, opt => opt.MapFrom(src => src.MessageFormatError));

			CreateMap<MessageReceiverModel, MessageReceiver>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.RowNo, opt => opt.MapFrom(src => src.RowNo))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.SendTitle, opt => opt.MapFrom(src => src.SendTitle))
                .ForMember(dst => dst.SendBody, opt => opt.MapFrom(src => src.SendBody))
                .ForMember(dst => dst.SendMessageType, opt => opt.MapFrom(src => src.SendMessageType))
                .ForMember(dst => dst.RecipientFromType, opt => opt.MapFrom(src => src.RecipientFromType))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.UpdatedTime, opt => opt.MapFrom(src => src.UpdatedTime))
                .ForMember(dst => dst.MessageLength, opt => opt.MapFrom(src => src.MessageLength))
                .ForMember(dst => dst.MessageNum, opt => opt.MapFrom(src => src.MessageNum))
                .ForMember(dst => dst.MessageCost, opt => opt.MapFrom(src => src.MessageCost))
                .ForMember(dst => dst.MessageFormatError, opt => opt.MapFrom(src => src.MessageFormatError));
		}
	}
}
