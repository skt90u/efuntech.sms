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
    [TableDescription("選單")]
    public class MenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        public int Level { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public string MapRouteUrl { get; set; }

        public int Order { get; set; }

        //public string Href { get; set; }

        [Required]
        public virtual WebAuthorization WebAuthorization { get; set; }
    }
}
