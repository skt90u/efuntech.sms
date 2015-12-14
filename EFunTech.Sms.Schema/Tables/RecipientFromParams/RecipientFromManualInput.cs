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
    [TableDescription("收訊人來源(手動輸入)")]
    public class RecipientFromManualInput
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [Required]
        [ColumnDescription("手機號碼(以逗號隔開)")]
        public string PhoneNumbers { get; set; }
    }
}
