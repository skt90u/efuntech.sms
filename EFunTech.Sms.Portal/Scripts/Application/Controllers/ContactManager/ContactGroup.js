(function (window, document) {
    'use strict';

    angular.module('app').controller('ContactGroup', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', '$timeout', '$q', 'WebApi', '$translate', 'dialogs', '$element',
        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, $timeout, $q, WebApi, $translate, dialogs, $element) {

            //========================================
            // Settings - ContactManager
            //========================================

            var ContactManager = function() {
                var self = this;

                var crudApi = new CrudApi('api/AllContact');

                this.editRow = function (rowEntity) {
                    var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                    var isNew = !angular.isDefined(model.Id);
                    model.Gender = angular.isDefined(model.Gender) ? model.Gender : '0'; // set default value

                    var schemaForm = SchemaFormFactory.create('ContactModel');

                    var title = isNew ? '新增聯絡人' : '編輯聯絡人';
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
                    });
                };

                this.createRow = function (model) {
                    crudApi.Create(model)
                    .then(function () {
                        // 重新查詢所有聯絡人
                        $scope.ContactNotInGroupManager.search();
                    });
                };

                this.updateRow = function (model) {
                    $log.log('目前不支援從此功能頁更新聯絡人資料');
                };
            };

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

                        $timeout(function(){ 
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
                    crudApi.Create(model)
                    .then(function () {
                        $scope.search();
                    });
                };

                this.updateRow = function (model) {
                    crudApi.Update(model)
                    .then(function () {
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
                    $scope.ContactInGroupManager.search();
                    $scope.ContactNotInGroupManager.search();
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
            // Settings - ContactInGroupManager
            //========================================

            var ContactInGroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/ContactInGroup');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

                var gridOptions = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                        paginationPageSizes: paginationOptions.pageSizes,
                        paginationPageSize: paginationOptions.pageSize,
                        useExternalPagination: true,
                        useExternalSorting: true,
                        enableColumnMenus: false,
                        columnDefs: [{
                            name: 'Name',
                            displayName: '姓名'
                        },
                        {
                            name: 'Mobile',
                            displayName: '手機號碼'
                        },
                        {
                            name: 'Region',
                            displayName: '發送地區'
                        },
                        {
                            name: 'Update',
                            width: '90',
                            displayName: '移除群組',
                            cellEditableCondition: false,
                            cellClass: 'grid-align-center',
                            cellTemplate: '<img class="mbi_032" ng-click="grid.appScope.removeFromGroup(row.entity)"/>'
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
                        self.loading = true;
                        $scope.ContactInGroupManager.search();
                        $scope.ContactNotInGroupManager.search();
                    });
                };

                this.searchText = '';
                this.gridOptions = gridOptions;
            };

            //========================================
            // Settings - ContactNotInGroupManager
            //========================================

            var ContactNotInGroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/ContactNotInGroup');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

                var gridOptions = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                    paginationPageSizes: paginationOptions.pageSizes,
                    paginationPageSize: paginationOptions.pageSize,
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableColumnMenus: false,
                    columnDefs: [{
                        name: 'Update',
                        width: '90',
                        displayName: '移入群組',
                        cellEditableCondition: false,
                        cellClass: 'grid-align-center',
                        cellTemplate: '<img class="mbi_033" ng-click="grid.appScope.joinToGroup(row.entity)"/>'
                    },
                    {
                        name: 'Name',
                        displayName: '姓名'
                    },
                    {
                        name: 'Mobile',
                        displayName: '手機號碼'
                    },
                    {
                        name: 'Region',
                        displayName: '發送地區'
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
                        SearchText : self.searchText,
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
                        self.loading = true;
                        $scope.ContactNotInGroupManager.search();
                        $scope.ContactInGroupManager.search();
                    });
                };

                this.searchText = '';
                this.gridOptions = gridOptions;
            };

            //========================================
            // Functions
            //========================================

            $scope.search = function () {
                // 取消選取
                $scope.GroupManager.selectedGroup = null;
                $scope.resetSelectedChip();

                $scope.GroupManager.search();

                $scope.ContactInGroupManager.gridOptions.data = [];
                $scope.ContactNotInGroupManager.gridOptions.data = [];
            };

            $scope.removeFromGroup = function (model) {
                if ($scope.GroupManager.selectedGroup == null) return;
                model.RemoveFromGroupId = $scope.GroupManager.selectedGroup.Id;
                $scope.ContactInGroupManager.updateRow(model);
            };

            $scope.joinToGroup = function (model) {
                if ($scope.GroupManager.selectedGroup == null) return;
                model.JoinToGroupId = $scope.GroupManager.selectedGroup.Id;
                $scope.ContactNotInGroupManager.updateRow(model);
            };

            //========================================
            // Events & EventHandlers
            //========================================

            $scope.$on('tab.onSelect', function (event, tabName) {
                if (tabName !== 'ContactGroup') return;
                $scope.search();
            });

            //========================================
            // Initialize
            //========================================

            $scope.ContactManager = new ContactManager();
            $scope.GroupManager = new GroupManager();
            $scope.ContactInGroupManager = new ContactInGroupManager();
            $scope.ContactNotInGroupManager = new ContactNotInGroupManager();
           
        }]);

})(window, document);
