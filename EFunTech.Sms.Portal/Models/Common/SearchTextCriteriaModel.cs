using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFunTech.Sms.Portal.Models.Common
{
    /// <summary>
    /// 簡易搜尋文字查詢條件的 Model
    /// </summary>
    public class SearchTextCriteriaModel : PagedCriteriaModel
    {
        /// <summary>
        /// 模糊搜尋的文字.
        /// </summary>
        public string SearchText { get; set; }

        public override string ToString()
        {
            return string.Format("SearchText: {0}, {1}",
                SearchText,
                base.ToString());
        }
    }
}