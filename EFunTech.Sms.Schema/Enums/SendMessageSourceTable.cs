using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("發送簡訊的參數依據來自哪一張表")]
    public enum SendMessageSourceTable
    {
        SendMessageQueue = 0,
        SendMessageRetryQueue = 1,
    }
}
