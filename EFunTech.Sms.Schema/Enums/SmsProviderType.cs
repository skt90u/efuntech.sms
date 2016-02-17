using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    //[TableDescription("簡訊供應商類型，用以指定首要簡訊提供商")]
    [TableDescription("發送線路")]
    public enum SmsProviderType
    {
        [ColumnDescription("一般 Infobip")]
        InfobipNormalQuality = 0,

        [ColumnDescription("高品質 Infobip")]
        InfobipHighQuality = 1,

        [ColumnDescription("Every8d")]
        Every8d = 2,
    }
}
