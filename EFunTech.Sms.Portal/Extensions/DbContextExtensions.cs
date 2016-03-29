using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using EntityFramework.Extensions;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.Entity.Validation;

namespace EFunTech.Sms.Portal
{
    public static class DbContextExtensions
    {
        public static TransactionScope CreateTransactionScope(this DbContext context)
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //return new TransactionScope(
            //    // a new transaction will always be created
            //    TransactionScopeOption.RequiresNew,
            //    // we will allow volatile data to be read during transaction
            //    new TransactionOptions()
            //    {
            //        IsolationLevel = IsolationLevel.ReadUncommitted
            //    }
            //);
        }

        public static async Task<TEntity> InsertAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            var newEntry = context.Set<TEntity>().Add(entity);
            await context.MySaveChangesAsync();
            return newEntry;
        }

        public static async Task<int> UpdateAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            var entry = context.Entry(entity);
            context.Set<TEntity>().Attach(entity);
            entry.State = EntityState.Modified;

            return await context.MySaveChangesAsync();
        }

        public static async Task<int> DeleteAsync<TEntity>(this DbContext context, TEntity entity)
            where TEntity : class
        {
            context.Set<TEntity>().Remove(entity);
            return await context.MySaveChangesAsync();
        }

        public static async Task<int> DeleteAsync<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            context.Set<TEntity>().Where(predicate).Delete();
            return await context.MySaveChangesAsync();
        }

        public static Task<int> MySaveChangesAsync(this DbContext context)
        {
            try
            {
                return context.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage =
                          string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}