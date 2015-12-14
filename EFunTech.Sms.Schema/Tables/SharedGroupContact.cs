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
    [TableDescription("分享群組至聯絡人")]
    public class SharedGroupContact
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Group")]
        [ColumnDescription("聯絡人群組編號")]
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("ShareToUser")]
        [ColumnDescription("分享到使用者編號")]
        public string ShareToUserId { get; set; }

        public virtual ApplicationUser ShareToUser { get; set; }
    }
}
