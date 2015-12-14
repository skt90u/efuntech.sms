using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Common
{
    /// <summary>
    /// 分頁排序用的查詢條件，共用屬性，方便實作。
    /// </summary>
    public class PagedCriteriaModel
    {
        public PagedCriteriaModel()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        /// <summary>
        /// 第幾頁.
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每頁筆數.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// 排序欄位.
        /// </summary>
        /// <value>
        /// The sort.
        /// </value>
        public string Sort { get; set; }
        /// <summary>
        /// 排序方式是否為降冪排序.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is descending; otherwise, <c>false</c>.
        /// </value>
        public string Order { get; set; }

        /// <summary>
        /// Download?
        /// </summary>
        /// <value>
        /// <c>Y</c> if this instance is descending; otherwise, <c>N</c>.
        /// </value>
        public bool IsDownload { get; set; }

        public bool IsDescending
        {
            get { return !"asc".Equals(Order); }
        }

        public override string ToString()
        {
            return string.Format("PageIndex: {0}, PageSize: {1}, Sort: {1}, Order: {1}, IsDownload: {1}, IsDescending: {1}",
                PageIndex, PageSize, Sort, Order, IsDownload, IsDescending);
        }
    }
}