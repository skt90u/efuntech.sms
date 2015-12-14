using AutoMapper;
using EFunTech.Sms.Schema;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class UploadedFileProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<UploadedFile, UploadedFileModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dst => dst.FilePath, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dst => dst.UploadedFileType, opt => opt.MapFrom(src => src.UploadedFileType))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));

			CreateMap<UploadedFileModel, UploadedFile>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dst => dst.FilePath, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dst => dst.UploadedFileType, opt => opt.MapFrom(src => src.UploadedFileType))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
		}
	}
}
