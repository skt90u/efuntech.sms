(function (window, document) {
    'use strict';

    angular.module('app').controller('SectorStatistics', ['$scope', 'DateUtil',
        'SelectUtil', 'SearchType', 'DownloadType',
        'SectorSendMessageStatisticManager', 'SectorSendMessageHistoryManager', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs',
    function ($scope, DateUtil,
        SelectUtil, SearchType, DownloadType,
        SectorSendMessageStatisticManager, SectorSendMessageHistoryManager, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs) {

        //========================================
        // Settings
        //========================================

        var SearchTypeOptions = SelectUtil.getEnumOptions(SearchType);
        var DownloadTypeOptions = SelectUtil.getEnumOptions(DownloadType);

        var getDateTime = DateUtil.getDateTime;
        var getDateBegin = DateUtil.getDateBegin;
        var getDateEnd = DateUtil.getDateEnd;

        $scope.SearchTypeOptions = SearchTypeOptions;
        $scope.DownloadTypeOptions = DownloadTypeOptions;

        $scope.SectorSendMessageStatisticManager = SectorSendMessageStatisticManager;
        $scope.SectorSendMessageHistoryManager = SectorSendMessageHistoryManager;

        $scope.SearchTypes = SearchType;

        $scope.Criteria = {
            SearchType: SearchTypeOptions.Department,
            DownloadType: DownloadTypeOptions.Statistic,
            DepartmentIds: '',
            UserIds: '',
            StartDate: new Date(),
            EndDate: new Date(),

            StartDateOpend: false,
            EndDateOpend: false,
        };

        $scope.selectedDepartmentUsers = [];

        $scope.ShowStatistic = true;

        //========================================
        // Functions
        //========================================

        $scope.OnChangeSearchType = function (SearchType) {
            if (SearchType == SearchTypeOptions.Department) {
                $scope.Criteria.UserIds = '';
                $scope.selectedDepartmentUsers = [];
            }
            if (SearchType == SearchTypeOptions.Member) {
                $scope.Criteria.DepartmentIds = '';
            }
        };

        $scope.PickupMember = function () {

            var modalInstance = $modal.open({
                templateUrl: 'template/modal/memberSelector.html',
                controller: 'MemberSelectorCtrl',
                windowClass: 'center-modal',
            });

            modalInstance.result.then(function (selectedRows) {
                $scope.selectedDepartmentUsers = selectedRows;
                $scope.Criteria.UserIds = _.pluck(selectedRows, 'Id').join(',');
            });

        };

        $scope.searchHistory = function (rowEntity) {

            $scope.CurrentRowEntity = rowEntity;

            $scope.ShowStatistic = false;

            $scope.SectorSendMessageHistoryManager.search({
                UserId: rowEntity.CreatedUserId,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
            });
        };

        $scope.searchStatistic = function () {

            $scope.ShowStatistic = true;

            $scope.SectorSendMessageStatisticManager.search({
                SearchType: $scope.Criteria.SearchType,
                DownloadType: $scope.Criteria.DownloadType,
                DepartmentIds: $scope.Criteria.DepartmentIds,
                UserIds: $scope.Criteria.UserIds,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
            });
        };

        $scope.search = function () {
            $scope.searchStatistic();
        };
        
        /*
         * 匯出部門通數統計
         */
        $scope.export1 = function () {

            $scope.SectorSendMessageStatisticManager.export({
                SearchType: $scope.Criteria.SearchType,
                DownloadType: DownloadTypeOptions.Statistic,
                DepartmentIds: $scope.Criteria.DepartmentIds,
                UserIds: $scope.Criteria.UserIds,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
            });

        };

        /*
         * 匯出部門發送紀錄
         */
        $scope.export2 = function () {

            $scope.SectorSendMessageStatisticManager.export({
                SearchType: $scope.Criteria.SearchType,
                DownloadType: DownloadTypeOptions.All,
                DepartmentIds: $scope.Criteria.DepartmentIds,
                UserIds: $scope.Criteria.UserIds,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
            });

        };

        /*
         * 匯出個人發送紀錄
         */
        $scope.export3 = function () {
            $scope.SectorSendMessageHistoryManager.export({
                UserId: $scope.CurrentRowEntity.CreatedUserId,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
            });
        };

        //========================================
        // Events & EventHandlers
        //========================================

        //========================================
        // Initialize
        //========================================
        
        if (GlobalSettings.isSPA) {
            $scope.$on('menu.onSelect', function (event, menuName) {
                if (menuName !== 'SectorStatistics') return;
                $scope.search();
            });
        }
        else {
            $scope.search();
        }
        

    }]);

})(window, document);