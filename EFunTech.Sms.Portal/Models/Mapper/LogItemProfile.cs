using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class LogItemProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<LogItem, LogItemModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.EntryAssembly, opt => opt.MapFrom(src => src.EntryAssembly))
                .ForMember(dst => dst.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(dst => dst.Method, opt => opt.MapFrom(src => src.Method))
                .ForMember(dst => dst.LogLevel, opt => opt.MapFrom(src => src.LogLevel))
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dst => dst.StackTrace, opt => opt.MapFrom(src => src.StackTrace))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dst => dst.Host, opt => opt.MapFrom(src => src.Host))
                .ForMember(dst => dst.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dst => dst.FileLineNumber, opt => opt.MapFrom(src => src.FileLineNumber))
                ;

			CreateMap<LogItemModel, LogItem>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.EntryAssembly, opt => opt.MapFrom(src => src.EntryAssembly))
                .ForMember(dst => dst.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(dst => dst.Method, opt => opt.MapFrom(src => src.Method))
                .ForMember(dst => dst.LogLevel, opt => opt.MapFrom(src => src.LogLevel))
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dst => dst.StackTrace, opt => opt.MapFrom(src => src.StackTrace))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dst => dst.Host, opt => opt.MapFrom(src => src.Host))
                .ForMember(dst => dst.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dst => dst.FileLineNumber, opt => opt.MapFrom(src => src.FileLineNumber))
                ;
		}
	}
}
