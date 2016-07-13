(function (window, document) {
    'use strict';

    angular.module('app').service('MemberSendMessageHistoryManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {

            var self = this;
            var crudApi = new CrudApi('api/MemberSendMessageHistory');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            // 收訊者姓名	收訊者門號	收訊內容	收訊狀態	收訊時間	發送扣點
            var gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                {
                    name: 'DestinationName',
                    displayName: '收訊者姓名'
                },
                {
                    name: 'DestinationAddress',
                    displayName: '收訊者門號'
                },
                {
                    name: 'Email',
                    displayName: '收訊者信箱'
                },
                {
                    name: 'Region',
                    displayName: '發送地區'
                },
                {
                    name: 'SendBody',
                    displayName: '收訊內容'
                },
                {
                    name: 'DeliveryStatusString',
                    displayName: '收訊狀態'
                },
                {
                    name: 'SentDate',
                    displayName: '收訊時間',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.SentDate | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
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
            //this.extraCriteria = {
            //    SendMessageQueueId: 123,
            //    ReceiptStatus: '123|234|345',
            //};

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
