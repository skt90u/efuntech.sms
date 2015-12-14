(function (window, document) {
    'use strict';

    angular.module('app').service('CompanyDeaprtmentManager', ['$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'WebApi', '$translate', 'dialogs', '$q', 'LookupApi', 'ValidationApi', 'SchemaFormHelper',
        function ($http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, WebApi, $translate, dialogs, $q, LookupApi, ValidationApi, SchemaFormHelper) {

            var self = this;

            //========================================
            // Settings
            //========================================

            var crudApi = new CrudApi('api/Department');

            this.UserName = '';
            this.CanEditDepartment = false;
            this.Deaprtments = [];
            this.collapse = false;

            //========================================
            // Functions
            //========================================

            this.search = function () {
                var criteria = {
                    SearchText: '',
                    PageIndex: 1,
                    PageSize: -1, // 抓取全部，目的在正確更新有效與無效筆數
                };

                crudApi.GetAll(criteria).then(function (result) {
                    var data = result.data;
                    self.Deaprtments = data.Result;
                });
            };

            this.editRow = function (rowEntity) {

                var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
                var isNew = !angular.isDefined(model.Id);
                if (isNew) {
                    // TODO: 設定新增資料的初始值
                }

                var schemaForm = SchemaFormFactory.create('DepartmentModel', {
                    isNew: isNew
                });

                var title = isNew ? '新增部門' : '編輯部門';
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
                    self.search();
                    (self.onChanged || angular.noop)();
                });
            };

            this.updateRow = function (model) {
                crudApi.Update(model).then(function () {
                    self.search();
                    (self.onChanged || angular.noop)();
                });
            };

            this.deleteRow = function (rowEntity) {
                var model = rowEntity[0] || rowEntity;

                var message = "確定刪除『" + model.Name + "』？";
                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    crudApi.Delete(model).then(function () {
                        self.search();
                        (self.onChanged || angular.noop)();
                    });
                });
            };

            this.onChanged = angular.noop;

            //========================================
            // Initialize
            //========================================

            // 取得目前使用者名稱
            LookupApi.GetCurrentUser({}, function (data) {
                self.UserName = data.UserName;
                self.CanEditDepartment = data.CanEditDepartment;
            });

            self.search();


        }]);

})(window, document);
