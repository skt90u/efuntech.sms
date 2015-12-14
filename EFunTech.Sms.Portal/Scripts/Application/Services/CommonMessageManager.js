(function (window, document) {
    'use strict';

    angular.module('app').service('CommonMessageManager', ['uiGridConstants', '$modal', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs',
        function (uiGridConstants, $modal, CrudApi, SchemaFormFactory, GlobalSettings, dialogs) {

            var self = this;
            var crudApi = new CrudApi('api/CommonMessage');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);
            paginationOptions.pageSize = 1000;

            /**
             * 將本次內容存入常用簡訊
             */
            this.save = function (sendTitle, sendBody) {
                if (sendBody.length == 0) {
                    dialogs.error('操作錯誤', '您尚未輸入任何內容');
                    return;
                }

                self.createRow({
                    Subject: sendTitle,
                    Content: sendBody,
                });
            };

            /**
             * 編輯常用簡訊
             */
            this.editCommonMessages = function () {

                ////////////////////////////////////////
                // gridModalCtrlOptions
                ////////////////////////////////////////

                var gridOptions = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                    // paginationPageSizes: paginationOptions.pageSizes, // 由 GridModalCtrl 控制
                    //paginationPageSize: paginationOptions.pageSize, // 由 GridModalCtrl 控制
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableColumnMenus: false,
                    columnDefs: [{
                        name: 'Subject',
                        displayName: '標題'
                    },
                    {
                        name: 'Content',
                        displayName: '內容'
                    },
                    {
                        name: 'Update',
                        width: '50',
                        displayName: '編輯',
                        cellEditableCondition: false,
                        cellClass: 'grid-align-center',
                        cellTemplate: '<img class="gg01" ng-click="grid.appScope.editRow(row.entity)"/>'
                    },
                    {
                        name: 'Delete',
                        width: '50',
                        displayName: '刪除',
                        cellEditableCondition: false,
                        cellClass: 'grid-align-center',
                        cellTemplate: '<img class="gg02" ng-click="grid.appScope.deleteRow(row.entity)"/>'
                    },
                    ],
                    data: [],
                    //onRegisterApi: function (gridApi) {}, // 由 GridModalCtrl 控制
                };

                ////////////////////////////////////////
                // formModalCtrlOptions
                ////////////////////////////////////////

                var schemaForm = SchemaFormFactory.create('CommonMessageModel');

                var title = function (model) {
                    return !angular.isDefined(model.Id) ? "新增常用簡訊" : "編輯常用簡訊";
                };
                var schema = schemaForm.schema;
                var form = schemaForm.form;
                var options = {
                    validationMessage: {
                        202: '不符合欄位格式',
                        302: '此為必填欄位',
                    }
                };
                var validateBeforeSubmit = function (modalScope, form) { return true; };

                var notify = function(type, args){
                    if (type == 'createRow' ||
                       type == 'updateRow' ||
                       type == 'deleteRow' ||
                       type == 'deleteSelection')
                        self.search();
                };

                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/gridModal.html',
                    controller: 'GridModalCtrl',
                    windowClass: 'center-modal',
                    //size: size,
                    resolve: {
                        gridModalCtrlOptions: function () {
                            return {
                                title: '常用簡訊',
                                gridOptions: gridOptions,
                                crudApi: crudApi,
                                disableCreate: false,
                                disableMultiDelete: false,
                                notify: notify,
                            };
                        },
                        formModalCtrlOptions: function () {
                            return {
                                title: title,
                                schema: schema,
                                form: form,
                                // model: model, 這邊是沒有model的
                                options: options,
                                validateBeforeSubmit: validateBeforeSubmit,
                            };
                        },
                    },
                });
            };

            this.search = function () {
                var criteria = {
                    SearchText: self.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria).then(function (result) {
                    var data = result.data;
                    self.dataResult = data.Result;
                });
            };

            this.createRow = function (model) {
                crudApi.Create(model)
                .then(function () {
                    self.search();
                });
            };

            this.show = function () {
                if (!self.loaded) {
                    self.search();
                    self.loaded = true;
                }
                self.visible = true;
            };

            //========================================
            // Events & EventHandlers
            //========================================

            this.searchText = '';
            this.dataResult = [];
            this.visible = false;
            this.loaded = false;

        }]);

})(window, document);
