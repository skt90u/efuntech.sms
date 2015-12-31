(function (window, document) {
    'use strict';

    angular.module('app').service('FileUploadManager', ['$modal', 'Converter', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'dialogs', 'FileManagerApi',
        function ($modal, Converter, CrudApi, SchemaFormFactory, GlobalSettings, dialogs, FileManagerApi) {

            var self = this;
            var crudApi = new CrudApi('api/UploadedMessageReceiver');

            var DataValidCondition_Valid = 1;
            var DataValidCondition_Invalid = 2;
            var DataValidCondition_All = DataValidCondition_Valid | DataValidCondition_Invalid;

            self.UseParam = false;

            this.selectFile = function (target) {
                var $file = angular.element(target).parent('div').find('input[type=file]');
                $file.click();
                //$('input[type=file]').click(); // 在 SinglePageApplication ，這種方式會出錯
            };

            this.uploadFile = function (target) {

                var $file = angular.element(target);
                //var $file = $('input[type=file]'); // 在 SinglePageApplication ，這種方式會出錯

                var $scope = angular.element($file).scope();

                // 由於這個function的呼叫是來自 angular.element(this).scope().fileNameChanged()，必須要使用 $scope.$apply 才能達到 $scope two way binding
                $scope.$apply(function () {

                    var url = '/FileManagerApi/UploadMessageReceiverList';
                    var extensionPattern = /^(xlsx|csv|zip)$/i;
                    var extraParameters = {UseParam: self.UseParam || false};

                    //var $file = $('input[type=file]');

                    if (!FileManagerApi.checkFile($file, extensionPattern)) {
                        $file.val(""); // 清空 input[type=file]，避免對同一個檔案上傳無法觸發onchange事件
                        return false;
                    }

                    $scope.uploading = true;
                    FileManagerApi.uploadFile(url, $file, extraParameters)
                    .then(function (result) {
                        var data = result.data;
                        self.uploadedMessageReceiverListResult = data;
                    })
                    .finally(function () {
                        $scope.uploading = false;
                        $file.val(""); // 清空 input[type=file]，避免對同一個檔案上傳無法觸發onchange事件
                    });
                });
            };

            this.editValidList = function () { self.editUploadedMessageReceiver(DataValidCondition_Valid); };
            this.editInvalidList = function () { self.editUploadedMessageReceiver(DataValidCondition_Invalid); };

            this.editUploadedMessageReceiver = function (DataValidCondition) {


                var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

                ////////////////////////////////////////
                // gridModalCtrlOptions
                ////////////////////////////////////////

                

                var gridOptionsNoUseParam = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                    // paginationPageSizes: paginationOptions.pageSizes, // 由 GridModalCtrl 控制
                    //paginationPageSize: paginationOptions.pageSize, // 由 GridModalCtrl 控制
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableColumnMenus: false,
                    columnDefs: [{
                        name: 'RowNo',
                        width: '50',
                        displayName: '#'
                    },
                    {
                        name: 'Name',
                        displayName: '姓名'
                    },
                    {
                        name: 'Mobile',
                        displayName: '手機門號'
                    },
                    {
                        name: 'Email',
                        displayName: '電子郵件'
                    },
                    {
                        name: 'SendTime',
                        displayName: '發送時間',
                        cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.SendTime ? (row.entity.SendTime | UtcToLocalTimeFilter: "YYYYMMDDHHmm" ) : "依發送時間設定" }}</div>',
                    },
                    {
                        name: 'Region',
                        displayName: '發送地區'
                    },
                    {
                        name: 'Update',
                        width: '50',
                        displayName: '',
                        cellEditableCondition: false,
                        cellClass: 'grid-align-center',
                        cellTemplate: [
                        '<img class="gg01" ng-click="grid.appScope.editRow(row.entity)"/>',
                        '<img class="gg02" ng-click="grid.appScope.deleteRow(row.entity)"/>',
                        ].join('\n'),
                    },
                    ],
                    data: [],
                    //onRegisterApi: function (gridApi) {}, // 由 GridModalCtrl 控制
                };

                var gridOptionsUseParam = {
                    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                    // paginationPageSizes: paginationOptions.pageSizes, // 由 GridModalCtrl 控制
                    //paginationPageSize: paginationOptions.pageSize, // 由 GridModalCtrl 控制
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableColumnMenus: false,
                    columnDefs: [{
                        name: 'RowNo',
                        width: '50',
                        displayName: '#'
                    },
                    {
                        name: 'Name',
                        displayName: '姓名'
                    },
                    {
                        name: 'Mobile',
                        displayName: '手機門號'
                    },
                    {
                        name: 'Email',
                        displayName: '電子郵件'
                    },
                    {
                        name: 'SendTime',
                        displayName: '發送時間',
                        cellTemplate: '<div class="ui-grid-cell-contents">{{ row.entity.SendTime ? (row.entity.SendTime | UtcToLocalTimeFilter: "YYYYMMDDHHmm" ) : "依發送時間設定" }}</div>',
                    },
                    {
                        name: 'Param1',
                        displayName: '參數一'
                    },
                    {
                        name: 'Param2',
                        displayName: '參數二'
                    },
                    {
                        name: 'Param3',
                        displayName: '參數三'
                    },
                    {
                        name: 'Param4',
                        displayName: '參數四'
                    },
                    {
                        name: 'Param5',
                        displayName: '參數五'
                    },
                    {
                        name: 'Update',
                        width: '50',
                        displayName: '',
                        cellEditableCondition: false,
                        cellClass: 'grid-align-center',
                        cellTemplate: [
                        '<img class="gg01" ng-click="grid.appScope.editRow(row.entity)"/>',
                        '<img class="gg02" ng-click="grid.appScope.deleteRow(row.entity)"/>',
                        ].join('\n'),
                    },
                    ],
                    data: [],
                    //onRegisterApi: function (gridApi) {}, // 由 GridModalCtrl 控制
                };

                var gridOptions = self.UseParam ? gridOptionsUseParam : gridOptionsNoUseParam;

                if ((DataValidCondition & DataValidCondition_Invalid) == DataValidCondition_Invalid) {
                    // http://stackoverflow.com/questions/586182/how-do-i-insert-an-item-into-an-array-at-a-specific-index
                    gridOptions.columnDefs.splice(gridOptions.columnDefs.length - 1, 0, {
                        name: 'InvalidReason',
                        displayName: '無效原因'
                    });
                }

                ////////////////////////////////////////
                // formModalCtrlOptions
                ////////////////////////////////////////

                var schemaForm = SchemaFormFactory.create('UploadedMessageReceiverModel', {UseParam: self.UseParam});

                var title = function (model) {
                    return !angular.isDefined(model.Id) ? "新增收訊人" : "編輯收訊人";
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

                var notify = function (type, args) {
                    if (type == 'createRow' ||
                       type == 'updateRow' ||
                       type == 'deleteRow' ||
                       type == 'deleteSelection')
                    {
                        var criteria = {
                            SearchText: '',
                            UploadedSessionId: self.uploadedMessageReceiverListResult.UploadedSessionId,
                            DataValidCondition: DataValidCondition_All,
                            PageIndex: 1,
                            PageSize: -1, // 抓取全部，目的在正確更新有效與無效筆數
                        };

                        crudApi.GetAll(criteria)
                        .then(function (result) {
                            var data = result.data;

                            var totalCount = data.Result.length;
                            var validCount = (function () {
                                var cnt = 0;
                                for (var i = 0; i < totalCount; i++) {
                                    var item = data.Result[i];
                                    if (item.IsValid == true)
                                        cnt++;
                                }
                                return cnt;
                            })();
                            var invalidCount = totalCount - validCount;

                            if (self.uploadedMessageReceiverListResult) {
                                self.uploadedMessageReceiverListResult.ValidCount = validCount;
                                self.uploadedMessageReceiverListResult.InvalidCount = invalidCount;
                            }
                        });
                    }
                };

                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/gridModal.html',
                    controller: 'GridModalCtrl',
                    windowClass: 'center-modal',
                    //size: self.UseParam ? 'lg' : void 0,
                    size: 'lg',
                    resolve: {
                        gridModalCtrlOptions: function () {
                            return {
                                extraCriteria: {
                                    UploadedSessionId: self.uploadedMessageReceiverListResult.UploadedSessionId,
                                    DataValidCondition: DataValidCondition,
                                },
                                title: DataValidCondition == DataValidCondition_Valid ? '上傳名單結果(有效名單)' : '上傳名單結果(無效名單)',
                                gridOptions: gridOptions,
                                crudApi: crudApi,
                                disableCreate: false,
                                disableMultiDelete: false,
                                notify: notify,
                                onCreating: function (model) {
                                    // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                                    model.UploadedSessionId = self.uploadedMessageReceiverListResult.UploadedSessionId;
                                    model.UseParam = self.UseParam;
                                },
                                onUpdating: function (model) {
                                    // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                                },
                                onDeleting: function (model) {
                                    // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                                },
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

            this.cancel = function () {
                var message = "確定取消上傳資料嗎？";
                dialogs.confirm('載入大量名單', message).result.then(function (btn) {
                    self.uploadedMessageReceiverListResult = null;
                });
            };

            this.init = function () {
                self.uploadedMessageReceiverListResult = null;
            };

            //========================================
            // Events & EventHandlers
            //========================================

            this.uploadedMessageReceiverListResult = null;
            this.AddSelfToMessageReceiverList = false;


        }]);

})(window, document);
