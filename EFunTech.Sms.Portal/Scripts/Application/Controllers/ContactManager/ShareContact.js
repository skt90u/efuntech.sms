(function (window, document) {
    'use strict';

    angular.module('app').controller('ShareContact', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', '$timeout', '$q', 'WebApi', 'LookupApi', '$translate', 'dialogs', '$element',
        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, $timeout, $q, WebApi, LookupApi, $translate, dialogs, $element) {

            //========================================
            // Settings - GroupManager
            //========================================

            var GroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/Group');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);
                paginationOptions.pageSize = 1000;

                this.search = function () {
                    var criteria = {
                        SearchText: self.searchText,
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria).then(function (result) {
                        var data = result.data;
                        self.dataResult = data.Result;

                        $timeout(function () {
                            // GroupManager每次搜尋，都表示重新再入這個頁簽，因此設定預設值(第一筆資料)
                            var chipsCtrl = $element.find('chips').controller('chips');
                            if (chipsCtrl) {
                                chipsCtrl.selectChip(0);
                            }
                        }, 10); // 如果不delay，會失效
                    });
                };

                this.editRow = function (rowEntity) {
                    var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                    var isNew = !angular.isDefined(model.Id);
                    if (isNew) {
                        // TODO: 設定新增資料的初始值
                    }

                    var schemaForm = SchemaFormFactory.create('GroupModel', {
                        isNew: isNew
                    });

                    var title = isNew ? '新增群組' : '編輯群組';
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
                            self.createRow(savedModel);
                        }
                        else {
                            self.updateRow(savedModel);
                        }
                    },
                    function () {
                        $log.debug('Modal dismissed at: ' + new Date());
                    });
                };

                this.createRow = function (model) {
                    crudApi.Create(model).then(function () {
                        $scope.search();
                    });
                };

                this.updateRow = function (model) {
                    crudApi.Update(model).then(function () {
                        $scope.search();
                    });
                };

                this.deleteRow = function (rowEntity) {
                    var selectedRow = rowEntity[0] || rowEntity;

                    var message = "刪除群組，將連同群組內的聯絡人、分享聯絡人一起刪除，確定刪除『" + selectedRow.Description + "』？";
                    dialogs.confirm('刪除資料', message).result.then(function (btn) {
                        crudApi.Delete(selectedRow)
                        .then(function () {
                            $scope.search();
                        });
                    });
                };

                $scope.chipSelectChange = function (selectedItems) {
                    $scope.GroupManager.selectedGroup = selectedItems[0];
                    $scope.UserNotInSharedGroupManager.loadDepartmentUsers();
                    $scope.UserNotInSharedGroupManager.loadOtherUsers();
                    $scope.UserInSharedGroupManager.search();
                };

                $scope.resetSelectedChip = function () {
                    var chipsCtrl = $element.find('chips').controller('chips');
                    if (chipsCtrl) {
                        chipsCtrl.resetSelectedChip();
                    }
                };

                this.selectedGroup = null;
                this.searchText = '';
                this.dataResult = [];
            };

            //========================================
            // Settings - UserNotInSharedGroupManager
            //========================================
            
            var UserNotInSharedGroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/UserNotInSharedGroup');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);
                paginationOptions.pageSize = 1000;

                this.loadDepartments = function () {
                    self.selectedDepartment = null;
                    self.selectedDepartmentUsers = null;

                    LookupApi.GetAvailableDepartments_ShareContact({}, function (data) {
                        self.departments = data;
                        self.selectedDepartment = self.departments.length != 0 ? self.departments[0] : null;

                        if (self.selectedDepartment != null) {
                            self.loadDepartmentUsers();
                        }
                    });
                };

                // 載入部門使用者
                this.loadDepartmentUsers = function () {
                    self.selectedDepartmentUsers = null;

                    if ($scope.GroupManager.selectedGroup == null) return;
                    if (self.selectedDepartment == null) return;

                    var criteria = {
                        GroupId: $scope.GroupManager.selectedGroup.Id,
                        DepartmentId: self.selectedDepartment.value,
                        SearchText: '',
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria)
                    .then(function (result) {
                        var data = result.data;
                        self.departmentUsers = data.Result;
                        self.selectedDepartmentUsers = null;
                    });
                };
                // 載入其他系統使用者
                this.loadOtherUsers = function () {
                    self.manualShareList = null;
                    self.manualShareListUsernames = [];
                    self.manualShareListValidUsers = [];

                    var criteria = {
                        GroupId: $scope.GroupManager.selectedGroup.Id,
                        DepartmentId: -1, // 手動輸入使用者時，DepartmentId 會傳回-1
                        SearchText: '',
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria)
                    .then(function (result) {
                        var data = result.data;
                        self.otherUsers = data.Result;
                        self.manualShareList = null;
                        self.manualShareListUsernames = [];
                        self.manualShareListValidUsers = [];
                    });
                };

                this.onChangeDepartment = function () {
                    if ($scope.GroupManager.selectedGroup == null) {
                        dialogs.error('操作錯誤', '請先選擇上方群組');
                        return;
                    }

                    if (self.selectedDepartment == null) return;

                    self.loadDepartmentUsers();
                };

                this.onChangeManualShareList = function () {

                    var input = self.manualShareList || '';
                    if (input == '') {
                        self.manualShareListUsernames = [];
                        self.manualShareListValidUsers = [];
                        return;
                    }

                    var userNames = [];
                    var users = [];

                    var tokenSeparators = GlobalSettings.tokenSeparators;
                    var tokens = input.split(new RegExp(tokenSeparators.join('|'), 'g'));
                    for (var i = 0; i < tokens.length; i++) {
                        var userName = tokens[i].trim();
                        if (userName !== '')
                            userNames.push(userName);
                    }

                    angular.forEach(userNames, function (userName) {
                        var user = _.findWhere(self.otherUsers || [], { UserName: userName });
                        if (user) {
                            users.push(user);
                        }
                    });

                    self.manualShareListUsernames = userNames;
                    self.manualShareListValidUsers = users;
                };

                this.createMultiRows = function (models) {
                    if (models.length == 0) return;

                    angular.forEach(models, function (model, index) {
                        model.SharedGroupId = $scope.GroupManager.selectedGroup.Id;
                        crudApi.Create(model).then(function () {
                            if (index == models.length - 1) {
                                $scope.UserNotInSharedGroupManager.manualShareList = null;
                                $scope.UserNotInSharedGroupManager.loadDepartmentUsers();
                                $scope.UserNotInSharedGroupManager.loadOtherUsers();
                                $scope.UserInSharedGroupManager.search();
                            }
                        });
                    });
                };

                this.departments = null; // 目前使用者可選擇的所有部門
                this.departmentUsers = null; // 指定部門下的所有使用者
                this.selectedDepartment = null; // 目前選取的部門
                this.selectedDepartmentUsers = null; // 目前選取部門使用者

                this.otherUsers = []; // 系統其他使用者
                this.manualShareList = null; // 目前手動輸入使用者帳號(textarea內容)
                this.manualShareListUsernames =[]; // manualShareList 以逗號隔開
                this.manualShareListValidUsers =[]; // manualShareListUsernames 中的所有有效使用者
            };

            //========================================
            // Settings - UserInSharedGroupManager
            //========================================

            var UserInSharedGroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/UserInSharedGroup');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

                var gridOptions = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                    paginationPageSizes: paginationOptions.pageSizes,
                    paginationPageSize: paginationOptions.pageSize,
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableColumnMenus: false,
                    columnDefs: [{
                        name: 'UserName',
                        displayName: '帳號名稱'
                    },
                    {
                        name: 'FullName',
                        displayName: '姓名'
                    }, {
                        name: 'Delete',
                        width: '90',
                        displayName: '刪除',
                        cellEditableCondition: false,
                        cellClass: 'grid-align-center',
                        cellTemplate: '<img class="cross" ng-click="grid.appScope.deleteShareContact(row.entity)"/>'
                    },
                    ],
                    data: [],
                    onRegisterApi: function (gridApi) {
                        self.gridApi = gridApi;
                        gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                            paginationOptions.pageNumber = newPage;
                            paginationOptions.pageSize = pageSize;
                            self.search();
                        });
                    }
                };

                this.search = function () {
                    var criteria = {
                        GroupId: $scope.GroupManager.selectedGroup ? $scope.GroupManager.selectedGroup.Id : -1,
                        SearchText: self.searchText,
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria).then(function (result) {
                        var data = result.data;
                        self.gridOptions.totalItems = data.TotalCount;
                        self.gridOptions.data = data.Result;
                    });
                };

                this.updateRow = function (model) {
                    crudApi.Update(model).then(function () {
                        $scope.UserNotInSharedGroupManager.loadDepartmentUsers();
                        $scope.UserInSharedGroupManager.search();
                    });
                };

                this.searchText = '';
                this.gridOptions = gridOptions;
            };

            //========================================
            // Functions
            //========================================

            $scope.search = function () {
                $scope.GroupManager.selectedGroup = null;
                $scope.resetSelectedChip(); // 取消選取
                $scope.GroupManager.search();

                $scope.UserInSharedGroupManager.gridOptions.data = [];
            };

            $scope.deleteShareContact = function (model) {
                if ($scope.GroupManager.selectedGroup == null) return;
                model.SharedGroupId = $scope.GroupManager.selectedGroup.Id;
                $scope.UserInSharedGroupManager.updateRow(model);
            };

            $scope.createShareContactByShareList = function () {
                if ($scope.GroupManager.selectedGroup == null) {
                    dialogs.error('操作錯誤', '請先選擇上方群組');
                    return;
                }

                if ($scope.UserNotInSharedGroupManager.selectedDepartmentUsers == null ||
                    $scope.UserNotInSharedGroupManager.selectedDepartmentUsers.length == 0) {
                    dialogs.error('操作錯誤', '請先選擇成員');
                    return;
                }
                
                $scope.UserNotInSharedGroupManager.createMultiRows($scope.UserNotInSharedGroupManager.selectedDepartmentUsers);
            };

            $scope.createShareContactByManualShareList = function () {
                if ($scope.GroupManager.selectedGroup == null) {
                    dialogs.error('操作錯誤', '請先選擇上方群組');
                    return;
                }

                var users = $scope.UserNotInSharedGroupManager.manualShareListValidUsers;
                if (users.length != 0)
                    $scope.UserNotInSharedGroupManager.createMultiRows(users);
            };

            //========================================
            // Events & EventHandlers
            //========================================

            $scope.$on('tab.onSelect', function (event, tabName) {
                if (tabName !== 'ShareContact') return;
                $scope.search();
                $scope.UserNotInSharedGroupManager.loadDepartments();
            });

            //========================================
            // Initialize
            //========================================

            $scope.GroupManager = new GroupManager();
            $scope.UserNotInSharedGroupManager = new UserNotInSharedGroupManager();
            $scope.UserInSharedGroupManager = new UserInSharedGroupManager();

        }]);

})(window, document);
