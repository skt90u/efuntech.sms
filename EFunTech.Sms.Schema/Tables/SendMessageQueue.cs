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
    [TableDescription("簡訊發送任務")]
    public class SendMessageQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ColumnDescription("編號")]
        public int Id { get; set; }

        [Required]
        [ColumnDescription("發送訊息類型(手機簡訊|APP簡訊)")]
        public SendMessageType SendMessageType { get; set; }

        [Required]
        [ColumnDescription("發送時間(即是此筆資料建立時間)")]
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime SendTime { get; set; }

        [ColumnDescription("簡訊類別描述")]
        public string SendTitle { get; set; }

        [Required]
        [ColumnDescription("發送內容")]
        public string SendBody { get; set; }

        [Required]
        [ColumnDescription("單向|雙向 簡訊發送")]
        public SendCustType SendCustType { get; set; }

        [Required]
        [ColumnDescription("發送通數")]
        public int TotalReceiverCount { get; set; } // TODO: 這個名稱不好，會誤會成總接收通數，要改掉

        [Required]
        [DecimalPrecision(15, 2)] // 1000000000000.00
        [ColumnDescription("花費點數(發送所需點數)")]
        public decimal TotalMessageCost { get; set; }

        [Required]
        [ColumnDescription("簡訊發送規則")]
        //[ForeignKey("SendMessageRule")] // 20150930 Norman, 由於 SendMessageRule 可以被刪除，因此不再跟 SendMessageRule 綁在一起，否則連發送紀錄都必須刪除(ForeignKey)
        public int SendMessageRuleId { get; set; }
    }
}
