(function (window, document) {
    'use strict';

    angular.module('app').service('SectorSendMessageStatisticManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {

            var self = this;
            var crudApi = new CrudApi('api/SectorSendMessageStatistic');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            var gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                // 部門	
                // 姓名	
                // 發送通數	
                // 發送扣點
                {
                    name: 'DepartmentName',
                    displayName: '部門'
                },
                {
                    name: 'FullName',
                    displayName: '姓名'
                },
                {
                    name: 'TotalReceiverCount',
                    displayName: '發送通數'
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

            this.init = function () {
                self.searchText = '';
                self.search();
            };

            this.gridApi = null;
            this.searchText = '';
            this.gridOptions = gridOptions;
        }]);

})(window, document);
