using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.Comparision
{
    public class SharedGroupContactCompare : IEqualityComparer<SharedGroupContact>
    {
        #region IEqualityComparer<SharedGroupContact> Members

        public bool Equals(SharedGroupContact x, SharedGroupContact y)
        {
            return x.GroupId.Equals(y.GroupId) &&
                   x.ShareToUserId.Equals(y.ShareToUserId);
        }

        public int GetHashCode(SharedGroupContact x)
        {
            return string.Format("{0}_{1}", x.GroupId, x.ShareToUserId).GetHashCode();
        }

        #endregion
    }
}
