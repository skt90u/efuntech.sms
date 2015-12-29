(function (window, document) {
    'use strict';

    angular.module('app').controller('SystemAnnouncement', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs', 'DateUtil',
        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs, DateUtil) {

        var getDateTime = DateUtil.getDateTime;
        var getDateBegin = DateUtil.getDateBegin;
        var getDateEnd = DateUtil.getDateEnd;
        var toLocalTime = DateUtil.toLocalTime;
        var toDate = DateUtil.toDate;

        //========================================
        // Settings
        //========================================

        var crudApi = new CrudApi('api/SystemAnnouncement');
        var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

        $scope.gridOptions = {
            // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
            paginationPageSizes: paginationOptions.pageSizes,
            paginationPageSize: paginationOptions.pageSize,
            useExternalPagination: true,
            useExternalSorting: true,
            enableColumnMenus: false,
            columnDefs: [
            //{
            //    name: 'IsVisible',
            //    displayName: '',
            //    cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.IsVisible ? "顯示公告" : "隱藏公告" }}</div>',
            //    width:'90',
            //},
            {
                name: 'PublishDate',
                width: '130',
                displayName: '公告日期',
                cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.PublishDate | UtcToLocalTimeFilter: "YYYY/MM/DD" }}</div>',
            },
            {
                name: 'Announcement',
                displayName: '公告內容'
            },
            //{
            //    name: 'CreatedTime',
            //    displayName: '更新時間',
            //    cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.CreatedTime | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
            //},
            {
                name: 'Update',
                width: '50',
                displayName: '',
                cellEditableCondition: false,
                cellClass: 'grid-align-center',
                cellTemplate: '<img class="edit" ng-click="grid.appScope.editRow(row.entity)"/>'
            },
            {
                name: 'Delete',
                width: '50',
                displayName: '',
                cellEditableCondition: false,
                cellClass: 'grid-align-center',
                cellTemplate: '<img class="lightbulb_48" ng-click="grid.appScope.deleteRow(row.entity)"/>'
            },
            ],
            data: [],
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;

                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    paginationOptions.pageNumber = newPage;
                    paginationOptions.pageSize = pageSize;
                    $scope.search();
                });
            }
        };

        //========================================
        // Functions
        //========================================

        $scope.search = function () {
            var criteria = {
                SearchText: $scope.Criteria.SearchText,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
                PageIndex: paginationOptions.pageNumber,
                PageSize: paginationOptions.pageSize
            };

            crudApi.GetAll(criteria).then(function (result) {
                if ($scope.gridApi) $scope.gridApi.selection.clearSelectedRows();

                var data = result.data;
                $scope.gridOptions.totalItems = data.TotalCount;
                $scope.gridOptions.data = data.Result;
            });
        };

        $scope.export = function () {

            var criteria = {
                SearchText: $scope.Criteria.SearchText,
                StartDate: getDateBegin($scope.Criteria.StartDate),
                EndDate: getDateEnd($scope.Criteria.EndDate),
                PageIndex: paginationOptions.pageNumber,
                PageSize: paginationOptions.pageSize
            };

            crudApi.Download(criteria);
        };

        $scope.editRow = function (rowEntity) {

            var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
            var isNew = !angular.isDefined(model.Id);
            if (isNew) {
                // TODO: 設定新增資料的初始值
                model.IsVisible = true;
                model.PublishDate = getDateBegin(new Date());
                model.Announcement = '';
            }
            else {
                model.PublishDate = toLocalTime(model.PublishDate);
            }

            var schemaForm = SchemaFormFactory.create('SystemAnnouncementModel', {
                isNew: isNew
            });

            var title = isNew ? '新增資料' : '編輯資料';
            var schema = schemaForm.schema;
            var form = schemaForm.form;
            var options = {
                validationMessage: {
                    202: '不符合欄位格式',
                    302: '此為必填欄位',
                }
            };
            var validateBeforeSubmit = function (modalScope, form) {
                return true;
            };

            var modalInstance = $modal.open({
                templateUrl: 'template/modal/editRow.html',
                controller: 'FormModalCtrl',
                windowClass: 'center-modal',
                //size: size,
                resolve: {
                    options: function () {
                        return {
                            title: title,
                            schema: schema,
                            form: form,
                            model: model,
                            options: options,
                            validateBeforeSubmit: validateBeforeSubmit,
                        };
                    },
                }
            });

            modalInstance.result.then(function (savedModel) {
                if (!savedModel) return;
                var isNew = !angular.isDefined(savedModel.Id);
                if (isNew) {
                    $scope.createRow(savedModel);
                }
                else {
                    $scope.updateRow(savedModel);
                }
            },
            function () {
                $log.debug('Modal dismissed at: ' + new Date());
            });
        };

        $scope.createRow = function (model) {

            model.PublishDate = getDateBegin(toDate(model.PublishDate));
            model.IsVisible = true;

            crudApi.Create(model).then(function () {
                $scope.search();
            });
        };

        $scope.updateRow = function (model) {

            model.PublishDate = getDateBegin(toDate(model.PublishDate));
            model.IsVisible = true;

            crudApi.Update(model).then(function () {
                $scope.search();
            });
        };

        $scope.deleteRow = function (rowEntity) {
            var model = rowEntity[0] || rowEntity;

            var message = "確定刪除此筆公告？";
            dialogs.confirm('刪除資料', message).result.then(function (btn) {
                crudApi.Delete(model).then(function () {
                    $scope.search();
                });
            });
        };

        $scope.deleteSelection = function () {
            var selectedRows = $scope.gridApi.selection.getSelectedRows();

            if (selectedRows.length == 0) {
                dialogs.error('刪除資料', '請先選擇欲刪除之資料');
                return;
            }

            var message = "確定刪除這" + selectedRows.length + "筆資料？";
            dialogs.confirm('刪除資料', message).result.then(function (btn) {
                crudApi.Delete(selectedRows).then(function () {
                    $scope.search();
                });
            });
        };

        //========================================
        // Events & EventHandlers
        //========================================

        $scope.$on('tab.onSelect', function (event, tabName) {
            if (tabName !== 'SystemAnnouncement') return;
            $scope.search();
        });

        //========================================
        // Initialize
        //========================================

        $scope.Criteria = {
            SearchText: '',
            StartDate: new Date(),
            EndDate: new Date(),

            StartDateOpend: false,
            EndDateOpend: false,
        };

        if (GlobalSettings.isSPA) {
            $scope.$on('menu.onSelect', function (event, menuName) {
                if (menuName !== 'Others') return;
                $scope.search();
            });
        }
        else {
            $scope.search();
        }

    }]);

})(window, document);