using AutoMapper;
using EFunTech.Sms.Schema;
using JUtilSharp.Database;
using System.Collections.Generic;
using System.Linq;

namespace EFunTech.Sms.Portal.Models.Mapper
{
	public class SendMessageHistoryProfile : Profile
	{
		protected override void Configure()
		{
			CreateMap<SendMessageHistory, SendMessageHistoryModel>()
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
                .ForMember(dst => dst.MessageId, opt => opt.MapFrom(src => src.MessageId))
                .ForMember(dst => dst.MessageStatus, opt => opt.MapFrom(src => src.MessageStatus))
                .ForMember(dst => dst.MessageStatusString, opt => opt.MapFrom(src => src.MessageStatusString))
                .ForMember(dst => dst.SenderAddress, opt => opt.MapFrom(src => src.SenderAddress))
                .ForMember(dst => dst.DestinationAddress, opt => opt.MapFrom(src => src.DestinationAddress))
                .ForMember(dst => dst.SendMessageResultCreatedTime, opt => opt.MapFrom(src => src.SendMessageResultCreatedTime))
                .ForMember(dst => dst.SentDate, opt => opt.MapFrom(src => src.SentDate))
                .ForMember(dst => dst.DoneDate, opt => opt.MapFrom(src => src.DoneDate))
                .ForMember(dst => dst.DeliveryStatus, opt => opt.MapFrom(src => src.DeliveryStatus))
                .ForMember(dst => dst.DeliveryStatusString, opt => opt.MapFrom(src => src.DeliveryStatusString))
                .ForMember(dst => dst.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dst => dst.DeliveryReportCreatedTime, opt => opt.MapFrom(src => src.DeliveryReportCreatedTime))
                .ForMember(dst => dst.MessageCost, opt => opt.MapFrom(src => src.MessageCost))
                .ForMember(dst => dst.Delivered, opt => opt.MapFrom(src => src.Delivered))
                .ForMember(dst => dst.DestinationName, opt => opt.MapFrom(src => src.DestinationName))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.RetryMaxTimes, opt => opt.MapFrom(src => src.RetryMaxTimes))
                .ForMember(dst => dst.RetryMaxTimes, opt => opt.MapFrom(src => src.RetryMaxTimes))
                ;

			CreateMap<SendMessageHistoryModel, SendMessageHistory>()
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
                .ForMember(dst => dst.MessageId, opt => opt.MapFrom(src => src.MessageId))
                .ForMember(dst => dst.MessageStatus, opt => opt.MapFrom(src => src.MessageStatus))
                .ForMember(dst => dst.MessageStatusString, opt => opt.MapFrom(src => src.MessageStatusString))
                .ForMember(dst => dst.SenderAddress, opt => opt.MapFrom(src => src.SenderAddress))
                .ForMember(dst => dst.DestinationAddress, opt => opt.MapFrom(src => src.DestinationAddress))
                .ForMember(dst => dst.SendMessageResultCreatedTime, opt => opt.MapFrom(src => src.SendMessageResultCreatedTime))
                .ForMember(dst => dst.SentDate, opt => opt.MapFrom(src => src.SentDate))
                .ForMember(dst => dst.DoneDate, opt => opt.MapFrom(src => src.DoneDate))
                .ForMember(dst => dst.DeliveryStatus, opt => opt.MapFrom(src => src.DeliveryStatus))
                .ForMember(dst => dst.DeliveryStatusString, opt => opt.MapFrom(src => src.DeliveryStatusString))
                .ForMember(dst => dst.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dst => dst.DeliveryReportCreatedTime, opt => opt.MapFrom(src => src.DeliveryReportCreatedTime))
                .ForMember(dst => dst.MessageCost, opt => opt.MapFrom(src => src.MessageCost))
                .ForMember(dst => dst.Delivered, opt => opt.MapFrom(src => src.Delivered))
                .ForMember(dst => dst.DestinationName, opt => opt.MapFrom(src => src.DestinationName))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region))
                .ForMember(dst => dst.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dst => dst.RetryMaxTimes, opt => opt.MapFrom(src => src.RetryMaxTimes))
                .ForMember(dst => dst.RetryMaxTimes, opt => opt.MapFrom(src => src.RetryMaxTimes))
                ;
		}

        public static IEnumerable<SendMessageHistoryModel> ConvertModel(IEnumerable<SendMessageHistoryModel> models, IUnitOfWork unitOfWork)
        {
            //var deaprtmentRepository = unitOfWork.Repository<Department>();
            //var userRepository = unitOfWork.Repository<ApplicationUser>();

            //int rowNo = 0;

            //foreach (var model in models)
            //{
            //    model.RowNo = ++rowNo;

            //    if (model.DepartmentId.HasValue)
            //    {
            //        model.DepartmentName = deaprtmentRepository.GetById(model.DepartmentId.Value).Name;
            //    }

            //    var user = userRepository.GetById(model.CreatedUserId);
            //    model.UserName = user.UserName;
            //    model.FullName = user.FullName;

            //    model.DeliveryStatusChineseString = model.DeliveryStatusString; // TODO: DeliveryStatus 中文說明
            //}

            //return models;

            var departments = unitOfWork.Repository<Department>().GetAll().ToList();
            var users = unitOfWork.Repository<ApplicationUser>().GetAll().ToList();

            foreach (var model in models)
            {
                if (model.DepartmentId.HasValue)
                {
                    model.DepartmentName = departments.Find(p => p.Id == model.DepartmentId).Name;
                }

                model.UserName = users.Find(p => p.Id == model.CreatedUserId).UserName;
                model.FullName = users.Find(p => p.Id == model.CreatedUserId).FullName;

                model.DeliveryStatusChineseString = model.DeliveryStatusString; // TODO: DeliveryStatus 中文說明
            }

            return models;
        }
	}
}
