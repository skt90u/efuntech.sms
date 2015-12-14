using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("搜尋類型")]
    public enum SearchType
    {
        [ColumnDescription("依部門搜尋")]
        Department = 1,
        [ColumnDescription("依成員搜尋")]
        Member = 2,
    }
}
