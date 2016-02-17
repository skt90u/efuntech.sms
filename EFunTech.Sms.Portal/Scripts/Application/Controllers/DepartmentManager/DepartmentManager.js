(function (window, document) {
    'use strict';

    angular.module('app').controller('DepartmentManager', ['$scope', 'CompanyDeaprtmentManager', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'WebApi', '$translate', 'dialogs', '$q', 'LookupApi', 'ValidationApi', 'SchemaFormHelper', 'EnumMapping',
        function ($scope, CompanyDeaprtmentManager, $http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, WebApi, $translate, dialogs, $q, LookupApi, ValidationApi, SchemaFormHelper, EnumMapping) {

            //========================================
            // Settings
            //========================================

            var availableRoles = null;
            var availableDepartments = null;
            var existentUserNames = null;
            var canEditSmsProviderType = null;

            var crudApi = new CrudApi('api/DepartmentManager');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            $scope.gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                { name: 'Department.Name', displayName: '部門' }, 
                { name: 'FullName', displayName: '姓名' },
                { name: 'UserName', displayName: '帳號' },
                //{ name: 'EmployeeNo', displayName: '員工編號' },
                { name: 'PhoneNumber', displayName: '手機' },
                //{ name: 'Email', displayName: 'E-mail' },
                { name: 'CreatedUserName', displayName: '建立者' },
                {
                    name: 'SmsProviderType',
                    displayName: '發送線路',
                    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.SmsProviderType | EnumFilter: "SmsProviderType" }}</div>',
                    visible: false,
                },
                {
                    name: 'Activatable',
                    width: '100',
                    displayName: '啟用/關閉',
                    cellEditableCondition: false,
                    cellClass: 'grid-align-center',
                    headerCellClass: 'grid-align-center',
                    cellTemplate: [
                        '<img ng-class="row.entity.Enabled ? \'userEnabled\' : \'userDisabled\'" ng-show="row.entity.Activatable" ng-click="grid.appScope.activateAccount(row.entity)"/>',
                        '<img class="userAdmin" ng-show="!row.entity.Activatable" />',
                    ].join('\n'),
                },
                {
                    name: 'Maintainable',
                    width: '100',
                    displayName: '維護',
                    cellEditableCondition: false,
                    cellClass: 'grid-align-center',
                    headerCellClass: 'grid-align-center',
                    cellTemplate: [
						'<img class="Gear" ng-show="row.entity.Maintainable" ng-click="grid.appScope.editRow(row.entity)"/>',
						'<img class="cross" ng-show="row.entity.Deletable" ng-click="grid.appScope.deleteRow(row.entity)"/>',
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
                    var data = result.data;
                    $scope.gridOptions.totalItems = data.TotalCount;
                    $scope.gridOptions.data = data.Result;
                });
            };

            $scope.editRow = function (rowEntity) {

                if (availableRoles == null ||
                    availableDepartments == null ||
                    existentUserNames == null || 
                    canEditSmsProviderType == null) {
                    $scope.loading = true;
                    $q.all([
                        LookupApi.GetAvailableRoles(),
                        LookupApi.GetAvailableDepartments_DepartmentManager(),
                        LookupApi.GetExistentUserNames(),
                        LookupApi.GetCurrentUser()
                    ])
                    .then(function (results) {
                        availableRoles = results[0].data;
                        availableDepartments = results[1].data;
                        existentUserNames = results[2].data;
                        canEditSmsProviderType = results[3].data.CanEditSmsProviderType;

                        $scope.editRow(rowEntity);
                    })
                    .finally(function () {
                        $scope.loading = false;
                    });
                    return; // 不要拿掉
                }

                var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                model.NewPassword = '';
                model.NewPasswordConfirmed = '';

                var isNew = !angular.isDefined(model.Id);
                if (!isNew) {
                    if (availableDepartments.length == 0 ||
                    !_.findWhere(availableDepartments, { value: model.DepartmentId })) {
                        // 如果目前部門不在可用部門中，則將部門設定為 null
                        model.DepartmentId = null;
                    }

                    if (availableRoles.length == 0 ||
                        !_.findWhere(availableRoles, { value: model.RoleId })) {
                        // 如果目前角色不在可用角色中，則將角色設定為 null
                        model.RoleId = null;
                    }
                }
                else {
                    // 新增資料時的預設值

                    // 預設線路為 '一般線路'
                    model.SmsProviderType = EnumMapping.SmsProviderType.InfobipNormalQuality.value;
                }

                var schemaForm = SchemaFormFactory.create('DepartmentManagerModel', {
                    isNew: isNew,
                    canEditSmsProviderType: canEditSmsProviderType,
                    "UserName.readonly": !isNew,
                    "UserName.$asyncValidators": {
                        usernameAlreadyTaken: function (value) {
                            var deferred = $q.defer();
                            
                            //ValidationApi.MakeSureUserNameNotExists({
                            //    UserName: value
                            //}, function () {
                            //    deferred.resolve(); // 驗證通過
                            //}, function () {
                            //    deferred.reject(); // 驗證失敗
                            //});
                            
                            if (_.contains(existentUserNames, value)) {
                                deferred.reject(); // 驗證失敗
                                // throw new Exception(string.Format("帳號 {0} 已經存在", UserName));
                            }
                            else {
                                deferred.resolve(); // 驗證通過
                            }
                            
                            return deferred.promise;
                        }
                    },
                    "NewPassword.$validators": {
                        passwordNotMatch: function (value) {
                            var NewPassword = value;
                            var NewPasswordConfirmed = SchemaFormHelper.getViewValue('NewPasswordConfirmed');
                            if (NewPassword == null) NewPassword = '';
                            if (NewPasswordConfirmed == null) NewPasswordConfirmed = '';
                            if (NewPassword === NewPasswordConfirmed) {
                                SchemaFormHelper.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', true);
                                SchemaFormHelper.$broadcast('schemaForm.error.NewPasswordConfirmed', '302', true); // 首先輸入[再次輸入密碼]，之後才輸入[密碼]，都會出現此為必填欄位，目前不知道怎麼解決。
                            }
                            else {
                                SchemaFormHelper.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', false); // false -> 表示 NewPasswordConfirmed 的 passwordNotMatch 驗證不通過
                            }

                            return true; // 永遠都回傳true, 目的是要在先輸入確認密碼，後輸入密碼，改變確認密碼錯誤訊息。
                        },
                    },
                    "NewPasswordConfirmed.$validators": {
                        passwordNotMatch: function (value) {
                            var NewPassword = SchemaFormHelper.getViewValue('NewPassword');
                            var NewPasswordConfirmed = value;
                            if (NewPassword == null) NewPassword = '';
                            if (NewPasswordConfirmed == null) NewPasswordConfirmed = '';
                            return NewPassword === NewPasswordConfirmed;
                        },
                    },
                    "DepartmentId.enum": _.pluck(availableDepartments, 'value'),
                    "RoleId.enum": _.pluck(availableRoles, 'value'),
                    "DepartmentId.titleMap": availableDepartments,
                    "RoleId.titleMap": availableRoles,
                });

                var title = isNew ? '新增帳號' : '編輯帳號';
                var schema = schemaForm.schema;
                var form = schemaForm.form;
                var options = {
                    validationMessage: {
                        202: '不符合欄位格式',
                        302: '此為必填欄位',
                    }
                };
                var validateBeforeSubmit = function (modalScope, form) {
                    if (isNew) {
                        // 新增資料，密碼必填
                        if(form.NewPassword.$viewValue != form.NewPasswordConfirmed.$viewValue) {
                            modalScope.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', '密碼不一致');
                            return false;
                        }
                        else {
                            modalScope.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', true);
                            return true;
                        }
                    }
                    else {
                        // 如果有重設密碼，必須檢查再次輸入密碼是否一致
                        //檢查密碼與再次確認密碼
                        if ((form.NewPassword.$viewValue.length != 0 || form.NewPasswordConfirmed.$viewValue.length != 0) &&
                             form.NewPassword.$viewValue != form.NewPasswordConfirmed.$viewValue) {
                            modalScope.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', '密碼不一致');
                            return false;
                        }
                        else{
                            modalScope.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', true);
                            return true;
                        }
                    }
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

                    // 20151003 Norman, 每次編輯都重新讀取
                    availableRoles = null;
                    availableDepartments = null;
                    existentUserNames = null;

                }, function () {

                    $log.debug('Modal dismissed at: ' + new Date());

                    // 20151003 Norman, 每次編輯都重新讀取
                    availableRoles = null;
                    availableDepartments = null;
                    existentUserNames = null;
                });
            };

            
            $scope.createRow = function (model) {
                crudApi.Create(model)
                .then(function () {
                    $scope.search();
                    $scope.CompanyDeaprtmentManager.search();
                });
            };

            $scope.updateRow = function (model) {
                crudApi.Update(model)
                .then(function () {
                    $scope.search();
                    $scope.CompanyDeaprtmentManager.search();
                });
            };

            $scope.deleteRow = function (rowEntity) {
                var selectedRow = rowEntity[0] || rowEntity;

                var message = "確定刪除『" + selectedRow.UserName + "』？";
                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    crudApi.Delete(selectedRow)
                    .then(function () {
                        $scope.search();
                        $scope.CompanyDeaprtmentManager.search();
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
                        $scope.CompanyDeaprtmentManager.search();
                    });
                });
            };

            // 啟用/關閉帳號
            $scope.activateAccount = function (rowEntity) {
                var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                
                var message = model.Enabled
                    ? '是否停用帳號 ' + model.UserName + ' ?'
                    : '是否啟用帳號 ' + model.UserName + ' ?';
                dialogs.confirm('啟用 / 停用帳號', message).result.then(function (btn) {
                    model.Enabled = !model.Enabled;
                    $scope.updateRow(model);
                });
            };

            //========================================
            // Initialize
            //========================================

            $scope.fullName = '';
            $scope.userName = '';

            $scope.CompanyDeaprtmentManager = CompanyDeaprtmentManager;
            $scope.CompanyDeaprtmentManager.onChanged = function () {
                availableDepartments = null;
                $scope.search();
            };
            
            $scope.search();

            //當角色為Admin時，則查詢結果包含【簡訊預設發送線路】欄位
            //當角色非Admin時，則查詢結果不含【簡訊預設發送線路】欄位
            LookupApi.GetCurrentUser({}, function (data) {
                var currentUser = data;
                var matchs = _.where($scope.gridOptions.columnDefs, { name: 'SmsProviderType' });
                angular.forEach(matchs, function (match, idx) {
                    canEditSmsProviderType = currentUser.CanEditSmsProviderType;
                    match.visible = canEditSmsProviderType;
                });
            });

        }]);

})(window, document);
