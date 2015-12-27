using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Transactions;
using System.Net.Http.Headers;
using OfficeOpenXml;
using System.IO;
using System.Data;
using OfficeOpenXml.Style;
using System.Drawing;
using Ionic.Zip;
using System.Threading.Tasks;
using System.Text;
using EFunTech.Sms.Core;
using EFunTech.Sms.Portal.Models.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

// http://aspnet.codeplex.com/SourceControl/changeset/view/7ce67a547fd0#Samples/WebApi/RelaySample/Controllers/RelayController.cs

namespace EFunTech.Sms.Portal.Controllers.Common
{
    public abstract class CrudApiController<TCriteria, TModel, TEntity, TIdentity> : ApiControllerBase
        where TCriteria : new()
        where TModel : new()
        where TEntity : class
    {
        protected CrudApiController(DbContext context, ILogService logService)
            : base(context, logService)
        {
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
        public async Task<HttpResponseMessage> GetAll([FromUri] TCriteria criteria)
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
                bool hasDbAsyncQueryProvider = models is IDbAsyncQueryProvider;

                if (IsDownload(criteria))
                {
                    var result = ConvertModel(hasDbAsyncQueryProvider
                        ? await models.ToListAsync()
                        : models.ToList());

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

                    return response;
                }
                else
                {
                    var aPagedCriteria = criteria as PagedCriteriaModel;
                    if (aPagedCriteria == null)
                    {
                        var totalCount = hasDbAsyncQueryProvider
                            ? await models.CountAsync()
                            : models.Count();

                        var result = ConvertModel(hasDbAsyncQueryProvider
                            ? await models.ToListAsync()
                            : models.ToList());

                        var response = this.Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Criteria = criteria,
                            TotalCount = totalCount,
                            Result = result,
                            WebPath = string.Empty,
                        });

                        return response;
                    }
                    else
                    {
                        if (aPagedCriteria.PageSize == -1)
                            aPagedCriteria.PageSize = Int32.MaxValue - 1;

                        int pageIndex = aPagedCriteria.PageIndex;
                        int pageSize = aPagedCriteria.PageSize;

                        var totalCount = hasDbAsyncQueryProvider
                            ? await models.CountAsync()
                            : models.Count();

                        var result = ConvertModel(hasDbAsyncQueryProvider
                            ? await models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
                            : models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());

                        var response = this.Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Criteria = criteria,
                            TotalCount = totalCount,
                            Result = result,
                            WebPath = string.Empty,
                        });

                        return response;
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

        public virtual Task<TEntity> DoGet(TIdentity id)
        {
            return context.Set<TEntity>().FindAsync(id);
        }

        [System.Web.Http.HttpGet]
        public async Task<TModel> GetById(TIdentity id)
        {
            TEntity entity = await DoGet(id);

            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = Mapper.Map<TEntity, TModel>(entity);

            return model;
        }
        #endregion

        #region Create

        protected virtual Task<TEntity> DoCreate(TModel model, TEntity entity)
        {
            throw new NotImplementedException();
        }

        // POST api/<controller>
        /// <summary>
        /// .新增資料
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> Create([FromBody]TModel model)
        {
            try
            {
                TEntity entity = Mapper.Map<TModel, TEntity>(model);

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    entity = await DoCreate(model, entity);

                    scope.Complete();
                }

                var response = this.Request.CreateResponse(HttpStatusCode.Created,
                                Mapper.Map<TEntity, TModel>(entity));

                return response;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        #endregion

        #region Update

        protected virtual Task DoUpdate(TModel model, TIdentity id, TEntity entity)
        {
            throw new NotImplementedException();
        }

        // PUT api/<controller>/{id}
        /// <summary>
        /// 更新資料.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> Update(TIdentity id, [FromBody]TModel model)
        {
            try
            {
                TEntity entity = await DoGet(id);

                if (entity == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                Mapper.Map(model, entity);

                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    await DoUpdate(model, id, entity);

                    scope.Complete();
                }

                var response = this.Request.CreateResponse(HttpStatusCode.OK);

                return response;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }
        #endregion

        #region Delete

        protected virtual Task DoRemove(TIdentity[] ids)
        {
            throw new NotImplementedException();
        }

        protected virtual Task DoRemove(TIdentity id)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<controller>/{idsWithComma}
        /// <summary>
        /// 刪除多筆資料.
        /// </summary>
        [System.Web.Http.HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] TIdentity[] ids)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    await DoRemove(ids);

                    scope.Complete();
                }

                var response = this.Request.CreateResponse(HttpStatusCode.OK);

                return response;
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
        public async Task<HttpResponseMessage> Delete(TIdentity id)
        {
            try
            {
                using (TransactionScope scope = context.CreateTransactionScope())
                {
                    await DoRemove(id);

                    scope.Complete();
                }

                var response = this.Request.CreateResponse(HttpStatusCode.OK);

                return response;
            }
            catch (Exception ex)
            {
                this.logService.Error(ex);

                throw;
            }
        }

        #endregion
    }



}