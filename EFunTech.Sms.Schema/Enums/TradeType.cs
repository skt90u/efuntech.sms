using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("交易類別")]
    public enum TradeType
    {
        [ColumnDescription("全部")] // 用於查詢條件的下拉選單中的選項
        All = 0,

        [ColumnDescription("發送扣點")]
        DeductionOfSendMessage = 1,

        [ColumnDescription("發送回補")]
        CoverOfSendMessage = 2,

        [ColumnDescription("儲值")]
        Deposit = 3,

        [ColumnDescription("回補")]
        Cover = 4,

        [ColumnDescription("點數匯出")]
        ExportPoints = 5,

        [ColumnDescription("點數匯入")]
        ImportPoints = 6,

        [ColumnDescription("回收點數匯出")]
        ExportRecoveryPoints = 7,

        [ColumnDescription("回收點數匯入")]
        ImportRecoveryPoints = 8,

        [ColumnDescription("簡訊重新發送")]
        RetrySMS = 9,

        //[ColumnDescription("0800互動簡訊月租費扣點")]
        //MonthlyFeeDeductionOfInteractiveNewsletter = 9,

        //[ColumnDescription("0800互動簡訊預算扣點")]
        //BudgetDeductionOfInteractiveNewsletter = 10,

        //[ColumnDescription("0800互動簡訊預算回補")]
        //CoveringBudgetOfInteractiveNewsletter = 11,
    }
}
