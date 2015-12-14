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
    // https://msdn.microsoft.com/zh-tw/data/jj591583.aspx
    // http://stackoverflow.com/questions/10080601/how-to-add-description-to-columns-in-entity-framework-4-3-code-first-using-migra

    [TableDescription("部門")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [Required]
        [ColumnDescription("名稱")]
        public string Name { get; set; }

        [Required]
        [ColumnDescription("說明")]
        public string Description { get; set; }

        /*
        //[Required]
        //[ColumnDescription("建立者")]
        //[MaxLength(256), ForeignKey("CreatedUser")] <--- 會造成循環的ForeignKey
        //public string CreatedUserId { get; set; }

        //public virtual ApplicationUser CreatedUser { get; set; }

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(128)]
        public string CreatedUserId { get; set; }
        */

        [Required]
        public virtual ApplicationUser CreatedUser { get; set; }

        [InverseProperty("Department")] // Department is ApplicationUser.Department
        // 多對一關係必須加上 InverseProperty 屬性，否則會在ApplicationUser建立重複ForeignKey 
        // .ForeignKey("dbo.Departments", t => t.Department_Id)
        // .ForeignKey("dbo.Departments", t => t.Department_Id1)
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
