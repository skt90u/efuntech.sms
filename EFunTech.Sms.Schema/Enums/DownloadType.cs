using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("下載類型")]
    public enum DownloadType
    {
        [ColumnDescription("未知")]
        Unknown = 0,
        [ColumnDescription("統計表")]
        Statistic = 1,
        [ColumnDescription("全部")]
        All = 2,
    }
}
