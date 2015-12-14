using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class RecipientFromFileUploadProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<RecipientFromFileUpload, RecipientFromFileUploadModel>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.UploadedFileId, opt => opt.MapFrom(src => src.UploadedFileId))
                .ForMember(dst => dst.AddSelfToMessageReceiverList, opt => opt.MapFrom(src => src.AddSelfToMessageReceiverList));
            
			CreateMap<RecipientFromFileUploadModel, RecipientFromFileUpload>()
                .ForMember(dst => dst.SendMessageRuleId, opt => opt.MapFrom(src => src.SendMessageRuleId))
                .ForMember(dst => dst.UploadedFileId, opt => opt.MapFrom(src => src.UploadedFileId))
                .ForMember(dst => dst.AddSelfToMessageReceiverList, opt => opt.MapFrom(src => src.AddSelfToMessageReceiverList));
		}
	}
}
