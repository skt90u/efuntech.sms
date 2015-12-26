using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JUtilSharp.Database;
using System;

using System.Transactions;
using System.Net.Http.Formatting;

using System.Net.Http.Headers;
using OfficeOpenXml;
using System.IO;
using System.Data;
using System.Reflection;
using OfficeOpenXml.Style;
using System.Drawing;
using Ionic.Zip;
using System.Threading.Tasks;
using System.Text;
using EFunTech.Sms.Core;
using EFunTech.Sms.Schema;
using EFunTech.Sms.Portal.Models.Common;
using EntityFramework.Extensions;
using EntityFramework.Caching;

// http://aspnet.codeplex.com/SourceControl/changeset/view/7ce67a547fd0#Samples/WebApi/RelaySample/Controllers/RelayController.cs

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class _CrudApiController<TCriteria, TModel, TEntity, TIdentity> : ApiControllerBase
        where TCriteria : new()
        where TModel : new()
        where TEntity : class
    {
        protected IRepository<TEntity> repository;


        protected _CrudApiController(IUnitOfWork unitOfWork, ILogService logService)
            : base(unitOfWork, logService)
        {
            this.repository = this.unitOfWork.Repository<TEntity>();
        }

        /// <summary>
        /// 轉換Model的資料
        /// </summary>
        /// <param name="models">轉換Models</param>
        /// <returns>排序後的結果</returns>
        /// <remarks>
        /// </remarks>
        protected virtual IEnumerable<TModel> ConvertModel(IEnumerable<TModel> models)
        {
            return models;
        }

        #region ProduceFile
        
        /// <summary>
        /// ProduceFile 預設行為是將查詢到的資料以 Excel 的格式輸出
        /// </summary>
        protected virtual ReportDownloadModel ProduceFile(TCriteria criteria, IEnumerable<TModel> models)
        {
            string fileName = typeof(TModel).Name + ".xlsx";
            string sheetName = typeof(TModel).Name;
            return ProduceExcelFile(fileName, sheetName, models);
        }

        /// <summary>
        /// 輸出一個Excel裡面包含一個Sheet
        /// </summary>
        /// <example>
        /// return ProduceExcelFile(
        ///     fileName: "點數購買與匯轉明細.xlsx", 
        ///     sheetName: "點數購買與匯轉明細",
        ///     resultList: result.ToList());
        /// </example>
        protected ReportDownloadModel ProduceExcelFile<T>(string fileName, string sheetName, IEnumerable<T> models)
        {
            return ProduceExcelFile(fileName, new Dictionary<string, DataTable> { 
                { sheetName, Converter.ToDataTable(models) }
            });
        }

        /// <summary>
        /// 輸出一個Excel裡面包含多個Sheet
        /// 
        /// EPPlus 使用範例請參考
        ///     - 1. 從 http://epplus.codeplex.com/ 下載原始碼
        ///     - 2. 參考 EPPlusSamples
        /// </summary>
        protected ReportDownloadModel ProduceExcelFile(string fileName, Dictionary<string, DataTable> dict)
        {
            using (var package = new ExcelPackage())
            {
                byte[] bytes = null;

                foreach (var sheetName in dict.Keys)
                {
                    var table = dict[sheetName];
                    var worksheet = package.Workbook.Worksheets.Add(sheetName);
                    worksheet.Cells["A1"].LoadFromDataTable(table, true);
                    worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                    int ToCol = table.Columns.Count;

                    using (var range = worksheet.Cells[1, 1, 1, ToCol]) // int FromRow, int FromCol, int ToRow, int ToCol
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                        range.Style.Font.Color.SetColor(Color.White);
                    }
                }

                bytes = package.GetAsByteArray();

                return new ReportDownloadModel(bytes, fileName);
            }
        }

        /// <summary>
        /// 輸出一個Zip裡面包含多個Excel，每個Excel只有一個Sheet
        /// </summary>
        /// <example>
        ///  return ProduceZipFile(
        ///      fileName: "點數購買與匯轉明細.zip",
        ///      zipEntries: new Dictionary<string, DataTable> { 
        ///          {"點數購買與匯轉明細", Converter.ToDataTable(result.ToList())},
        ///      });
        /// </example>
        protected ReportDownloadModel ProduceZipFile(string fileName, Dictionary<string, DataTable> zipEntries)
        {
            // 壓縮檔中文亂碼
            Encoding encoding = Encoding.GetEncoding(950); // 繁體中文
            using (var zipFile = new ZipFile(encoding))
            {
                byte[] bytes = null;

                foreach (var sheetName in zipEntries.Keys)
                {
                    using (var package = new ExcelPackage())
                    {
                        var table = zipEntries[sheetName];
                        var worksheet = package.Workbook.Worksheets.Add(sheetName);
                        worksheet.Cells["A1"].LoadFromDataTable(table, true);
                        worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                        int ToCol = table.Columns.Count;

                        using (var range = worksheet.Cells[1, 1, 1, ToCol]) // int FromRow, int FromCol, int ToRow, int ToCol
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                            range.Style.Font.Color.SetColor(Color.White);
                        }

                        string entryName = string.Format("{0}.xlsx", sheetName);
                        zipFile.AddEntry(entryName, package.GetAsByteArray());
                    }
                }

                var memStream = new MemoryStream();
                zipFile.Save(memStream); 
                bytes = memStream.ToArray();

                return new ReportDownloadModel(bytes, fileName);
            }
        }
        
        #endregion

        #region GetAll

        protected abstract IQueryable<TEntity> DoGetList(TCriteria criteria);

        private bool IsDownload(TCriteria criteria)
        {
            var pagedCriteriaModel = (criteria as PagedCriteriaModel);

            return pagedCriteriaModel == null ? false : pagedCriteriaModel.IsDownload;
        }

        [System.Web.Http.HttpGet]
        public virtual Task<HttpResponseMessage> GetAll([FromUri] TCriteria criteria)
        {
            try
            {
                // 避免部分的查詢沒有條件，傳入 null 值
                if (null == criteria)
                {
                    criteria = new TCriteria();
                }
                
                IQueryable<TEntity> entities = DoGetList(criteria);
                IQueryable<TModel> models = entities.Project().To<TModel>();

                if (IsDownload(criteria))
                {
                    var result = ConvertModel(models.ToList());

                    var reportDownloadModel = ProduceFile(criteria, result);

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = reportDownloadModel.Content
                    };
                    response.Content.Headers.ContentType = reportDownloadModel.ContentType;
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = reportDownloadModel.FileName
                    };
                    return Task.FromResult(response);
                }
                else
                {
                    var aPagedCriteria = criteria as PagedCriteriaModel;
                    if (aPagedCriteria == null)
                    {
                        var totalCount = models.Count();
                        var result = ConvertModel(models.ToList());

                        var response = this.Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Criteria = criteria,
                            TotalCount = totalCount,
                            Result = result,
                            WebPath = string.Empty,
                        });

                        return Task.FromResult(response);
                    }
                    else
                    {
                        if (aPagedCriteria.PageSize == -1)
                            aPagedCriteria.PageSize = Int32.MaxValue - 1;

                        int pageIndex = aPagedCriteria.PageIndex;
                        int pageSize = aPagedCriteria.PageSize;

                        var totalCount = models.Count();
                        var result = ConvertModel(models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());

                        var response = this.Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Criteria = criteria,
                            TotalCount = totalCount,
                            Result = result,
                            WebPath = string.Empty,
                        });

                        return Task.FromResult(response);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        #endregion

        #region GetById

        protected virtual TEntity DoGet(TIdentity id)
        {
            return this.unitOfWork.Repository<TEntity>().DbSet.Find(id);
        }

        [System.Web.Http.HttpGet]
        public virtual Task<TModel> GetById(TIdentity id)
        {
            TEntity entity = DoGet(id);

            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var model = Mapper.Map<TEntity, TModel>(entity);

            return Task.FromResult(model);
        }
        #endregion

        #region Create

        protected abstract TEntity DoCreate(TModel model, TEntity entity, out TIdentity id);
        // POST api/<controller>
        /// <summary>
        /// .新增資料
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public virtual Task<HttpResponseMessage> Create([FromBody]TModel model)
        {
            try
            {
                TIdentity id = default(TIdentity);

                TEntity entity = Mapper.Map<TModel, TEntity>(model);

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    entity = DoCreate(model, entity, out id);
                    scope.Complete();
                }

                var response = this.WrapPostResponseMessage(Mapper.Map<TEntity, TModel>(entity), id.ToString());
                return Task.FromResult(response);
            }
            catch(Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        private HttpResponseMessage WrapPostResponseMessage<T>(T entity, string id)
        {
            HttpResponseMessage respMessage = this.Request.CreateResponse<T>(HttpStatusCode.Created, entity);

            string uri = this.Url.Link("DefaultApi", new { id = id });
            respMessage.Headers.Location = new Uri(uri);

            return respMessage;
        }

        #endregion

        #region Update

        protected abstract void DoUpdate(TModel model, TIdentity id, TEntity entity);

        // PUT api/<controller>/{id}
        /// <summary>
        /// 更新資料.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [System.Web.Http.HttpPut]
        public virtual Task<HttpResponseMessage> Update(TIdentity id, [FromBody]TModel model)
        {
            try
            {
                TEntity entity = DoGet(id);
                if (entity == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                Mapper.Map(model, entity);

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    DoUpdate(model, id, entity);
                    scope.Complete();
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
        #endregion

        #region Delete

        protected abstract void DoRemove(TIdentity[] ids);
        protected abstract void DoRemove(TIdentity id);

        // DELETE api/<controller>/{idsWithComma}
        /// <summary>
        /// 刪除多筆資料.
        /// </summary>
        [System.Web.Http.HttpDelete]
        public virtual Task<HttpResponseMessage> Delete([FromUri] TIdentity[] ids)
        {
            try
            {
                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    DoRemove(ids);
                    scope.Complete();
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
        //public virtual Task<HttpResponseMessage> Delete([FromUri] TIdentity[] ids)
        //{
        //    try
        //    {
        //        List<TEntity> entities = new List<TEntity>();

        //        foreach (var id in ids)
        //        {
        //            TEntity entity = DoGet(id);
        //            if (entity == null)
        //            {
        //                throw new HttpResponseException(HttpStatusCode.NotFound);
        //            }
        //            entities.Add(entity);
        //        }

        //        using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
        //        {
        //            DoRemove(ids.ToList(), entities);
        //            scope.Complete();
        //        }

        //        var response = Request.CreateResponse(HttpStatusCode.OK);

        //        return Task.FromResult(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logService.Error(ex);

        //        throw;
        //    }
        //}

        // DELETE api/<controller>/{id}
        /// <summary>
        /// 刪除資料.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [System.Web.Http.HttpDelete]
        public virtual Task<HttpResponseMessage> Delete(TIdentity id)
        {
            try
            {
                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    DoRemove(id);
                    scope.Complete();
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
        //public virtual Task<HttpResponseMessage> Delete(TIdentity id)
        //{
        //    try
        //    {
        //        TEntity entity = DoGet(id);
        //        if (entity == null)
        //        {
        //            throw new HttpResponseException(HttpStatusCode.NotFound);
        //        }

        //        using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
        //        {
        //            DoRemove(id, entity);
        //            scope.Complete();
        //        }

        //        var response = Request.CreateResponse(HttpStatusCode.OK);

        //        return Task.FromResult(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logService.Error(ex);

        //        throw;
        //    }
        //}
        #endregion
    }

    #region WebApiQueryResult
    public class WebApiQueryResult<T>
    {
        #region Propreties

        public int TotalCount { get; set; }
        public object Criteria { get; set; }
        public List<T> Result { get; set; }
        public ReportDownloadModel WebPath { get; set; }
        #endregion

        public WebApiQueryResult()
        {

        }

        public WebApiQueryResult(IEnumerable<T> result)
            : this()
        {
            this.Result = result.ToList();
            this.TotalCount = result.Count();
        }

        public WebApiQueryResult(IEnumerable<T> result, int totalCount)
            : this()
        {
            this.TotalCount = totalCount;
            this.Result = result.ToList();
        }


        public WebApiQueryResult(ReportDownloadModel webPath)
            : this()
        {
            this.TotalCount = 0;
            this.Result = null;
            this.WebPath = webPath;
        }

    }
    #endregion

    #region Pagination

    public interface IPagedList
    {
        int TotalCount { get; set; }
        int PageIndex { get; set; }
        int PageSize { get; set; }
        bool IsPreviousPage { get; }
        bool IsNextPage { get; }
    }

    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize = 10)
        {
            return new PagedList<T>(source, index, pageSize);
        }

        public static MappedPagedList<TSource, TOutput> ToPagedList<TSource, TOutput>(this IQueryable<TSource> source, int index, Func<IEnumerable<TSource>, IEnumerable<TOutput>> mapper, int pageSize = 10)
        {
            return new MappedPagedList<TSource, TOutput>(source, index, pageSize, mapper);
        }
    }

    public class PagedList<T> : List<T>, IPagedList
    {
        public PagedList(IQueryable<T> source, int index, int pageSize)
            : this(index, pageSize, source.Count())
        {
            this.AddRange(source.Skip(index * pageSize).Take(pageSize).ToList());
        }

        public PagedList(List<T> source, int index, int pageSize)
            : this(index, pageSize, source.Count())
        {
            this.AddRange(source.Skip(index * pageSize).Take(pageSize).ToList());
        }

        protected PagedList(int index, int pageSize, int totalCount)
        {
            this.PageSize = pageSize;
            this.PageIndex = index;
            this.TotalCount = totalCount;
        }

        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public bool IsPreviousPage { get { return (PageIndex > 0); } }
        public bool IsNextPage { get { return (PageIndex * PageSize) < TotalCount - PageSize; } }
    }

    public class MappedPagedList<TSource, TOutput> : PagedList<TOutput>
    {
        /// <param name="source">Source of items to page</param>
        /// <param name="index">Zero-based page index</param>
        /// <param name="pageSize">Number of items to display per page</param>
        /// <param name="mapper">A mapping function, most easily created using AutoMapper</param>
        public MappedPagedList(IQueryable<TSource> source, int index, int pageSize, Func<IEnumerable<TSource>, IEnumerable<TOutput>> map)
            : base(index, pageSize, source.Count())
        {
            this.AddRange(map(source.Skip(index * pageSize).Take(pageSize).ToList()));
        }

        /// <param name="source">Source of items to page</param>
        /// <param name="index">Zero-based page index</param>
        /// <param name="pageSize">Number of items to display per page</param>
        /// <param name="m">A mapping function, most easily created using AutoMapper</param>
        public MappedPagedList(List<TSource> source, int index, int pageSize, Func<IEnumerable<TSource>, IEnumerable<TOutput>> map)
            : base(index, pageSize, source.Count())
        {
            this.AddRange(map(source.Skip(index * pageSize).Take(pageSize).ToList()));
        }
    }
	
    #endregion
}