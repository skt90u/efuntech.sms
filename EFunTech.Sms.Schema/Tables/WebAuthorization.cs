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
    /// <summary>
    /// 指定需要特殊權限的Action
    /// </summary>
    [TableDescription("WebAuthorization")]
    public class WebAuthorization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("ControllerName")]
        public string ControllerName { get; set; }

        [ColumnDescription("ActionName")]
        public string ActionName { get; set; }

        [ColumnDescription("Roles(以逗號隔開，e.g. Role1,Role2)")]
        public string Roles { get; set; }

        [ColumnDescription("Users(以逗號隔開，e.g. User1,User2)")]
        public string Users { get; set; }
        
        public string Remark { get; set; }
    }
}
