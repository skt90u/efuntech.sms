using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("發送簡訊狀態(通用型-用於寫入SendMessageHistory)")]
    public enum MessageStatus
    {
        [ColumnDescription("Unknown")]
        Unknown = 0,

        [ColumnDescription("MessageAccepted")]
        MessageAccepted = 1,

        [ColumnDescription("MessageNotSent")]
        MessageNotSent = 2,

        [ColumnDescription("MessageSent")]
        MessageSent = 3,

        [ColumnDescription("MessageWaitingForDelivery")]
        MessageWaitingForDelivery = 4,

        [ColumnDescription("MessageNotDelivered")]
        MessageNotDelivered = 5,

        [ColumnDescription("MessageDelivered")]
        MessageDelivered = 6,

        [ColumnDescription("NetworkNotAllowed")]
        NetworkNotAllowed = 7,

        [ColumnDescription("NetworkNotAvailable")]
        NetworkNotAvailable = 8,

        [ColumnDescription("InvalidDestinationAddress")]
        InvalidDestinationAddress = 9,

        [ColumnDescription("MessageDeliveryUnknown")]
        MessageDeliveryUnknown = 10,

        [ColumnDescription("RouteNotAvailable")]
        RouteNotAvailable = 11,

        [ColumnDescription("InvalidSourceAddress")]
        InvalidSourceAddress = 12,

        [ColumnDescription("NotEnoughCredits")]
        NotEnoughCredits = 13,

        [ColumnDescription("MessageRejected")]
        MessageRejected = 14,

        [ColumnDescription("MessageExpired")]
        MessageExpired = 15,

        [ColumnDescription("SystemError")]
        SystemError = 16,
    }
}
