(function (window, document) {
    'use strict';

    angular.module('app').service('SendMessageHistoryManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {

            var self = this;
            var crudApi = new CrudApi('api/SendMessageHistory');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            var gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                // 編號	
                // 簡訊類別	
                // 部門	
                // 姓名	
                // 帳號
                {
                    name: 'RowNo',
                    displayName: '編號'
                },
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
                // 門號	
                // 發送時間	
                // 收訊時間	
                // 簡訊類別描述	
                // 發送內容	
                {
                    name: 'DestinationAddress',
                    displayName: '門號'
                },
                {
                    name: 'Region',
                    displayName: '發送地區'
                },
                {
                    name: 'SendTime',
                    displayName: '發送時間'
                },
                {
                    name: 'SentDate',
                    displayName: '收訊時間'
                },
                {
                    name: 'SendTitle',
                    displayName: '簡訊類別描述'
                },
                {
                    name: 'SendBody',
                    displayName: '發送內容'
                },
                // 狀態	
                // 發送扣點
                {
                    name: 'DeliveryStatusString',
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

            this.search = function () {
                var criteria = {
                    SearchText: self.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria)
                .then(function (result) {
                    if (self.gridApi)self.gridApi.selection.clearSelectedRows();

                    var data = result.data;
                    self.gridOptions.totalItems = data.TotalCount;
                    self.gridOptions.data = data.Result;
                });
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
