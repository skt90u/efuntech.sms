using AutoMapper;
using EFunTech.Sms.Core;
using EFunTech.Sms.Schema;
using System;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SystemAnnouncementProfile : Profile
	{
		protected override void Configure()
		{
            TimeSpan timezoneOffset = new TimeSpan(8, 0, 0); // Taiwan

			CreateMap<SystemAnnouncement, SystemAnnouncementModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsVisible, opt => opt.MapFrom(src => src.IsVisible))
                .ForMember(dst => dst.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dst => dst.Announcement, opt => opt.MapFrom(src => src.Announcement))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.PublishDateString, opt => opt.MapFrom(src => Converter.ToLocalTimeString(src.PublishDate, timezoneOffset, "yyyy-MM-dd")))
                ;
            
			CreateMap<SystemAnnouncementModel, SystemAnnouncement>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.IsVisible, opt => opt.MapFrom(src => src.IsVisible))
                .ForMember(dst => dst.PublishDate, opt => opt.MapFrom(src => src.PublishDate))
                .ForMember(dst => dst.Announcement, opt => opt.MapFrom(src => src.Announcement))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
		}
	}
}
