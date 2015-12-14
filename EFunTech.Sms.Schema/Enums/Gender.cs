using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("性別")]
    public enum Gender
    {
        [ColumnDescription("不詳")]
        Unknown = 0,

        [ColumnDescription("男性")]
        Male = 1,

        [ColumnDescription("女性")]
        Female = 2, 
    }
}
