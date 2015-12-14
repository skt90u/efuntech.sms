using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊供應商類型，用以指定首要簡訊提供商")]
    public enum SmsProviderType
    {
        InfobipSmsProvider = 0,
        Every8dSmsProvider = 1,
    }
}
