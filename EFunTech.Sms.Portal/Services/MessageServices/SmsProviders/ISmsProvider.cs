using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Portal
{
    public interface ISmsProvider
    {
        string Name { get; }

        bool IsAvailable { get; }

        decimal Balance { get; }

        decimal ToProviderBalance(decimal efuntechBalance);

        void SendSMS(int sendMessageQueueId);
        void RetrySMS(int sendMessageHistoryId);
        
        /// <param name="requestId">
        /// The request identifier
        ///     Infobip: clientCorrelator
        ///     Every8d: batchId
        /// </param>
        void GetDeliveryReport(string requestId);
    }
}
