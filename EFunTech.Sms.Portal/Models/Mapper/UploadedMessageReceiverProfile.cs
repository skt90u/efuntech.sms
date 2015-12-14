using AutoMapper;

using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Core;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class UploadedMessageReceiverProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<UploadedMessageReceiver, UploadedMessageReceiverModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.RowNo, opt => opt.MapFrom(src => src.RowNo))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))

                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => src.SendTime))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                .ForMember(dst => dst.SendTimeString, opt => opt.MapFrom(src => src.SendTimeString))
                .ForMember(dst => dst.UseParam, opt => opt.MapFrom(src => src.UseParam))

                .ForMember(dst => dst.Param1, opt => opt.MapFrom(src => src.Param1))
                .ForMember(dst => dst.Param2, opt => opt.MapFrom(src => src.Param2))
                .ForMember(dst => dst.Param3, opt => opt.MapFrom(src => src.Param3))
                .ForMember(dst => dst.Param4, opt => opt.MapFrom(src => src.Param4))
                .ForMember(dst => dst.Param5, opt => opt.MapFrom(src => src.Param5))
                .ForMember(dst => dst.IsValid, opt => opt.MapFrom(src => src.IsValid))
                .ForMember(dst => dst.InvalidReason, opt => opt.MapFrom(src => src.InvalidReason))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.UploadedFile, opt => opt.MapFrom(src => src.UploadedFile))
                ;

			CreateMap<UploadedMessageReceiverModel, UploadedMessageReceiver>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.RowNo, opt => opt.MapFrom(src => src.RowNo))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Mobile, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dst => dst.E164Mobile, opt => opt.MapFrom(src => src.E164Mobile))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))

                .ForMember(dst => dst.SendTime, opt => opt.MapFrom(src => Converter.ToUniversalTime(src.SendTimeString, Converter.yyyyMMddHHmm, src.ClientTimezoneOffset)))
                .ForMember(dst => dst.ClientTimezoneOffset, opt => opt.MapFrom(src => src.ClientTimezoneOffset))
                .ForMember(dst => dst.SendTimeString, opt => opt.MapFrom(src => src.SendTimeString))
                .ForMember(dst => dst.UseParam, opt => opt.MapFrom(src => src.UseParam))

                .ForMember(dst => dst.Param1, opt => opt.MapFrom(src => src.Param1))
                .ForMember(dst => dst.Param2, opt => opt.MapFrom(src => src.Param2))
                .ForMember(dst => dst.Param3, opt => opt.MapFrom(src => src.Param3))
                .ForMember(dst => dst.Param4, opt => opt.MapFrom(src => src.Param4))
                .ForMember(dst => dst.Param5, opt => opt.MapFrom(src => src.Param5))
                .ForMember(dst => dst.IsValid, opt => opt.MapFrom(src => src.IsValid))
                .ForMember(dst => dst.InvalidReason, opt => opt.MapFrom(src => src.InvalidReason))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.UploadedFile, opt => opt.MapFrom(src => src.UploadedFile))
                ;
		}
	}
}
