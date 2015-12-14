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
    [TableDescription("收訊人來源(檔案上傳)")]
    public class RecipientFromFileUpload
    {
        [Key]
        [ColumnDescription("簡訊發送規則")]
        [ForeignKey("SendMessageRule")]
        public int SendMessageRuleId { get; set; }

        public virtual SendMessageRule SendMessageRule { get; set; }

        [ColumnDescription("上傳檔案")]
        [ForeignKey("UploadedFile")]
        public int UploadedFileId { get; set; }

        [Required]
        public virtual UploadedFile UploadedFile { get; set; }

        public bool AddSelfToMessageReceiverList { get; set; }

    }
}
