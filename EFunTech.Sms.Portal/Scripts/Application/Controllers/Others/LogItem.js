(function (window, document) {
    'use strict';

    angular.module('app').controller('LogItem', ['$scope', 'SelectUtil', 'LogLevel', 'DateUtil', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs',
        function ($scope, SelectUtil, LogLevel, DateUtil, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs) {

        var getDateTime = DateUtil.getDateTime;
        var getDateBegin = DateUtil.getDateBegin;
        var getDateEnd = DateUtil.getDateEnd;
        var LogLevelOptions = SelectUtil.getEnumOptions(LogLevel);

        //========================================
        // Settings
        //========================================

        var crudApi = new CrudApi('api/LogItem');
        var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

        $scope.LogLevelOptions = LogLevelOptions;

        $scope.gridOptions = {
            // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
            paginationPageSizes: paginationOptions.pageSizes,
            paginationPageSize: paginationOptions.pageSize,
            useExternalPagination: true,
            useExternalSorting: true,
            enableColumnMenus: false,
            columnDefs: [
            {
                name: 'CreatedTime',
                displayName: '建立時間',
                cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.CreatedTime | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
            },

            {
                name: 'Host',
                displayName: 'Host'
            },
            {
                name: 'UserName',
                displayName: '建立者'
            },

            //{
            //    name: 'EntryAssembly',
            //    displayName: '主要執行的專案名稱'
            //},
            //{
            //    name: 'Class',
            //    displayName: '類別'
            //},
            //{
            //    name: 'Method',
            //    displayName: '函式'
            //},
            {
                name: 'Class_Method',
                displayName: '函式',
                cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Class + "." + row.entity.Method }}</div>',
            },

            {
                name: 'LogLevel',
                displayName: '層級',
                cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.LogLevel | EnumFilter: "LogLevel" }}</div>',
            },
            {
                name: 'Message',
                displayName: '訊息內容'
            },
            {
                name: 'StackTrace',
                displayName: '呼叫堆疊'
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
                LogLevel: $scope.Criteria.LogLevel,
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
                LogLevel: $scope.Criteria.LogLevel,
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
            }

            var schemaForm = SchemaFormFactory.create('LogItemModel', {
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
            crudApi.Create(model).then(function () {
                $scope.search();
            });
        };

        $scope.updateRow = function (model) {
            crudApi.Update(model).then(function () {
                $scope.search();
            });
        };

        $scope.deleteRow = function (rowEntity) {
            var model = rowEntity[0] || rowEntity;

            var message = "確定刪除選取紀錄？";
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


        $scope.selectColumns = function () {

            var modalInstance = $modal.open({
                templateUrl: 'template/modal/columnEditor.html',
                controller: 'ColumnEditorCtrl',
                windowClass: 'center-modal',
                //size: size,
                resolve: {
                    options: function () {
                        return {
                            title: '自訂資料顯示欄位',
                            maxColumns: $scope.gridOptions.columnDefs.length,
                            columnDefs: $scope.gridOptions.columnDefs,
                            exclusiveNames: [], //['Name'],
                        };
                    },
                }
            });

            modalInstance.result.then(function (xxx) {
                $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            });
        };


        //========================================
        // Events & EventHandlers
        //========================================

        $scope.$on('tab.onSelect', function (event, tabName) {
            if (tabName !== 'LogItem') return;
            $scope.search();
        });

        //========================================
        // Initialize
        //========================================

        $scope.Criteria = {
            SearchText: '',
            StartDate: new Date(),
            EndDate: new Date(),
            LogLevel: LogLevelOptions.All,

            StartDateOpend: false,
            EndDateOpend: false,
        };

        $scope.search();

    }]);

})(window, document);