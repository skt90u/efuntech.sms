using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    // https://developer.infobip.com/api#!/SendSMS/GET_1_smsmessaging_outbound_requests_deliveryReports
    [TableDescription("簡訊派送結果")]
    public class Every8d_DeliveryReport
    {
        public string RequestId { get; set; }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime CreatedTime { get; set; }

        public int CODE { get; set; }

        public string DESCRIPTION { get; set; }

        public string NAME { get; set; }
        public string MOBILE { get; set; }

        [ColumnDescription("電信商實際發送時間")]
        public string SENT_TIME { get; set; }
        public string COST { get; set; }
        public string STATUS { get; set; }

        public virtual Every8d_SendMessageResult SendMessageResult { get; set; }
    }
}
