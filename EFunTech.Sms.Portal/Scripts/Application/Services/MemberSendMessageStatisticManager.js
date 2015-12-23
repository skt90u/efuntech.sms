(function (window, document) {
    'use strict';

    angular.module('app').service('MemberSendMessageStatisticManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {

            var self = this;
            var crudApi = new CrudApi('api/MemberSendMessageStatistic');
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
                //    displayName: '訊息類型',
                //    cellEditableCondition: false,
                //    cellClass: 'grid-align-center',
                //    cellTemplate: [
                //        '<img class="smsMessage" ng-show="row.entity.SendMessageType === 0" />',
                //        '<img class="appMessage" ng-show="row.entity.SendMessageType === 1" />',
                //    ].join('\n'),
                //},
                {
                    name: 'SendTime',
                    displayName: '發送時間',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.SendTime | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
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
                    name: 'TotalReceiverCount',
                    displayName: '發送通數'
                },
                {
                    name: 'TotalSuccess',
                    displayName: '成功接收'
                },
                {
                    name: 'TotalSending',
                    displayName: '傳送中'
                },
                {
                    name: 'TotalTimeout',
                    //displayName: '逾期收訊'
                    displayName: '傳送失敗'
                },
                {
                    name: 'TotalMessageCost',
                    displayName: '發送扣點'
                },
				{
				    name: 'Maintain',
				    width: '50',
				    displayName: '明細',
				    cellEditableCondition: false,
				    cellClass: 'grid-align-center',
				    cellTemplate: [
						'<img class="details" ng-click="grid.appScope.searchHistory(row.entity)"/>',
				    ].join('\n'),
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

            this.selectColumns = function () {

                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/columnEditor.html',
                    controller: 'ColumnEditorCtrl',
                    windowClass: 'center-modal',
                    //size: size,
                    resolve: {
                        options: function () {
                            return {
                                title: '自訂資料顯示欄位',
                                maxColumns: self.gridOptions.columnDefs.length,
                                columnDefs: self.gridOptions.columnDefs,
                                exclusiveNames: [], //['Name'],
                            };
                        },
                    }
                });

                modalInstance.result.then(function (xxx) {
                    self.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
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
