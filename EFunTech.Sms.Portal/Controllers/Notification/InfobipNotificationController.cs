using EFunTech.Sms.Portal.Models;
using EFunTech.Sms.Schema;
using System.Linq;
using EFunTech.Sms.Portal.Controllers.Common;
using EFunTech.Sms.Portal.Models.Common;
using JUtilSharp.Database;

using System.Collections.Generic;
using LinqKit;
using EFunTech.Sms.Portal.Models.Criteria;
using System;
using System.Data.Entity.Core.Objects;
using System.Web.Security;
using System.Web.Http;

namespace EFunTech.Sms.Portal.Controllers
{
    public class InfobipNotificationController : ApiControllerBase
    {
        public InfobipNotificationController(IUnitOfWork unitOfWork, ILogService logService) : base(unitOfWork, logService) { }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            // DeliveryInfoNotification ConvertJsonToDeliveryInfoNotification(string json);
            // DeliveryReportList GetDeliveryReports();
            // DeliveryInfoList QueryDeliveryStatus(string senderAddress, string requestId);

            this.logService.Debug("InfobipNotificationController");
            this.logService.Debug("Post");
            this.logService.Debug(value);
/*
    public class ConvertJsonToDeliveryInfoNotification
    {
        // Pushed 'Delivery Info Notification' JSON example
        private const string JSON = "{\"deliveryInfoNotification\":{\"deliveryInfo\":{\"address\":\"38454234234\",\"deliveryStatus\":\"DeliveredToTerminal\"},\"callbackData\":\"\"}}";

        public static void Execute()
        {
            Configuration configuration = new Configuration();
            SMSClient smsClient = new SMSClient(configuration);

            // example:on-delivery-notification
            DeliveryInfoNotification deliveryInfoNotification = smsClient.SmsMessagingClient.ConvertJsonToDeliveryInfoNotification(JSON);
            // ----------------------------------------------------------------------------------------------------
            Console.WriteLine(deliveryInfoNotification);
        }
    }
*/
        }
    }
}