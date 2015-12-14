using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    /// <summary>
    /// 使用 flag 方式， http://www.dotblogs.com.tw/atowngit/archive/2009/11/19/12051.aspx?fid=70079
    /// </summary>
    ///[Flags]
    /// 20151101 Norman, 
    /// 將 LogLevel 改成使用 Flag 之後，Hangfire 仍然使用舊的方式(Debug = 0, Info = 1, Warn = 2, Error = 3)
    /// 找不出發現原因，目前沒有解決辦法，只好改回來
    [TableDescription("LogLevel")]
    public enum LogLevel
    {
        //[ColumnDescription("Debug")]
        //Debug = 1,
        //[ColumnDescription("Info")]
        //Info = 2,
        //[ColumnDescription("Warn")]
        //Warn = 4,
        //[ColumnDescription("Error")]
        //Error = 8,

        //[ColumnDescription("All")]
        //All = Debug | Info | Warn | Error,

        [ColumnDescription("Debug")]
        Debug = 0,
        [ColumnDescription("Info")]
        Info = 1,
        [ColumnDescription("Warn")]
        Warn = 2,
        [ColumnDescription("Error")]
        Error = 3,

        [ColumnDescription("All")]
        All = 4,
    }
}
