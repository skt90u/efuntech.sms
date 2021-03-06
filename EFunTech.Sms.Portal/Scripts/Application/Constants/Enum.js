﻿(function (window, document) {
    'use strict';

	////////////////////////////////////////
	// 這個檔案是經由 EFunTech.Sms.CodeGenerator.SyncEnum 自動產生，請勿手動修改
	//
	// 最後產製時間： 2016/02/17 14:13:31
	////////////////////////////////////////

    /**
     * 簡訊派送結果狀態(通用型-用於寫入SendMessageHistory)
     */
    var DeliveryReportStatus = {
        Sending: {value: 0,  text: '傳送中'},
        Sent: {value: 100,  text: '發送成功'},
        TerminalUncertain: {value: 101,  text: '逾時收訊'},
        NetworkUncertain102: {value: 102,  text: '逾時收訊'},
        PhoneNumberNotAvailable: {value: 103,  text: '空號'},
        NetworkUncertain104: {value: 104,  text: '逾時收訊'},
        NetworkUncertain105: {value: 105,  text: '逾時收訊'},
        NetworkUncertain106: {value: 106,  text: '逾時收訊'},
        Timeout107: {value: 107,  text: '逾時收訊'},
        MessageAccepted: {value: 1000,  text: '傳送中'},
        DeliveredToTerminal: {value: 1001,  text: '發送成功'},
        DeliveryUncertain: {value: 1002,  text: '逾時收訊'},
        DeliveryImpossible: {value: 1003,  text: '逾時收訊'},
        MessageWaiting: {value: 1004,  text: '逾時收訊'},
        DeliveredToNetwork: {value: 1005,  text: '逾時收訊'},
        DeliveryReportTimeout: {value: 1006,  text: '逾時收訊'},
        Unknown: {value: -999,  text: '未定義'},
        Timeout_203: {value: -203,  text: '逾時收訊'},
        Timeout_202: {value: -202,  text: '逾時收訊'},
        Timeout_201: {value: -201,  text: '逾時收訊'},
        Timeout_101: {value: -101,  text: '逾時收訊'},
        Timeout_100: {value: -100,  text: '逾時收訊'},
        Timeout_32: {value: -32,  text: '逾時收訊'},
        Timeout_9: {value: -9,  text: '逾時收訊'},
        Timeout_8: {value: -8,  text: '逾時收訊'},
        Timeout_6: {value: -6,  text: '逾時收訊'},
        Timeout_5: {value: -5,  text: '逾時收訊'},
        Timeout_4: {value: -4,  text: '逾時收訊'},
        WrongPhoneNumber: {value: -3,  text: '電話號碼格式錯誤'},
        Timeout_2: {value: -2,  text: '逾時收訊'},
        Timeout_1: {value: -1,  text: '逾時收訊'},
    };
    angular.module('app').constant('DeliveryReportStatus', DeliveryReportStatus);

    /**
     * 發送線路
     */
    var SmsProviderType = {
        InfobipNormalQuality: {value: 0,  text: '一般 Infobip'},
        InfobipHighQuality: {value: 1,  text: '高品質 Infobip'},
        Every8d: {value: 2,  text: 'Every8d'},
    };
    angular.module('app').constant('SmsProviderType', SmsProviderType);

    /**
     * 發送簡訊狀態(通用型-用於寫入SendMessageHistory)
     */
    var MessageStatus = {
        Unknown: {value: 0,  text: 'Unknown'},
        MessageAccepted: {value: 1,  text: 'MessageAccepted'},
        MessageNotSent: {value: 2,  text: 'MessageNotSent'},
        MessageSent: {value: 3,  text: 'MessageSent'},
        MessageWaitingForDelivery: {value: 4,  text: 'MessageWaitingForDelivery'},
        MessageNotDelivered: {value: 5,  text: 'MessageNotDelivered'},
        MessageDelivered: {value: 6,  text: 'MessageDelivered'},
        NetworkNotAllowed: {value: 7,  text: 'NetworkNotAllowed'},
        NetworkNotAvailable: {value: 8,  text: 'NetworkNotAvailable'},
        InvalidDestinationAddress: {value: 9,  text: 'InvalidDestinationAddress'},
        MessageDeliveryUnknown: {value: 10,  text: 'MessageDeliveryUnknown'},
        RouteNotAvailable: {value: 11,  text: 'RouteNotAvailable'},
        InvalidSourceAddress: {value: 12,  text: 'InvalidSourceAddress'},
        NotEnoughCredits: {value: 13,  text: 'NotEnoughCredits'},
        MessageRejected: {value: 14,  text: 'MessageRejected'},
        MessageExpired: {value: 15,  text: 'MessageExpired'},
        SystemError: {value: 16,  text: 'SystemError'},
    };
    angular.module('app').constant('MessageStatus', MessageStatus);

    /**
     * 簡訊規則目前狀態
     */
    var SendMessageRuleStatus = {
        Prepare: {value: 0,  text: '正在建立簡訊規則以及相關資料'},
        Ready: {value: 1,  text: '簡訊規則以及相關資料已經備妥，可以準備發送'},
        Sending: {value: 2,  text: '正在發送簡訊規則'},
        Sent: {value: 3,  text: '簡訊規則已發送完畢'},
        Finish: {value: 4,  text: '簡訊規則發送任務已完成'},
        Updating: {value: 5,  text: '簡訊規則正在更新'},
        Deleting: {value: 6,  text: '簡訊規則正在刪除'},
    };
    angular.module('app').constant('SendMessageRuleStatus', SendMessageRuleStatus);

    /**
     * 性別
     */
    var Gender = {
        Unknown: {value: 0,  text: '不詳'},
        Male: {value: 1,  text: '男性'},
        Female: {value: 2,  text: '女性'},
    };
    angular.module('app').constant('Gender', Gender);

    /**
     * 周期簡訊發送時間類型
     */
    var SendCycleType = {
        EveryDay: {value: 0,  text: '每天發送'},
        EveryWeek: {value: 1,  text: '每周發送'},
        EveryMonth: {value: 2,  text: '每月發送'},
        EveryYear: {value: 3,  text: '每年發送'},
    };
    angular.module('app').constant('SendCycleType', SendCycleType);

    /**
     * 搜尋類型
     */
    var SearchType = {
        Department: {value: 1,  text: '依部門搜尋'},
        Member: {value: 2,  text: '依成員搜尋'},
    };
    angular.module('app').constant('SearchType', SearchType);

    /**
     * 下載類型
     */
    var DownloadType = {
        Unknown: {value: 0,  text: '未知'},
        Statistic: {value: 1,  text: '統計表'},
        All: {value: 2,  text: '全部'},
    };
    angular.module('app').constant('DownloadType', DownloadType);

    /**
     * 交易類別
     */
    var TradeType = {
        All: {value: 0,  text: '全部'},
        DeductionOfSendMessage: {value: 1,  text: '發送扣點'},
        CoverOfSendMessage: {value: 2,  text: '發送回補'},
        Deposit: {value: 3,  text: '儲值'},
        Cover: {value: 4,  text: '回補'},
        ExportPoints: {value: 5,  text: '點數匯出'},
        ImportPoints: {value: 6,  text: '點數匯入'},
        ExportRecoveryPoints: {value: 7,  text: '回收點數匯出'},
        ImportRecoveryPoints: {value: 8,  text: '回收點數匯入'},
    };
    angular.module('app').constant('TradeType', TradeType);

    /**
     * LogLevel
     */
    var LogLevel = {
        Debug: {value: 0,  text: 'Debug'},
        Info: {value: 1,  text: 'Info'},
        Warn: {value: 2,  text: 'Warn'},
        Error: {value: 3,  text: 'Error'},
        All: {value: 4,  text: 'All'},
    };
    angular.module('app').constant('LogLevel', LogLevel);

    /**
     * 發送訊息類型
     */
    var SendMessageType = {
        SmsMessage: {value: 0,  text: 'SMS'},
        AppMessage: {value: 1,  text: 'App'},
    };
    angular.module('app').constant('SendMessageType', SendMessageType);

    /**
     * 行動電話(國碼)
     */
    var MobileCountry = {
        Chinese: {value: 86,  text: '中國大陸'},
        Taiwan: {value: 886,  text: '台灣'},
    };
    angular.module('app').constant('MobileCountry', MobileCountry);

    /**
     * 簡訊接收者類型
     */
    var RecipientFromType = {
        FileUpload: {value: 0,  text: '載入大量名單'},
        CommonContact: {value: 1,  text: '常用聯絡人'},
        GroupContact: {value: 2,  text: '由聯絡人(群組)選取'},
        ManualInput: {value: 3,  text: '手動輸入'},
    };
    angular.module('app').constant('RecipientFromType', RecipientFromType);

    /**
     * 角色
     */
    var Role = {
        Unknown: {value: 0,  text: '尚未定義'},
        Employee: {value: 1,  text: '員工'},
        DepartmentHead: {value: 10,  text: '部門主管'},
        Supervisor: {value: 100,  text: '督導者'},
        Administrator: {value: 1000,  text: '系統管理者'},
    };
    angular.module('app').constant('Role', Role);

    /**
     * 單向|雙向 簡訊發送
     */
    var SendCustType = {
        OneWay: {value: 0,  text: '單向'},
        TwoWay: {value: 1,  text: '雙向'},
    };
    angular.module('app').constant('SendCustType', SendCustType);

    /**
     * 簡訊發送時間類型
     */
    var SendTimeType = {
        Immediately: {value: 0,  text: '立即發送簡訊'},
        Deliver: {value: 1,  text: '預約發送簡訊'},
        Cycle: {value: 2,  text: '周期發送簡訊'},
    };
    angular.module('app').constant('SendTimeType', SendTimeType);

    /**
     * 上傳檔案類型
     */
    var UploadedFileType = {
        SendMessage: {value: 0,  text: '簡訊發送'},
        SendParamMessage: {value: 1,  text: '參數簡訊發送'},
        Contact: {value: 2,  text: '聯絡人'},
        Blacklist: {value: 3,  text: '黑名單'},
    };
    angular.module('app').constant('UploadedFileType', UploadedFileType);

    /**
     * EfJobQueueStatus
     */
    var EfJobQueueStatus = {
        Enqueued: {value: 0,  text: 'Enqueued'},
    };
    angular.module('app').constant('EfJobQueueStatus', EfJobQueueStatus);

    /****************************************
     * 所有 ENUM 定義，用於 EnumFilter
     ****************************************/
    var EnumMapping = {
        'DeliveryReportStatus': DeliveryReportStatus, // 簡訊派送結果狀態(通用型-用於寫入SendMessageHistory)
        'SmsProviderType': SmsProviderType, // 發送線路
        'MessageStatus': MessageStatus, // 發送簡訊狀態(通用型-用於寫入SendMessageHistory)
        'SendMessageRuleStatus': SendMessageRuleStatus, // 簡訊規則目前狀態
        'Gender': Gender, // 性別
        'SendCycleType': SendCycleType, // 周期簡訊發送時間類型
        'SearchType': SearchType, // 搜尋類型
        'DownloadType': DownloadType, // 下載類型
        'TradeType': TradeType, // 交易類別
        'LogLevel': LogLevel, // LogLevel
        'SendMessageType': SendMessageType, // 發送訊息類型
        'MobileCountry': MobileCountry, // 行動電話(國碼)
        'RecipientFromType': RecipientFromType, // 簡訊接收者類型
        'Role': Role, // 角色
        'SendCustType': SendCustType, // 單向|雙向 簡訊發送
        'SendTimeType': SendTimeType, // 簡訊發送時間類型
        'UploadedFileType': UploadedFileType, // 上傳檔案類型
        'EfJobQueueStatus': EfJobQueueStatus, // EfJobQueueStatus
    };
    angular.module('app').constant('EnumMapping', EnumMapping);

})(window, document);
