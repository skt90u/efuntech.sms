(function (window, document) {
    'use strict';

    angular.module('app').service('SectorSendMessageHistoryManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {

            var self = this;
            var crudApi = new CrudApi('api/SectorSendMessageHistory');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            var gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                //{
                //    name: 'RowNo',
                //    displayName: '編號'
                //},
                //{
                //    name: 'SendMessageType',
                //    width: '80',
                //    displayName: '簡訊類別',
                //    cellEditableCondition: false,
                //    cellClass: 'grid-align-center',
                //    cellTemplate: [
                //        '<img class="smsMessage" ng-show="row.entity.SendMessageType === 0" />',
                //        '<img class="appMessage" ng-show="row.entity.SendMessageType === 1" />',
                //    ].join('\n'),
                //},
                {
                    name: 'DepartmentName',
                    displayName: '部門'
                },
                {
                    name: 'FullName',
                    displayName: '姓名'
                },
                {
                    name: 'UserName',
                    displayName: '帳號'
                },
                {
                    name: 'DestinationAddress',
                    displayName: '門號'
                },
                {
                    name: 'SendTime',
                    displayName: '發送時間',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.SendTime | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
                },
                {
                    name: 'SentDate',
                    displayName: '收訊時間',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.SentDate | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
                },
                {
                    name: 'SendTitle',
                    displayName: '簡訊類別描述'
                },
                {
                    name: 'SendBody',
                    displayName: '發送內容'
                },
                {
                    name: 'DeliveryStatusChineseString',
                    displayName: '狀態'
                },
                {
                    name: 'MessageCost',
                    displayName: '發送扣點'
                },
                ],
                data: [],
                onRegisterApi: function (gridApi) {
                    self.gridApi = gridApi;

                    gridApi.pagination.on.paginationChanged(null /* 給 null 也沒關係， 我想他是用 scope 做 listener */, function (newPage, pageSize) {
                        paginationOptions.pageNumber = newPage;
                        paginationOptions.pageSize = pageSize;
                        self.search();
                    });
                }
            };

            this.extraCriteria = {};

            this.search = function (extraCriteria) {

                self.extraCriteria = extraCriteria || self.extraCriteria;

                var criteria = angular.extend({}, {
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                }, self.extraCriteria);

                crudApi.GetAll(criteria)
                .then(function (result) {
                    var data = result.data;
                    self.gridOptions.totalItems = data.TotalCount;
                    self.gridOptions.data = data.Result;
                });
            };

            this.export = function (extraCriteria) {
                var criteria = angular.extend({}, {
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize,
                }, extraCriteria);

                crudApi.Download(criteria);
            };

            this.init = function () {
                self.searchText = '';
                self.search();
            };

            this.gridApi = null;
            this.searchText = '';
            this.gridOptions = gridOptions;
        }]);

})(window, document);
