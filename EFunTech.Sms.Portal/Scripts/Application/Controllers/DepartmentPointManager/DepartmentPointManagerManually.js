(function (window, document) {
    'use strict';

    angular.module('app').controller('DepartmentPointManagerManually', ['$scope', 'CurrentUserManager', '$http', 'NumberUtil', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', 'toaster', 'WebApi', '$translate', 'dialogs',
        function ($scope, CurrentUserManager, $http, NumberUtil, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, toaster, WebApi, $translate, dialogs) {

            //========================================
            // Settings
            //========================================

            var availableRoles = null;
            var availableDepartments = null;
            var crudApi = new CrudApi('api/DepartmentPointManager');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            $scope.gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                { name: 'Department.Description', displayName: '所屬部門' }, 
                { name: 'FullName', displayName: '姓名' },
                { name: 'UserName', displayName: '帳號' },
                { name: 'EmployeeNo', displayName: '員工編號' },
                { name: 'SmsBalance', displayName: '點數' },
                {
                    name: 'CanAllotPoint',
                    width: '50',
                    displayName: '選取',
                    cellEditableCondition: false,
                    cellClass: 'grid-align-center',
                    headerCellClass: 'grid-align-center',
                    cellTemplate: [
                        '<input type="checkbox" ng-show="row.entity.CanAllotPoint" ng-model="row.entity.Checked" ng-click="grid.appScope.selectButtonClick(row.entity)">',
                        '<img class="userAdmin" ng-show="!row.entity.CanAllotPoint" />',
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
                    FullName: $scope.fullName,
                    UserName: $scope.userName,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria)
                .then(function (result) {
                    $scope.selectedRows = [];
                    var data = result.data;
                    $scope.gridOptions.totalItems = data.TotalCount;
                    $scope.gridOptions.data = data.Result;
                });
            };

            $scope.edit = function () {
                if ($scope.selectedRows.length == 0) {
                    dialogs.error('操作錯誤', '請選取成員');
                    return;
                }

                var schemaForm = SchemaFormFactory.create('DepartmentPointManagerManuallyModel');

                var title = '手動撥點';
                var schema = schemaForm.schema;
                var form = schemaForm.form;
                var model = {
                    ids: _.pluck($scope.selectedRows, 'Id'),
                    point: '',
                };
                var options = {
                    validationMessage: {
                        202: '不符合欄位格式',
                        302: '此為必填欄位',
                    }
                };
                var validateBeforeSubmit = function (modalScope, form) { return true;};

                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/editRow.html',
                    controller: 'FormModalCtrl',
                    windowClass: 'center-modal',
                    size: 'sm',
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
                    
                    var webApi = new WebApi('api/DepartmentPointManager/AllotPoint', { title: '手動撥點' });

                    webApi.Post(savedModel)
                    .then(function () {
                        // 更新使用者點數
                        CurrentUserManager.updateSmsBalance();

                        $scope.search();
                    });
                });
            };

            $scope.selectButtonClick = function (rowEntity) {
                var model = rowEntity[0] || rowEntity || {};

                if (_.contains($scope.selectedRows, model)) {
                    $scope.selectedRows = _.without($scope.selectedRows, model);
                }
                else {
                    $scope.selectedRows.push(model);
                }
            };

            //========================================
            // Events & EventHandlers
            //========================================

            $scope.$on('tab.onSelect', function (event, tabName) {
                if (tabName !== 'DepartmentPointManagerManually') return;
                $scope.search();
            });

            //========================================
            // Initialize
            //========================================

            $scope.selectedRows = [];
            $scope.fullName = '';
            $scope.userName = '';
            
            $scope.search();
        }]);

})(window, document);
