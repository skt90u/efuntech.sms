(function (window, document) {
    'use strict';

    angular.module('app').controller('MemberSendMessage', ['$scope', 'DateUtil', 'MemberSendMessageStatisticManager', 'MemberSendMessageHistoryManager', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs', 'DeliveryReportStatus',
    function ($scope, DateUtil, MemberSendMessageStatisticManager, MemberSendMessageHistoryManager, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs, DeliveryReportStatus) {

        //var DeliveryReportStatusOptions = {
        //    Sending: "1004,1005,0", // 傳送中
        //    Sent: "1001,100", // 成功
        //    PhoneNumberNotAvailable: "103", // 空號
        //    WrongPhoneNumber: "-3", // 電話號碼格式錯誤
        //    Timeout: "1002,1003,101,102,104,105,106,107,-1,-2,-4,-5,-6,-8,-9,-32,-100,-101,-201,-202,-203" // 逾時
        //};

        //var DeliveryReportStatusOptions = {
        //    Sending: "1000,0", // 傳送中
        //    Sent: "1001,100", // 成功
        //    PhoneNumberNotAvailable: "103", // 空號
        //    WrongPhoneNumber: "-3", // 電話號碼格式錯誤
        //    Timeout: "1002,1003,1004,1005,1006,101,102,104,105,106,107,-1,-2,-4,-5,-6,-8,-9,-32,-100,-101,-201,-202,-203" // 逾時
        //};

        function getOptionValue(text) {
            var matchs = _.filter(DeliveryReportStatus, function (prop) {
                return prop.text === text;
            });
            return _.pluck(matchs, 'value').join(',')
        }

        var DeliveryReportStatusOptions = {
            Sending: getOptionValue('傳送中'), 
            Sent: getOptionValue('發送成功'),
            PhoneNumberNotAvailable: getOptionValue('空號'),
            WrongPhoneNumber: getOptionValue('電話號碼格式錯誤'),
            Timeout: getOptionValue('逾時收訊'),
        };

        //========================================
        // Settings
        //========================================

        var getDateTime = DateUtil.getDateTime;
        var getDateBegin = DateUtil.getDateBegin;
        var getDateEnd = DateUtil.getDateEnd;

        $scope.MemberSendMessageStatisticManager = MemberSendMessageStatisticManager;
        $scope.MemberSendMessageHistoryManager = MemberSendMessageHistoryManager;
        
        $scope.CurrentRowEntity = null;

        $scope.ShowStatistic = true;

        //========================================
        // Functions
        //========================================

        $scope.searchHistory = function (rowEntity) {
            
            $scope.CurrentRowEntity = rowEntity;

            $scope.ShowStatistic = false;

            $scope.MemberSendMessageHistoryManager.search({
                SendMessageQueueId: $scope.CurrentRowEntity.SendMessageQueueId,
                ReceiptStatus: $scope.ReceiptStatus,
            });
        };

        $scope.searchStatistic = function () {

            $scope.ShowStatistic = true;

            $scope.MemberSendMessageStatisticManager.search({
                StartDate: getDateBegin($scope.StartDate),
                EndDate: getDateEnd($scope.EndDate),
                Mobile: $scope.Mobile,
                ReceiptStatus: $scope.ReceiptStatus,
            });
        };

        $scope.search = function () {
            $scope.searchStatistic();
        };
        
        $scope.hideHistory = function () {
            $scope.ShowStatistic = true;
        };

        $scope.exportStatistic = function (extraCriteria) {
            $scope.MemberSendMessageStatisticManager.export({
                StartDate: getDateBegin($scope.StartDate),
                EndDate: getDateEnd($scope.EndDate),
                Mobile: $scope.Mobile,
                ReceiptStatus: $scope.ReceiptStatus,
            });
        };

        $scope.exportHistory = function (extraCriteria) {
            $scope.MemberSendMessageHistoryManager.export({
                SendMessageQueueId: $scope.CurrentRowEntity.SendMessageQueueId,
                ReceiptStatus: $scope.ReceiptStatus,
            });
        };

        $scope.selectStatisticColumns = function () {
            $scope.MemberSendMessageStatisticManager.selectColumns();
        };

        $scope.selectHistoryColumns = function () {
            $scope.MemberSendMessageHistoryManager.selectColumns();
        };

        //========================================
        // Events & EventHandlers
        //========================================

        $scope.$on('tab.onSelect', function (event, tabName) {
            if (tabName !== 'MemberSendMessage') return;
            $scope.search();
        });

        //========================================
        // Initialize
        //========================================
        
        $scope.StartDate = new Date();
        $scope.StartDateOpend = false;
        $scope.EndDate = new Date();
        $scope.EndDateOpend = false;

        $scope.EnableMobile = false;
        $scope.Mobile = '';
        $scope.ToggleMobile = function () {
            if (!$scope.EnableMobile) {
                $scope.Mobile = '';
            }
        };

        $scope.ReceiptStatusOptions = [
            { value: DeliveryReportStatusOptions.Sending, text: '傳送中' },
            { value: DeliveryReportStatusOptions.Sent, text: '成功' },
            { value: DeliveryReportStatusOptions.PhoneNumberNotAvailable, text: '空號' },
            { value: DeliveryReportStatusOptions.WrongPhoneNumber, text: '電話號碼格式錯誤' },
            //{ value: DeliveryReportStatusOptions.Timeout, text: '逾時' },
            { value: DeliveryReportStatusOptions.Timeout, text: '傳送失敗' },
        ];

        $scope.EnableReceiptStatus = false;
        $scope.ReceiptStatus = '';
        $scope.ReceiptStatusOptionSelection = [];
        $scope.ToggleReceiptStatus = function () {
            if (!$scope.EnableReceiptStatus) {
                $scope.ReceiptStatus = '';
                $scope.ReceiptStatusOptionSelection = [];
            }
        };

        $scope.ShowStatistic = true;

        $scope.ClickReceiptStatusOption = function (ReceiptStatusOption) {
            if (_.includes($scope.ReceiptStatusOptionSelection, ReceiptStatusOption)) {
                $scope.ReceiptStatusOptionSelection = _.without($scope.ReceiptStatusOptionSelection, ReceiptStatusOption);
            }
            else {
                $scope.ReceiptStatusOptionSelection.push(ReceiptStatusOption);
            }

            var array = [];
            for (var i = 0, len = $scope.ReceiptStatusOptionSelection.length; i < len; i++) {
                var tokenSeparators = GlobalSettings.tokenSeparators;
                var tokens = $scope.ReceiptStatusOptionSelection[i].value.split(new RegExp(tokenSeparators.join('|'), 'g'));
                array = array.concat(tokens);
            }
            $scope.ReceiptStatus = array.join(',');
        };

        if (GlobalSettings.isSPA) {
            $scope.$on('menu.onSelect', function (event, menuName) {
                if (menuName !== 'SearchMemberSend') return;
                var tabName = $scope.$parent.tabName;
                if (tabName !== 'MemberSendMessage') return;
                $scope.search();
            });
        }
        
        // 載入完成後，需直接呼叫，避免直接透過 hashbang 連結系統設定功能，導致功能不正常。
        $scope.search();

    }]);

})(window, document);