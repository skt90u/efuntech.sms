using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊接收者類型")]
    public enum RecipientFromType
    {
        [ColumnDescription("載入大量名單")]
        FileUpload = 0,

        [ColumnDescription("常用聯絡人")]
        CommonContact = 1,

        [ColumnDescription("由聯絡人(群組)選取")]
        GroupContact = 2,

        [ColumnDescription("手動輸入")]
        ManualInput = 3
    }
}
