using System;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using EntityFramework.Extensions;

namespace JUtilSharp.Database
{
    /// <summary>
    /// 執行檔專案必須加入EntityFramework
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext context = null;
        private DbSet<TEntity> dbSet = null;

        public Repository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// 起因於 public List<MenuItemModel> GetMenuItems() 發生以下錯誤
        /// There is already an open DataReader associated with this Command which must be closed first.
        /// 
        /// 解決方式：(尚未確認哪個可行)
        /// http://stackoverflow.com/questions/12625929/multiple-includes-using-entity-framework-and-repository-pattern
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        //public virtual IRepository<TEntity> Include(Expression<Func<TEntity, object>> include)
        //{
        //    // http://stackoverflow.com/questions/5376421/ef-including-other-entities-generic-repository-pattern

        //    this.dbSet.Include(include).Load();

        //    return this;
        //}

        public virtual TEntity GetById(params object[] keys)
        {
            return this.dbSet.Find(keys);
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbSet.FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return this.dbSet.AsQueryable();
        }

        public IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbSet.Where(predicate).AsQueryable<TEntity>();
        }

        //public virtual IQueryable<TEntity> SqlQuery(string sql, params object[] parameters)
        //{
        //    return this.dbSet.SqlQuery(sql, parameters).AsQueryable();
        //}

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbSet.Any(predicate);
        }

        public int Count()
        {
            return this.dbSet.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbSet.Count(predicate);
        }

        public TEntity Insert(TEntity entity)
        {
            var newEntry = this.dbSet.Add(entity);
            this.SaveChanges();
            return newEntry;
        }

        public int Update(TEntity entity)
        {
            var entry = context.Entry(entity);
            this.dbSet.Attach(entity);
            entry.State = EntityState.Modified;
            return this.SaveChanges();
        }

        public int Delete(TEntity entity)
        {
            this.dbSet.Remove(entity);
            return this.SaveChanges();
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            this.dbSet.Where(predicate).Delete();
            return this.SaveChanges();
        }

        /// <summary>
        /// 直接覆寫SaveChanges方法使其拋出有意義之訊息
        /// 
        /// http://www.dotblogs.com.tw/wasichris/archive/2015/01/24/148255.aspx
        /// </summary>
        /// <returns></returns>
        private int SaveChanges()
        {
            try
            {
                return this.context.SaveChanges();
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
