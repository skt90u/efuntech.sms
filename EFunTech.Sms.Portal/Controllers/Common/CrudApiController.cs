using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using JUtilSharp.Database;
using EFunTech.Sms.Portal.Models.Common;
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

// http://aspnet.codeplex.com/SourceControl/changeset/view/7ce67a547fd0#Samples/WebApi/RelaySample/Controllers/RelayController.cs

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class CrudApiController<TCriteria, TModel, TEntity, TIdentity> : ApiControllerBase
        where TCriteria : new()
        where TModel : new()
        where TEntity : class
    {
        protected IRepository<TEntity> repository;


        protected CrudApiController(IUnitOfWork unitOfWork, ILogService logService): base(unitOfWork, logService)
        {
            this.repository = this.unitOfWork.Repository<TEntity>();
        }

        /// <summary>
        /// 依照前端的排序資訊，排序Model的資料
        /// </summary>
        /// <param name="models">要排序的Models</param>
        /// <param name="criteria">查詢條件的 Model.</param>
        /// <returns>排序後的結果</returns>
        /// <remarks>
        /// </remarks>
        //protected virtual IEnumerable<TModel> SortModel(IEnumerable<TModel> models, TCriteria criteria)
        //{
        //    return models;
        //}

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
        protected virtual ReportDownloadModel ProduceFile(TCriteria criteria, List<TModel> resultList)
        {
            string fileName = typeof(TModel).Name + ".xlsx";
            string sheetName = typeof(TModel).Name;
            return ProduceExcelFile(fileName, sheetName, resultList);
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
        protected ReportDownloadModel ProduceExcelFile<T>(string fileName, string sheetName, List<T> resultList)
        {
            return ProduceExcelFile(fileName, new Dictionary<string, DataTable> { 
                { sheetName, Converter.ToDataTable(resultList) }
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

        protected abstract IOrderedQueryable<TEntity> DoGetList(TCriteria criteria);

        private bool IsDownload(TCriteria criteria)
        {
            var pagedCriteriaModel = (criteria as PagedCriteriaModel);

            return pagedCriteriaModel == null ? false : pagedCriteriaModel.IsDownload;
        }

        [System.Web.Http.HttpGet]
        public virtual HttpResponseMessage GetAll([FromUri] TCriteria criteria)
        {
            try
            {
                // 避免部分的查詢沒有條件，傳入 null 值
                if (null == criteria)
                {
                    criteria = new TCriteria();
                }
                
                // 透過 subclass 實作 DoGetList 方法，回傳一個排序過的 Queryable 物件
                // 因為有可能需要作 Take / Skip 需要先排序
                IQueryable<TEntity> orderedResult = DoGetList(criteria);
                if (orderedResult == null) // 強制要求使用正確寫法
                {
                    throw new Exception(string.Format("DoGetList 不可以回傳 null，若查詢不到任何資料，請回傳 Enumerable.Empty<{0}>().AsQueryable()", typeof(TEntity).Name));
                    //orderedResult = Enumerable.Empty<TEntity>().AsQueryable(); // 檢查 Enumerable.Empty 用法，在 Union 會出問題
                }

                //var result = orderedResult.Project().To<TModel>();
                //var b = orderedResult.ProjectTo<TModel>();

                var result = Mapper.Map<IEnumerable<TEntity>, IEnumerable<TModel>>(orderedResult);
                result = ConvertModel(result);
                //result = SortModel(result, criteria);

                if (IsDownload(criteria))
                {
                    var reportDownloadModel = ProduceFile(criteria, result.ToList());

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = reportDownloadModel.Content
                    };
                    response.Content.Headers.ContentType = reportDownloadModel.ContentType;
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = reportDownloadModel.FileName
                    };
                    return response;
                }
                else
                {
                    WebApiQueryResult<TModel> content = null;

                    var aPagedCriteriaModel = criteria as PagedCriteriaModel;
                    if (aPagedCriteriaModel == null)
                    {
                        content = new WebApiQueryResult<TModel>(result) { Criteria = criteria };
                    }
                    else
                    {
                        int totalCount = orderedResult.Count();

                        if (aPagedCriteriaModel.PageSize == -1)
                            aPagedCriteriaModel.PageSize = Int32.MaxValue - 1;

                        content = new WebApiQueryResult<TModel>(result.AsQueryable().ToPagedList(aPagedCriteriaModel.PageIndex - 1,
                                                                aPagedCriteriaModel.PageSize),
                                                             totalCount) { Criteria = criteria };
                    }

                    HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    responseMessage.Content = new ObjectContent<WebApiQueryResult<TModel>>(content, new JsonMediaTypeFormatter());
                    return responseMessage;
                }
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        //[System.Web.Http.HttpGet]
        //public virtual WebApiQueryResult<TModel> GetAll([FromUri] TCriteria criteria)
        //{
        //    // 避免部分的查詢沒有條件，傳入 null 值
        //    if (null == criteria)
        //    {
        //        criteria = new TCriteria();
        //    }
            
        //    // 透過 subclass 實作 DoGetList 方法，回傳一個排序過的 Queryable 物件
        //    // 因為有可能需要作 Take / Skip 需要先排序
        //    IQueryable<TEntity> orderedResult = DoGetList(criteria);
        //    if (orderedResult == null) // 強制要求使用正確寫法
        //    {
        //        throw new Exception(string.Format("DoGetList 不可以回傳 null，若查詢不到任何資料，請回傳 Enumerable.Empty<{0}>().AsQueryable()", typeof(TEntity).Name));
        //        //orderedResult = Enumerable.Empty<TEntity>().AsQueryable();
        //    }

        //    var properties = typeof(TCriteria).GetProperties();
            

        //    if (IsDownload)
        //    {
        //        var result = Mapper.Map<IEnumerable<TEntity>, IEnumerable<TModel>>(orderedResult);
        //        var DownloadPath = ProduceFile(result.ToList());
        //        return new WebApiQueryResult<TModel>(DownloadPath) { Criteria = criteria };
        //    }
        //    else
        //    {
        //        // 如果查詢條件沒有繼承 PagedCriteriaModel, 表示不需要分頁
        //        // 直接轉換後回傳結果
        //        var result = Mapper.Map<IEnumerable<TEntity>, IEnumerable<TModel>>(orderedResult);
        //        result = ConvertModel(result);
        //        result = SortModel(result, criteria);
        //        if (!(criteria is PagedCriteriaModel))
        //        {

        //            return new WebApiQueryResult<TModel>(result) { Criteria = criteria };
        //        }
        //        // 如果有需要分頁，就可以透過 ToPagedList 這個 Extension Method 來轉換 
        //        var aPagedCriteriaModel = criteria as PagedCriteriaModel;
        //        int totalCount = orderedResult.Count();

        //        return new WebApiQueryResult<TModel>(result.AsQueryable().ToPagedList(aPagedCriteriaModel.PageIndex - 1,
        //                                                aPagedCriteriaModel.PageSize),
        //                                             totalCount) { Criteria = criteria };
        //    }
        //}

        #endregion

        #region GetById

        protected abstract TEntity DoGet(TIdentity id);

        [System.Web.Http.HttpGet]
        public virtual TModel GetById(TIdentity id)
        {
            TEntity entity = DoGet(id);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return Mapper.Map<TEntity, TModel>(entity);
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
        public virtual HttpResponseMessage Create([FromBody]TModel model)
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

                return this.WrapPostResponseMessage(Mapper.Map<TEntity, TModel>(entity), id.ToString());
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
        public virtual HttpResponseMessage Update(TIdentity id, [FromBody]TModel model)
        {
            try
            {
                TEntity entity = this.repository.GetById(id);
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
                
                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
        #endregion

        #region Delete

        protected abstract void DoRemove(List<TIdentity> ids, List<TEntity> entities);
        protected abstract void DoRemove(TIdentity id, TEntity entity);

        // DELETE api/<controller>/{idsWithComma}
        /// <summary>
        /// 刪除多筆資料.
        /// </summary>
        [System.Web.Http.HttpDelete]
        //public virtual HttpResponseMessage DeleteMulti([FromUri]TIdentity[] ids)
        public virtual HttpResponseMessage Delete([FromUri]TIdentity[] ids)
        {
            try
            {
                List<TEntity> entities = new List<TEntity>();

                foreach (var id in ids)
                {
                    TEntity entity = this.repository.GetById(id);
                    if (entity == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    entities.Add(entity);
                }

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    DoRemove(ids.ToList(), entities);
                    scope.Complete();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
                //return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        // DELETE api/<controller>/{id}
        /// <summary>
        /// 刪除資料.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [System.Web.Http.HttpDelete]
        public virtual HttpResponseMessage Delete(TIdentity id)
        {
            try
            {
                TEntity entity = this.repository.GetById(id);
                if (entity == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                using (TransactionScope scope = this.unitOfWork.CreateTransactionScope())
                {
                    DoRemove(id, entity);
                    scope.Complete();
                }

                return Request.CreateResponse(HttpStatusCode.OK);
                //return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
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