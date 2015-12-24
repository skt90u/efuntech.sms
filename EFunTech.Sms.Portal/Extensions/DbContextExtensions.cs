using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using EntityFramework.Extensions;
using System.Threading.Tasks;
using System.Transactions;

namespace EFunTech.Sms.Portal
{
    public static class DbContextExtensions
    {
        public static TransactionScope CreateTransactionScope(this DbContext context)
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            return new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }
            );
        }

        public static async Task<TEntity> InsertAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            var newEntry = context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return newEntry;
        }

        public static async Task<int> UpdateAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            var entry = context.Entry(entity);
            context.Set<TEntity>().Attach(entity);
            entry.State = EntityState.Modified;

            return await context.SaveChangesAsync();
        }

        public static async Task<int> DeleteAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            context.Set<TEntity>().Remove(entity);
            return await context.SaveChangesAsync();
        }

        public static async Task<int> DeleteAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            context.Set<TEntity>().Where(predicate).Delete();
            return await context.SaveChangesAsync();
        }
    }
}