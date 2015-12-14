using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("周期簡訊發送時間類型")]
    public enum SendCycleType
    {
        [ColumnDescription("每天發送")]
        EveryDay = 0,

        [ColumnDescription("每周發送")]
        EveryWeek = 1,

        [ColumnDescription("每月發送")]
        EveryMonth = 2,

        [ColumnDescription("每年發送")]
        EveryYear = 3

    }
}
