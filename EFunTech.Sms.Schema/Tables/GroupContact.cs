using EFunTech.Sms.Schema;
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
    [TableDescription("聯絡人群組與聯絡人對應表")]
    public class GroupContact
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Group")]
        [ColumnDescription("聯絡人群組編號")]
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Contact")]
        [ColumnDescription("聯絡人編號")]
        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }
    }
}
