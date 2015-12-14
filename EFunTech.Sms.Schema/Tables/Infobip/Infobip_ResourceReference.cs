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
    [TableDescription("reference to a resource created by the OneAPI server - in the form of a generated URL")]
    public class Infobip_ResourceReference
    {
        [Key]
        [ColumnDescription("簡訊發送回報")]
        [ForeignKey("SendMessageResult")]
        public int SendMessageResultId { get; set; }

        public virtual Infobip_SendMessageResult SendMessageResult { get; set; }

        [ColumnDescription("return a URL uniquely identifying a successful OneAPI server request")]
        public string ResourceURL { get; set; }
    }
}
