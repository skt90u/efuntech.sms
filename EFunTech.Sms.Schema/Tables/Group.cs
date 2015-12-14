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
    [TableDescription("聯絡人群組")]
    public class Group
    {
        public const string CommonContactGroupName = "常用聯絡人";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [Required]
        [ColumnDescription("建立者")]
        [MaxLength(256), ForeignKey("CreatedUser")]
        [Index("IX_CreatedUserIdAndGroupName", 1, IsUnique = true)]
        public string CreatedUserId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }

        [Required]
        [MaxLength(100)]
        [Index("IX_CreatedUserIdAndGroupName", 2, IsUnique = true)]
        [ColumnDescription("群組名稱")]
        public string Name { get; set; }

        [ColumnDescription("群組說明")]
        public string Description { get; set; }

        [ColumnDescription("是否可刪除")]
        public bool Deletable { get; set; }

        public virtual ICollection<GroupContact> GroupContacts { get; set; }
    }
}
