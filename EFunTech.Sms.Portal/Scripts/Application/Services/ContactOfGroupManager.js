(function (window, document) {
    'use strict';

    angular.module('app').service('ContactOfGroupManager', ['$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs', 'uiGridConstants',
        function ($modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs, uiGridConstants) {

            var serviceInst = this;

            //========================================
            // Settings - GroupManager
            //========================================

            var GroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/Group');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);
                paginationOptions.pageSize = 1000;

                this.search = function () {
                    self.selectedItems = [];

                    var criteria = {
                        //IncludeAllGroup: true,
                        SearchText: self.searchText,
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria).then(function (result) {
                        var data = result.data;
                        self.dataResult = data.Result;
                    });
                };

                this.chipSelectChange = function (selectedItems) {
                    self.selectedItems = selectedItems;
                    serviceInst.SharedContactManager.search();
                };

                this.resetSelectedChip = function () {
                    var chipsCtrl = angular.element('#GroupManagerChips').controller('chips');
                    if (chipsCtrl) {
                        chipsCtrl.resetSelectedChip();
                    }
                };

                this.init = function () {
                    self.resetSelectedChip();
                    self.search();
                };

                this.selectedItems = [];
                this.dataResult = [];
            };

            //========================================
            // Settings - SharedGroupManager
            //========================================

            var SharedGroupManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/SharedGroupContact');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);
                paginationOptions.pageSize = 1000;

                this.search = function () {
                    self.selectedItems = [];

                    var criteria = {
                        //IncludeAllGroup: true,
                        SearchText: self.searchText,
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria).then(function (result) {
                        var data = result.data;
                        self.dataResult = data.Result;
                    });
                };

                this.chipSelectChange = function (selectedItems) {
                    self.selectedItems = selectedItems;
                    serviceInst.SharedContactManager.search();
                };

                this.resetSelectedChip = function () {
                    var chipsCtrl = angular.element('#SharedGroupManagerChips').controller('chips');
                    if (chipsCtrl) {
                        chipsCtrl.resetSelectedChip();
                    }
                };

                this.init = function () {
                    self.resetSelectedChip();
                    self.search();
                };

                this.selectedItems = [];
                this.dataResult = [];
            };

            //========================================
            // Settings - SharedContactManager
            //========================================

            var SharedContactManager = function () {
                var self = this;
                var crudApi = new CrudApi('api/AllContact');
                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

                var gridOptions = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                    paginationPageSizes: paginationOptions.pageSizes,
                    paginationPageSize: paginationOptions.pageSize,
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableColumnMenus: false,
                    columnDefs: [
                        { name: 'Name', displayName: '姓名', visible: true },
                        { name: 'Mobile', displayName: '行動電話', visible: true },
                        { name: 'Email', displayName: '電子郵件', visible: true },			
                        { name: 'Groups', displayName: '群組' , visible: true },			
                        { name: 'HomePhone', displayName: '住家電話', visible: false},
                        { name: 'CompanyPhone', displayName: '公司電話', visible: false},
                        { name: 'Msn', displayName: 'MSN', visible: false},
                        { name: 'Birthday', displayName: '生日', visible: false},
                        {
                            name: 'Gender',
                            displayName: '性別',
                            cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Gender | EnumFilter: "Gender"}}</div>',
                            visible: false
                        },
                        { name: 'ImportantDay', displayName: '重要日子', visible: false },
                        { name: 'Description', displayName: '聯絡人概述', visible: false },
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

                this.search = function () {
                    var criteria = {
                        //IncludeAllGroup: true,
                        GroupIds: serviceInst.GroupManager.selectedItems.length == 0 ? -1 : _.pluck(serviceInst.GroupManager.selectedItems, 'Id').join(','),
                        SharedGroupIds: serviceInst.SharedGroupManager.selectedItems.length == 0 ? -1 : _.pluck(serviceInst.SharedGroupManager.selectedItems, 'GroupId'),
                        SearchText: self.searchText,
                        PageIndex: paginationOptions.pageNumber,
                        PageSize: paginationOptions.pageSize
                    };

                    crudApi.GetAll(criteria).then(function (result) {
                        if (self.gridApi) self.gridApi.selection.clearSelectedRows();

                        var data = result.data;
                        self.gridOptions.totalItems = data.TotalCount;
                        self.gridOptions.data = data.Result;
                    });
                };

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
                        self.search();
                    });
                };

                this.updateRow = function (model) {
                    crudApi.Update(model)
                    .then(function () {
                        self.search();
                    });
                };

                this.deleteRow = function (rowEntity) {
                    var selectedRow = rowEntity[0] || rowEntity;

                    var message = "確定刪除『" + selectedRow.Name + "』？";
                    dialogs.confirm('刪除資料', message).result.then(function (btn) {
                        crudApi.Delete(selectedRow)
                        .then(function () {
                            self.search();
                        });
                    });
                };

                this.selectColumns = function () {

                    var modalInstance = $modal.open({
                        templateUrl: 'template/modal/columnEditor.html',
                        controller: 'ColumnEditorCtrl',
                        windowClass: 'center-modal',
                        //size: size,
                        resolve: {
                            options: function () {
                                return {
                                    title: '自訂資料顯示欄位',
                                    maxColumns: 4,
                                    columnDefs: self.gridOptions.columnDefs,
                                    exclusiveNames: [], //['Name'],
                                };
                            },
                        }
                    });

                    modalInstance.result.then(function (xxx) {
                        self.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
                    });
                };

                this.init = function () {
                    self.searchText = '';
                    self.search();
                };

                this.gridApi = null;
                this.gridOptions = gridOptions;
                this.searchText = '';
            };


            //========================================
            // Initialize
            //========================================

            this.GroupManager = new GroupManager();
            this.SharedGroupManager = new SharedGroupManager();
            this.SharedContactManager = new SharedContactManager();

            this.init = function () {
                serviceInst.GroupManager.init();
                serviceInst.SharedGroupManager.init();
                serviceInst.SharedContactManager.init();
            };
            
        }]);

})(window, document);
