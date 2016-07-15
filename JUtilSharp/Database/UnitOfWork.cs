using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace JUtilSharp.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext context;
        
        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        private Hashtable repositories = new Hashtable();

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public IRepository<T> Repository<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Hashtable();
            }

            var type = typeof(T).Name;

            if (!repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(T)),
                    this.context);

                repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)repositories[type];
        }

        public TransactionScope CreateTransactionScope()
        {
            return new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                    
                    // 預設為 30 秒，設為 2 個小時
                    // http://www.expert.idv.tw/Content/Blog/Display/246e0816-8b6e-4264-bb27-6f5433649129
                    // http://charlesbc.blogspot.tw/2013/06/systemtransactions-timeouts-azure.html
                    Timeout = new TimeSpan(2, 0, 0) 
                }
            );
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (context != null)
                    {
                        context.Dispose();
                        context = null;
                    }
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DbContext DbContext
        {
            get { return this.context; }
        }
    }
}
