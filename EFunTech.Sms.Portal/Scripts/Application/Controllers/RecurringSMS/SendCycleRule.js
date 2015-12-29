(function (window, document) {
    'use strict';

    angular.module('app').controller('SendCycleRule', ['$scope', 'CurrentUserManager', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'LookupApi', 'ValidationApi', '$q', 'SchemaFormHelper', '$translate', 'dialogs',
        'SelectUtil', 'SendTimeType',
        function ($scope, CurrentUserManager, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, LookupApi, ValidationApi, $q, SchemaFormHelper, $translate, dialogs,
            SelectUtil, SendTimeType) {

        //========================================
        // Settings
        //========================================

        var SendTimeTypeOptions = SelectUtil.getEnumOptions(SendTimeType);

        var crudApi = new CrudApi('api/SendMessageRule');
        var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

        $scope.gridOptions = {
            // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
            paginationPageSizes: paginationOptions.pageSizes,
            paginationPageSize: paginationOptions.pageSize,
            useExternalPagination: true,
            useExternalSorting: true,
            enableColumnMenus: false,
            columnDefs: [{
                name: 'SendTitle',
                displayName: '簡訊類別描述'
            },
            {
                name: 'SendBody',
                displayName: '發送內容'
            },
            {
                name: 'StartDate',
                displayName: '起始時間',
                cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.StartDate | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
            },
            {
                name: 'EndDate',
                displayName: '結束時間',
                cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.EndDate | UtcToLocalTimeFilter: "YYYY/MM/DD HH:mm:ss" }}</div>',
            },
            {
                name: 'CycleString',
                displayName: '週期'
            },
            {
                name: 'Update',
                width: '50',
                displayName: '',
                cellEditableCondition: false,
                cellClass: 'grid-align-center',
                cellTemplate: '<img class="edit" ng-click="grid.appScope.editRow(row.entity)"/>'
            },
            {
                name: 'Delete',
                width: '50',
                displayName: '',
                cellEditableCondition: false,
                cellClass: 'grid-align-center',
                cellTemplate: '<img class="lightbulb_48" ng-click="grid.appScope.deleteRow(row.entity)"/>'
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
                SendTimeType: SendTimeTypeOptions.Cycle,
                SearchText: $scope.searchText,
                PageIndex: paginationOptions.pageNumber,
                PageSize: paginationOptions.pageSize
            };

            crudApi.GetAll(criteria).then(function (result) {
                if ($scope.gridApi) $scope.gridApi.selection.clearSelectedRows();

                var data = result.data;
                $scope.gridOptions.totalItems = data.TotalCount;
                $scope.gridOptions.data = data.Result;
            });
        };

        $scope.editRow = function (rowEntity) {

            var model = angular.copy(rowEntity ? (rowEntity[0] || rowEntity) : {}); // <--- angular.copy用以避免影響GridRow
            var isNew = !angular.isDefined(model.Id);
            if (isNew) {
                // TODO: 設定新增資料的初始值
            }

            ////////////////////////////////////////
            // SendRuleModalCtrlOptions
            ////////////////////////////////////////

            var title = '檢視週期簡訊';

            var SendMessageRule = model;

            var gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                //paginationPageSizes: paginationOptions.pageSizes, // 由 SendRuleModalCtrl 控制
                //paginationPageSize: paginationOptions.pageSize, // 由 SendRuleModalCtrl 控制
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                //{
                //    name: 'SendMessageType',
                //    width: '80',
                //    displayName: '訊息類型',
                //    cellEditableCondition: false,
                //    cellClass: 'grid-align-center',
                //    cellTemplate: [
                //        '<img class="smsMessage" ng-show="row.entity.SendMessageType === 0" />',
                //        '<img class="appMessage" ng-show="row.entity.SendMessageType === 1" />',
                //    ].join('\n'),
                //},
                {
                    name: 'Name',
                    displayName: '收訊者姓名'
                },
                {
                    name: 'Mobile',
                    displayName: '收訊者門號'
                },
                {
                    name: 'Email',
                    displayName: '收訊者信箱'
                },
                //{
                //    name: 'SendTime',
                //    displayName: '預約時間',
                //    cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.SendTime || grid.appScope.SendMessageRule.SendTime}}</div>',
                //},
                //{
                //    name: 'MessageCost',
                //    displayName: '發送扣點'
                //},
                //{
                //    name: 'Update',
                //    width: '50',
                //    displayName: '',
                //    cellEditableCondition: false,
                //    cellClass: 'grid-align-center',
                //    cellTemplate: '<img class="edit" ng-click="grid.appScope.editRow(row.entity)"/>'
                //},
                {
                    name: 'Delete',
                    width: '50',
                    displayName: '',
                    cellEditableCondition: false,
                    cellClass: 'grid-align-center',
                    cellTemplate: '<img class="lightbulb_48" ng-click="grid.appScope.deleteRow(row.entity)"/>'
                },
                ],
                data: [],
                //onRegisterApi: function (gridApi) {}, // 由 SendRuleModalCtrl 控制
            };

            var sendMessageRuleCrudApi = new CrudApi('api/SendMessageRule');
            var messageReceiverCrudApi = new CrudApi('api/MessageReceiver');

            var notify = function (type, parameters) {
                $scope.search();
            };

            var modalInstance = $modal.open({
                templateUrl: 'template/modal/sendRuleModal.html',
                controller: 'SendRuleModalCtrl',
                windowClass: 'center-modal',
                //size: 'lg',
                resolve: {
                    sendRuleModalCtrlOptions: function () {
                        return {
                            title: title,
                            SendMessageRule: SendMessageRule, // 串到 html, 目前沒有改成小寫的打算
                            gridOptions: gridOptions,
                            sendMessageRuleCrudApi: sendMessageRuleCrudApi,
                            messageReceiverCrudApi: messageReceiverCrudApi,
                            extraCriteria: { SendMessageRuleId: SendMessageRule.Id },
                            notify: notify,
                        };
                    },
                    // formModalCtrlOptions
                }
            });

            //modalInstance.result.then(function (savedModel) {
            //    if (!savedModel) return;
            //    var isNew = !angular.isDefined(savedModel.Id);
            //    if (isNew) {
            //        $scope.createRow(savedModel);
            //    }
            //    else {
            //        $scope.updateRow(savedModel);
            //    }
            //},
            //function () {
            //    $log.debug('Modal dismissed at: ' + new Date());
            //});
        };

        $scope.createRow = function (model) {
            crudApi.Create(model).then(function () {
                $scope.search();
            });
        };

        $scope.updateRow = function (model) {
            crudApi.Update(model).then(function () {
                $scope.search();
            });
        };

        $scope.deleteRow = function (rowEntity) {
            var selectedRow = rowEntity[0] || rowEntity;
            var model = selectedRow;
            var message = "確定刪除發送內容『" + model.SendBody + "』？";
            dialogs.confirm('刪除資料', message).result.then(function (btn) {
                crudApi.Delete(selectedRow).then(function () {
                    // 更新使用者點數
                    CurrentUserManager.updateSmsBalance();

                    $scope.search();
                });
            });
        };

        $scope.deleteSelection = function () {
            var selectedRows = $scope.gridApi.selection.getSelectedRows();

            if (selectedRows.length == 0) {
                dialogs.error('刪除資料', '請先選擇欲刪除之資料');
                return;
            }

            var message = "確定刪除這" + selectedRows.length + "筆資料？";
            dialogs.confirm('刪除資料', message).result.then(function (btn) {
                crudApi.Delete(selectedRows).then(function () {
                    // 更新使用者點數
                    CurrentUserManager.updateSmsBalance();

                    $scope.search();
                });
            });
        };

        //========================================
        // Events & EventHandlers
        //========================================

        $scope.$on('tab.onSelect', function (event, tabName) {
            if (tabName !== 'SendCycleRule') return;
            $scope.search();
        });

        //========================================
        // Initialize
        //========================================

        $scope.searchText = '';

        if (GlobalSettings.isSPA) {
            $scope.$on('menu.onSelect', function (event, menuName) {
                if (menuName !== 'RecurringSMS') return;
                var tabName = $scope.$parent.tabName;
                if (tabName !== 'SendCycleRule') return;
                $scope.search();
            });
        }

    }]);

})(window, document);