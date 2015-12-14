(function (window, document) {
    'use strict';

    angular.module('app').service('CommonContactManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {


            var self = this;
            var crudApi = new CrudApi('api/CommonContact');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            var gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                //columnDefs: [
                //    { name: 'Name', displayName: '姓名' },
                //    { name: 'Mobile', displayName: '行動電話' },
                //    { name: 'Email', displayName: 'E-Mail' },
                //],
                columnDefs: [
                    { name: 'Name', displayName: '姓名', visible: true },
                    { name: 'Mobile', displayName: '行動電話', visible: true },
                    { name: 'Email', displayName: '電子郵件', visible: true },			
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
                    SearchText: self.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria)
                .then(function (result) {
                    if (self.gridApi)self.gridApi.selection.clearSelectedRows();

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
                $log.log('目前不支援從此功能頁更新聯絡人資料');
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
                                maxColumns: 3,
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
            this.searchText = '';
            this.gridOptions = gridOptions;
        }]);

})(window, document);
