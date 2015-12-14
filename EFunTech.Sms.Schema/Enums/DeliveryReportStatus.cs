using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFunTech.Sms.Schema
{
    [TableDescription("簡訊派送結果狀態(通用型-用於寫入SendMessageHistory)")]
    public enum DeliveryReportStatus
    {
        // 20151119 Norman
        // 若一天之後，SendMessageHistory 狀態仍然為 MessageAccepted (仍未取得派送結果)，
        // 則將此筆 SendMessageHistory 狀態改成 DeliveryReportTimeout
        [ColumnDescription("傳送中")] // 自訂狀態(非Infobip,Every8d狀態)
        MessageAccepted = 1000,

        ////////////////////////////////////////
        // Infobip
        ////////////////////////////////////////

        [ColumnDescription("未定義")]
        Unknown = -999, // 因為 Timeout_1 = -1, Timeout_2 = -2, 所以將 Unknown = -999

        /// <summary>
        /// 歸類為 [發送成功]
        /// </summary>
        [ColumnDescription("發送成功")]
        DeliveredToTerminal = 1001,

        /// <summary>
        /// 歸類為 [逾時]
        /// </summary>
        [ColumnDescription("逾時收訊")]
        DeliveryUncertain = 1002,
        [ColumnDescription("逾時收訊")]
        DeliveryImpossible = 1003,

        /// <summary>
        /// 歸類為 [傳送中]
        /// </summary>
        //[ColumnDescription("傳送中")]
        //MessageWaiting = 1004,
        //[ColumnDescription("傳送中")]
        //DeliveredToNetwork = 1005,

        // 20151121 Norman, 將 Infobip 所有不是 DeliveredToTerminal 的狀態都判定為逾時
        [ColumnDescription("逾時收訊")]
        MessageWaiting = 1004,
        [ColumnDescription("逾時收訊")]
        DeliveredToNetwork = 1005,

        // 20151119 Norman
        // 若一天之後，SendMessageHistory 狀態仍然為 MessageAccepted (仍未取得派送結果)，
        // 則將此筆 SendMessageHistory 狀態改成 DeliveryReportTimeout
        [ColumnDescription("逾時收訊")] // 自訂狀態(非Infobip,Every8d狀態)
        DeliveryReportTimeout = 1006,

        ////////////////////////////////////////
        // Every8d
        ////////////////////////////////////////

        /// <summary>
        /// 歸類為 [傳送中]
        /// </summary>
        [ColumnDescription("傳送中")]
        Sending = 0,

        /// <summary>
        /// 歸類為 [發送成功]
        /// </summary>
        [ColumnDescription("發送成功")]
        Sent = 100,

        /// <summary>
        /// 歸類為 [空號](無此手機號碼)
        /// </summary>
        [ColumnDescription("空號")]
        PhoneNumberNotAvailable = 103,

        /// <summary>
        /// 歸類為 [電話號碼格式錯誤]
        /// </summary>
        [ColumnDescription("電話號碼格式錯誤")]
        WrongPhoneNumber = -3,

        /// <summary>
        /// 歸類為 [逾時收訊](手機端因素未能送達)
        /// </summary>
        [ColumnDescription("逾時收訊")]
        TerminalUncertain = 101,

        /// <summary>
        /// 歸類為 [逾時收訊](電信終端設備異常未能送達)
        /// </summary>
        [ColumnDescription("逾時收訊")]
        NetworkUncertain102 = 102,
        [ColumnDescription("逾時收訊")]
        NetworkUncertain104 = 104,
        [ColumnDescription("逾時收訊")]
        NetworkUncertain105 = 105,
        [ColumnDescription("逾時收訊")]
        NetworkUncertain106 = 106,

        /// <summary>
        /// 歸類為 [逾時收訊]
        /// </summary>
        [ColumnDescription("逾時收訊")]
        Timeout107 = 107,
        [ColumnDescription("逾時收訊")]
        Timeout_1 = -1,
        [ColumnDescription("逾時收訊")]
        Timeout_2 = -2,
        [ColumnDescription("逾時收訊")]
        Timeout_4 = -4,
        [ColumnDescription("逾時收訊")]
        Timeout_5 = -5,
        [ColumnDescription("逾時收訊")]
        Timeout_6 = -6,
        [ColumnDescription("逾時收訊")]
        Timeout_8 = -8,
        [ColumnDescription("逾時收訊")]
        Timeout_9 = -9,
        [ColumnDescription("逾時收訊")]
        Timeout_32 = -32,
        [ColumnDescription("逾時收訊")]
        Timeout_100 = -100,
        [ColumnDescription("逾時收訊")]
        Timeout_101 = -101,
        [ColumnDescription("逾時收訊")]
        Timeout_201 = -201,
        [ColumnDescription("逾時收訊")]
        Timeout_202 = -202,
        [ColumnDescription("逾時收訊")]
        Timeout_203 = -203,
    }
}
