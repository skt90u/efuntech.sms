using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JUtilSharp.Database
{
    public interface IRepository<TEntity>  where TEntity : class
    {
        TEntity GetById(params object[] keys);
        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate);

        //IQueryable<T> SqlQuery(string sql, params object[] parameters);

        bool Any(Expression<Func<TEntity, bool>> predicate);
        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);

        TEntity Insert(TEntity entity);
        int Update(TEntity entity);
        int Delete(TEntity entity);
        int Delete(Expression<Func<TEntity, bool>> predicate);

        // IRepository<TEntity> Include(Expression<Func<TEntity, object>> include); 
        // 20151028 Norman, 目前似乎搞不定
        //"Exception0 = There is already an open DataReader associated with this Command which must be closed first.

        //Exception1 = An error occurred while executing the command definition. See the inner exception for details."	"   at System.Data.Entity.Core.EntityClient.Internal.EntityCommandDefinition.ExecuteStoreCommands(EntityCommand entityCommand, CommandBehavior behavior)
        //   at System.Data.Entity.Core.Objects.Internal.ObjectQueryExecutionPlan.Execute[TResultType](ObjectContext context, ObjectParameterCollection parameterValues)
        //   at System.Data.Entity.Core.Objects.ObjectQuery`1.<>c__DisplayClass7.<GetResults>b__6()
        //   at System.Data.Entity.Core.Objects.ObjectContext.ExecuteInTransaction[T](Func`1 func, IDbExecutionStrategy executionStrategy, Boolean startLocalTransaction, Boolean releaseConnectionOnSuccess)
        //   at System.Data.Entity.Core.Objects.ObjectQuery`1.<>c__DisplayClass7.<GetResults>b__5()
        //   at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)
        //   at System.Data.Entity.Core.Objects.ObjectQuery`1.GetResults(Nullable`1 forMergeOption)
        //   at System.Data.Entity.Core.Objects.ObjectQuery`1.<System.Collections.Generic.IEnumerable<T>.GetEnumerator>b__0()
        //   at System.Data.Entity.Internal.LazyEnumerator`1.MoveNext()
        //   at System.Data.Entity.QueryableExtensions.Load(IQueryable source)
        //   at JUtilSharp.Database.Repository`1.Include(Expression`1 include) in c:\Project\efuntech.sms\JUtilSharp\Database\Repository.cs:line 43
        //   at EFunTech.Sms.Portal.Controllers.Common.MvcControllerBase.GetMenuItems() in c:\Project\efuntech.sms\EFunTech.Sms.Portal\Controllers\Common\MvcControllerBase.cs:line 97
        //   at EFunTech.Sms.Portal.Controllers.HomeController.Index() in c:\Project\efuntech.sms\EFunTech.Sms.Portal\Controllers\HomeController.cs:line 22"
    }
}
