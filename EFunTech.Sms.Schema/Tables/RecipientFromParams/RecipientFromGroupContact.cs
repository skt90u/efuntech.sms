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
    [TableDescription("收訊人來源(群組聯絡人)")]
    public class RecipientFromGroupContact
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [Required]
        [ColumnDescription("聯絡人編號(以逗號隔開)")]
        public string ContactIds { get; set; }
    }
}
