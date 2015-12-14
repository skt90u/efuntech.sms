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
    [TableDescription("LogItem")]
    public class LogItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [ColumnDescription("主要執行的專案名稱")]
        public string EntryAssembly { get; set; }

        [ColumnDescription("類別")]
        public string Class { get; set; }

        [ColumnDescription("函式")]
        public string Method { get; set; }

        [ColumnDescription("層級")]
        public LogLevel LogLevel { get; set; }

        [ColumnDescription("訊息內容")]
        public string Message { get; set; }

        [ColumnDescription("呼叫堆疊")]
        public string StackTrace { get; set; }

        [ColumnDescription("建立時間")]
        [DateTimeKind(DateTimeKind.Utc)]
        [Index] // 20151128 Norman, 加上Index看看速度會不會變快
        public DateTime CreatedTime { get; set; }

        [MaxLength(256)]
        [Index("IX_CreatedUserName")]
        [ColumnDescription("建立者")] 
        public string UserName { get; set; }

        public string Host { get; set; }

        public string FileName { get; set; }
        public int FileLineNumber { get; set; }
    }
}
