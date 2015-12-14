using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("EfJobQueueStatus")]
    public enum EfJobQueueStatus
    {
        [ColumnDescription("Enqueued")]
        Enqueued = 0,
        
    }
}
