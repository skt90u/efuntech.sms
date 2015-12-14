using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("上傳檔案類型")]
    public enum UploadedFileType
    {
        [ColumnDescription("簡訊發送")]
        SendMessage,

        [ColumnDescription("參數簡訊發送")]
        SendParamMessage,

        [ColumnDescription("聯絡人")]
        Contact,

        [ColumnDescription("黑名單")]
        Blacklist
    }
}
