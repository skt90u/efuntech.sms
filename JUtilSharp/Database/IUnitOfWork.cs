using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace JUtilSharp.Database
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext DbContext { get; }

        IRepository<T> Repository<T>() where T : class;

        TransactionScope CreateTransactionScope();
    }
}
