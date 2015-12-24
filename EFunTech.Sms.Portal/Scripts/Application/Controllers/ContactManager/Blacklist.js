(function (window, document) {
    'use strict';

    angular.module('app').controller('Blacklist', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'WebApi', '$translate', 'dialogs',
        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, WebApi, $translate, dialogs) {

            //========================================
            // Settings
            //========================================

            var crudApi = new CrudApi('api/Blacklist');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            $scope.gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
					{ name: 'Name', displayName: '姓名' },
					{ name: 'Mobile', displayName: '行動電話' },
                    { name: 'Region', displayName: '發送地區' },
                    {
                        name: 'Enabled',
                        displayName: '開啟/關閉',
                        cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.Enabled && "開啟" || "關閉" }}</div>',
                    },
					{ name: 'UpdatedUserName', displayName: '設定人' },
                    {
                        name: 'UpdatedTime',
                        displayName: '設定日期',
                        cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.UpdatedTime | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm" }}</div>',
                    },
					{ name: 'Remark', displayName: '備註' },
					{
					    name: 'Maintain',
					    width: '100',
					    displayName: '維護功能',
					    cellEditableCondition: false,
					    cellClass: 'grid-align-center',
					    cellTemplate: [
							'<img class="gg01" ng-click="grid.appScope.editRow(row.entity)"/>',
							'<img class="gg02" ng-click="grid.appScope.deleteRow(row.entity)"/>',
					    ].join('\n'),
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
                    SearchText: $scope.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria)
                .then(function (result) {
                    if ($scope.gridApi) $scope.gridApi.selection.clearSelectedRows();

                    var data = result.data;
                    $scope.gridOptions.totalItems = data.TotalCount;
                    $scope.gridOptions.data = data.Result;
                });
            };

            $scope.editRow = function (rowEntity) {
                var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                var isNew = !angular.isDefined(model.Id);
                model.Enabled = angular.isDefined(model.Enabled) ? model.Enabled : true; // set default value

                var schemaForm = SchemaFormFactory.create('BlackListModel');

                var title = isNew ? '新增黑名單' : '編輯黑名單';
                var schema = schemaForm.schema;
                var form = schemaForm.form;
                var options = {
                    validationMessage: {
                        202: '不符合欄位格式',
                        302: '此為必填欄位',
                    }
                };
                var validateBeforeSubmit = function (modalScope, form) { return true; };

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
                });
            };

            $scope.createRow = function (model) {
                crudApi.Create(model)
                .then(function () {
                    $scope.search();
                });
            };

            $scope.updateRow = function (model) {
                crudApi.Update(model)
                .then(function () {
                    $scope.search();
                });
            };

            $scope.deleteRow = function (rowEntity) {
                var selectedRow = rowEntity[0] || rowEntity;

                var message = "確定刪除『" + (selectedRow.Name || selectedRow.Mobile) + "』？";
                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    crudApi.Delete(selectedRow)
                    .then(function () {
                        $scope.search();
                    });
                });
            };

            $scope.deleteSelection = function () {
                var selectedRows = $scope.gridApi.selection.getSelectedRows();

                if (selectedRows.length == 0) {
                    dialogs.error('操作錯誤', '請先選擇欲刪除之資料');
                    return;
                }

                var message = "確定刪除這" + selectedRows.length + "筆資料？";
                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    crudApi.Delete(selectedRows)
                    .then(function () {
                        $scope.search();
                    });
                });
            };

            $scope.import = function () {
                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/uploadBlacklist.html',
                    controller: 'FileUploadCtrl',
                    windowClass: 'center-modal',
                    //size: size,
                    resolve: {
                        url: function () {
                            return '/FileManagerApi/UploadBlacklist';
                        },
                        extensionPattern: function () {
                            return /^(xlsx|csv|zip)$/i;
                        },
                        extraParameters: function () {
                            return {};
                        },
                    }
                });

                modalInstance.result.then(function (result) {
                    var extraParameters = result.extraParameters;
                    var uploadedFilename = result.uploadedFilename;
                    $scope.search();
                }, function () {
                    $log.debug('Modal dismissed at: ' + new Date());
                });
            };

            //========================================
            // Events & EventHandlers
            //========================================

            $scope.$on('tab.onSelect', function (event, tabName) {
                if (tabName !== 'Blacklist') return;
                $scope.search();
            });

            //========================================
            // Initialize
            //========================================

            $scope.searchText = '';
        }]);

})(window, document);
