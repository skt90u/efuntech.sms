using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("單向|雙向 簡訊發送")]
    public enum SendCustType
    {
        [ColumnDescription("單向")]
        OneWay = 0,

        [ColumnDescription("雙向")]
        TwoWay = 1
    }
}
