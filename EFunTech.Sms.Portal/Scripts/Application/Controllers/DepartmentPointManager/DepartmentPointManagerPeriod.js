(function (window, document) {
    'use strict';

    angular.module('app').controller('DepartmentPointManagerPeriod', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', 'toaster', 'WebApi', '$translate', 'dialogs',
        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, toaster, WebApi, $translate, dialogs) {

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
                { name: 'AllotSettingDesc', displayName: '設定' },
                {
                    name: 'CanAllotPoint',
                    width: '50',
                    displayName: '選取',
                    cellEditableCondition: false,
                    cellClass: 'grid-align-center',
                    headerCellClass: 'grid-align-center',
                    cellTemplate: [
                        '<input type="checkbox" ng-show="row.entity.CanAllotPoint && !row.entity.AllotSetting" ng-model="row.entity.Checked" ng-click="grid.appScope.selectButtonClick(row.entity)">',
                        '<img class="userAdmin" ng-show="!row.entity.CanAllotPoint" />',
                        '<img class="userAllotSetting" ng-show="row.entity.AllotSetting" ng-click="grid.appScope.deleteSetting(row.entity)"/>',
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

                var schemaForm = SchemaFormFactory.create('DepartmentPointManagerPeriodModel');
                var title = '自動撥數';
                var schema = schemaForm.schema;
                var form = schemaForm.form;
                var model = {
                    ids: _.pluck($scope.selectedRows, 'Id'),
                    MonthlyAllot: true,
                    MonthlyAllotDay: '01',
                    MonthlyAllotPoint: '',
                    LimitMinPoint: '',
                    LimitMaxPoint: '',
                };
                var options = {
                    validationMessage: {
                        202: '不符合欄位格式',
                        302: '此為必填欄位',
                    }
                };
                var validateBeforeSubmit = function (modalScope, form) {
                    var result = true;

                    if (form.MonthlyAllot.$viewValue == true) {
                        //if (!form.MonthlyAllotDay.$valid) modalScope.$broadcast('schemaForm.error.MonthlyAllotDay', 302, true);
                        //if (!form.MonthlyAllotPoint.$valid) modalScope.$broadcast('schemaForm.error.MonthlyAllotPoint', 302, true);

                        if (!form.MonthlyAllotDay.$viewValue || form.MonthlyAllotDay.$viewValue.length == 0) {
                            //modalScope.$broadcast('schemaForm.error.MonthlyAllotDay', 302, false);
                            dialogs.error('操作錯誤', '請輸入每月撥點日期');
                            result = false;
                        }
                        if (!form.MonthlyAllotPoint.$viewValue || form.MonthlyAllotPoint.$viewValue.length == 0) {
                            //modalScope.$broadcast('schemaForm.error.MonthlyAllotPoint', 302, false);
                            dialogs.error('操作錯誤', '請輸入匯入點數');
                            result = false;
                        }
                    }
                    else {
                        if (!form.LimitMinPoint.$valid) modalScope.$broadcast('schemaForm.error.LimitMinPoint', 302, true);
                        if (!form.LimitMaxPoint.$valid) modalScope.$broadcast('schemaForm.error.LimitMaxPoint', 302, true);

                        if (!form.LimitMinPoint.$viewValue || form.LimitMinPoint.$viewValue.length == 0) {
                            //modalScope.$broadcast('schemaForm.error.LimitMinPoint', 302, false);
                            dialogs.error('操作錯誤', '請輸入額度下限');
                            result = false;
                        }
                        if (!form.LimitMaxPoint.$viewValue || form.LimitMaxPoint.$viewValue.length == 0) {
                            //modalScope.$broadcast('schemaForm.error.LimitMaxPoint', 302, false);
                            dialogs.error('操作錯誤', '請輸入自動補至點數');
                            result = false;
                        }
                    }
                    
                    return result;
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
                    $scope.createSetting(savedModel);
                });
            };

            $scope.createSetting = function (model) {
                var webApi = new WebApi('api/DepartmentPointManager/CreateAllotSetting', { title: '撥點設定' });

                webApi.Post(model)
                .then(function () {
                    $scope.search();
                });
            };

            $scope.deleteSetting = function (rowEntity) {
                var model = rowEntity[0] || rowEntity;
                
                var message = "確定刪除『" + model.FullName + "』的撥點設定？";
                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    var webApi = new WebApi('api/DepartmentPointManager/DeleteAllotSetting', { title: '清除設定' });

                    webApi.Delete(model)
                    .then(function () {
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
                if (tabName !== 'DepartmentPointManagerPeriod') return;
                $scope.search();
            });

            //========================================
            // Initialize
            //========================================

            $scope.selectedRows = [];
            $scope.fullName = '';
            $scope.userName = '';

            if (GlobalSettings.isSPA) {
                $scope.$on('menu.onSelect', function (event, menuName) {
                    if (menuName !== 'DepartmentPointManager') return;
                    var tabName = $scope.$parent.tabName;
                    if (tabName !== 'DepartmentPointManagerPeriod') return;
                    $scope.search();
                });
            }
        }]);

})(window, document);
