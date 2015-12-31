(function (window, document) {

    angular.module('app').controller('TradeDetail', ['$scope', 'CurrentUserManager', 'DateUtil',
        'SelectUtil', 'TradeType',
        '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs',
        function ($scope, CurrentUserManager, DateUtil,
            SelectUtil, TradeType,
            $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs) {

            //========================================
            // Settings
            //========================================

            var getDateTime = DateUtil.getDateTime;
            var getDateBegin = DateUtil.getDateBegin;
            var getDateEnd = DateUtil.getDateEnd;

            var crudApi = new CrudApi('api/TradeDetail');
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
                //    name: 'RowNo',
                //    displayName: '編號'
                //},
                {
                    name: 'TradeTime',
                    displayName: '交易時間',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.TradeTime | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
                },
                {
                    name: 'TradeType',
                    displayName: '交易類別',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.TradeType | EnumFilter: "TradeType" }}</div>',
                },
                {
                    name: 'Point',
                    displayName: '交易點數'
                },
                {
                    name: 'Remark',
                    displayName: '交易說明'
                },
                {
                    name: 'Delete',
                    width: '100',
                    displayName: '撤銷撥點',
                    cellEditableCondition: false,
                    cellClass: 'grid-align-center',
                    cellTemplate: '<img class="lightbulb_48" ng-if="row.entity.TradeType == grid.appScope.TradeTypeOptions.ExportPoints" ng-click="grid.appScope.deleteRow(row.entity)"/>'
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
                $scope.StartDate = getDateBegin($scope.StartDate);
                $scope.EndDate = getDateEnd($scope.EndDate);

                var errors = [];

                if ($scope.EndDate < $scope.StartDate) {
                    errors.push('結束時間不能小於起始時間');
                }

                if (errors.length != 0) {
                    var messages = _.map(errors, function (error, index) {
                        return '<li>' + error + '</li>';
                    });
                    dialogs.error('操作錯誤', '<ol>' + messages.join('') + '</ol>');
                    return false;
                }

                var criteria = {
                    StartDate: $scope.StartDate,
                    EndDate: $scope.EndDate,
                    TradeType: $scope.TradeType,
                    SearchText: $scope.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria).then(function (result) {
                    var data = result.data;
                    $scope.gridOptions.totalItems = data.TotalCount;
                    $scope.gridOptions.data = data.Result;
                });
            };

            $scope.editRow = function (rowEntity) {

                var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                var isNew = !angular.isDefined(model.Id);
                if (isNew) {
                    // TODO: 設定新增資料的初始值
                }

                var schemaForm = SchemaFormFactory.create('TradeDetailModel', {
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

                var message = "確定撤銷『" + model.Remark + "』？";
                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    crudApi.Delete(model).then(function () {
                        // 更新使用者點數
                        CurrentUserManager.updateSmsBalance();

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
                        // 更新使用者點數
                        CurrentUserManager.updateSmsBalance();

                        $scope.search();
                    });
                });
            };

            $scope.export = function () {

                $scope.StartDate = getDateBegin($scope.StartDate);
                $scope.EndDate = getDateEnd($scope.EndDate);

                var errors = [];

                if ($scope.EndDate < $scope.StartDate) {
                    errors.push('結束時間不能小於起始時間');
                }

                if (errors.length != 0) {
                    var messages = _.map(errors, function (error, index) {
                        return '<li>' + error + '</li>';
                    });
                    dialogs.error('操作錯誤', '<ol>' + messages.join('') + '</ol>');
                    return false;
                }

                var criteria = {
                    StartDate: $scope.StartDate,
                    EndDate: $scope.EndDate,
                    TradeType: $scope.TradeType,
                    SearchText: $scope.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.Download(criteria);
            };

            //========================================
            // Events & EventHandlers
            //========================================

            $scope.$on('tab.onSelect', function (event, tabName) {
                if (tabName !== 'TradeDetail') return;
                $scope.search();
            });

            //========================================
            // Initialize
            //========================================

            $scope.searchText = '';

            $scope.StartDate = new Date();
            $scope.StartDateOpend = false;
            $scope.EndDate = new Date();
            $scope.EndDateOpend = false;

            var TradeTypeOptions = SelectUtil.getEnumOptions(TradeType);
            $scope.TradeTypeOptions = TradeTypeOptions;
            $scope.TradeTypes = TradeType;
            $scope.TradeType = TradeTypeOptions.All;
        }]);

})(window, document);