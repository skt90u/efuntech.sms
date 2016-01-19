(function (window, document) {
    'use strict';

    angular.module('app').controller('SendRuleModalCtrl', ['$scope',
        'GlobalSettings', 'dialogs', 'CrudApi', 'MessageCostInfo', 
        'SelectUtil', 'RecipientFromType', 'SendCustType', 'SendMessageType',
        '$modalInstance', 'sendRuleModalCtrlOptions',
        function ($scope,
            GlobalSettings, dialogs, CrudApi, MessageCostInfo, 
            SelectUtil, RecipientFromType, SendCustType, SendMessageType,
            $modalInstance, sendRuleModalCtrlOptions) {

            //========================================
            // Settings
            //========================================

            var RecipientFromTypeOptions = SelectUtil.getEnumOptions(RecipientFromType);
            var SendCustTypeOptions = SelectUtil.getEnumOptions(SendCustType);
            var SendMessageTypeOptions = SelectUtil.getEnumOptions(SendMessageType);

            var sendMessageRuleCrudApi = sendRuleModalCtrlOptions.sendMessageRuleCrudApi;
            var messageReceiverCrudApi = sendRuleModalCtrlOptions.messageReceiverCrudApi;
            var extraCriteria = sendRuleModalCtrlOptions.extraCriteria || {};

            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            $scope = angular.extend($scope, {
                //title: '資料列表',
                //SendMessageRule: {},
                //gridOptions: {},
                //sendMessageRuleCrudApi: new CrudApi('api/SendMessageRule'),
                //messageReceiverCrudApi: new CrudApi('api/MessageReceiver'),
                //extraCriteria: { SendMessageRuleId: SendMessageRule.Id }
                disableCreate: false, // 是否不允許新增資料
                disableMultiDelete: false, // 是否不允許多筆刪除資料
                //onCreating: angular.noop,
                //onUpdating: angular.noop,
                //onDeleting: angular.noop,
            }, sendRuleModalCtrlOptions);

            $scope.OriginalSendMessageRule = angular.copy(sendRuleModalCtrlOptions.SendMessageRule || {});
            
            $scope.gridOptions.paginationPageSizes = paginationOptions.pageSizes;
            $scope.gridOptions.paginationPageSize = paginationOptions.pageSize;
            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    paginationOptions.pageNumber = newPage;
                    paginationOptions.pageSize = pageSize;
                    $scope.search();
                });
            };

            //========================================
            // Functions
            //========================================

            $scope.search = function () {
                var criteria = angular.extend({}, {
                    SearchText: $scope.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                }, extraCriteria)

                messageReceiverCrudApi.GetAll(criteria).then(function (result) {
                    if ($scope.gridApi) $scope.gridApi.selection.clearSelectedRows();

                    var data = result.data;
                    $scope.gridOptions.totalItems = data.TotalCount;
                    $scope.gridOptions.data = data.Result;
                });
            };

            $scope.editRow = function (rowEntity) {

                var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow

                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/editRow.html',
                    controller: 'FormModalCtrl',
                    windowClass: 'center-modal',
                    //size: size,
                    resolve: {
                        options: function () {
                            var options = angular.copy(formModalCtrlOptions);
                            options.model = model;
                            return options;
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
                },
                function () {
                    $log.debug('Modal dismissed at: ' + new Date());
                });
            };

            $scope.createRow = function (model) {
                ($scope.onCreating || angular.noop)(model); // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                messageReceiverCrudApi.Create(model).then(function () {
                    $scope.search();
                    $scope.notify('createRow');
                });
            };

            $scope.updateRow = function (model) {
                ($scope.onUpdating || angular.noop)(model); // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                messageReceiverCrudApi.Update(model).then(function () {
                    $scope.search();
                    $scope.notify('updateRow');
                });
            };

            $scope.deleteRow = function (rowEntity) {
                var selectedRow = rowEntity[0] || rowEntity;

                var message = selectedRow.Name ? '確定刪除收訊者 ' + selectedRow.Name + ' ？' :
                              selectedRow.Mobile ? '確定刪除收訊門號 ' + selectedRow.Mobile + ' ？' :
                              "確定刪除選取的資料？";

                dialogs.confirm('刪除資料', message).result.then(function (btn) {
                    ($scope.onDeleting || angular.noop)(selectedRow); // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                    messageReceiverCrudApi.Delete(selectedRow)
                    .then(function () {
                        $scope.search();
                        $scope.notify('deleteRow');
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
                    ($scope.onDeleting || angular.noop)(selectedRows); // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                    messageReceiverCrudApi.Delete(selectedRows)
                    .then(function () {
                        $scope.search();
                        $scope.notify('deleteSelection');
                    });
                });
            };

            $scope.export = function () {
                
                var criteria = angular.extend({}, {
                    SearchText: $scope.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize,
                }, extraCriteria);

                messageReceiverCrudApi.Download(criteria);
            };

            $scope.checkSendBody = function () {

                var messageCostInfo = new MessageCostInfo($scope.SendMessageRule.SendBody, {
                    "@space1@": "",
                    "@space2@": "",
                    "@space3@": "",
                    "@space4@": "",
                    "@space5@": "",
                });

                $scope.SendMessageRule.MessageLength = messageCostInfo.MessageLength; // 簡訊字數
                $scope.SendMessageRule.MessageNum = messageCostInfo.MessageNum; // 簡訊總共幾則
                $scope.SendMessageRule.MessageCost = messageCostInfo.MessageCost; // 簡訊花費點數
                $scope.SendMessageRule.MessageFormatError = messageCostInfo.MessageFormatError;

                if ($scope.SendMessageRule.MessageFormatError) {
                    toaster.pop({
                        type: 'error',
                        title: '發送內容',
                        body: $scope.SendMessageRule.MessageFormatError,
                        showCloseButton: true,
                    });
                }
            };

            $scope.submit = function () {

                // 執行簡訊內容更新作業
                if ($scope.OriginalSendMessageRule.SendBody !== $scope.SendMessageRule.SendBody) {
                    var model = $scope.SendMessageRule;
                    var crudApi = sendMessageRuleCrudApi;
                    crudApi.Update(model).then(function () {
                        $scope.notify('updateRow');
                        $modalInstance.close();
                    });
                }
                else {
                    $modalInstance.close();
                }
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };

            //========================================
            // Events & EventHandlers
            //========================================

            $scope.notify = function (type, parameters) {
                (sendRuleModalCtrlOptions.notify || angular.noop)(type, parameters);
            };

            //========================================
            // Initialize
            //========================================

            $scope.searchText = '';

            $scope.search();

            $scope.checkSendBody();
        }]);

   
})(window, document);
