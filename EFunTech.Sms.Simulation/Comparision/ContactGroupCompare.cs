using EFunTech.Sms.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Simulation.Comparision
{
    public class ContactGroupCompare : IEqualityComparer<Group>
    {
        #region IEqualityComparer<Person> Members

        public bool Equals(Group x, Group y)
        {
            return x.Name.Equals(y.Name) &&
                   x.CreatedUserId.Equals(y.CreatedUserId);
        }

        public int GetHashCode(Group x)
        {
            return string.Format("{0}_{1}", x.Name, x.CreatedUserId).GetHashCode();
        }

        #endregion
    }
}
