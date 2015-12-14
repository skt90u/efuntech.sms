/// <reference path="GridModalCtrl.js" />
(function (window, document) {
    'use strict';

    //========================================
    // GridModalCtrl
    //  共用單筆資料編輯 Controller
    //
    // @param schema: sf-schema 物件
    // @param form: sf-form 物件
    // @param model: sf-model 物件
    // @param options: sf-options 物件
    // @param validateBeforeSubmit: 存檔前，執行驗證輸入數值驗證的 function
    //========================================
    angular.module('app').
    controller('GridModalCtrl', ['$scope', '$modalInstance', '$rootScope', '$log', 'GlobalSettings', '$modal', 'SchemaFormFactory', '$translate', 'dialogs', 'gridModalCtrlOptions', 'formModalCtrlOptions',
    function ($scope, $modalInstance, $rootScope, $log, GlobalSettings, $modal, SchemaFormFactory, $translate, dialogs, gridModalCtrlOptions, formModalCtrlOptions) {

        //========================================
        // Settings
        //========================================

        var crudApi = gridModalCtrlOptions.crudApi;
        var extraCriteria = gridModalCtrlOptions.extraCriteria || {};
        var paginationOptions = angular.copy(GlobalSettings.paginationOptions);
        
        $scope = angular.extend($scope, {
            //title: '資料列表',
            //gridOptions: {
            //    // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
            //    // paginationPageSizes: paginationOptions.pageSizes, // 由 GridModalCtrl 控制
            //    //paginationPageSize: paginationOptions.pageSize, // 由 GridModalCtrl 控制
            //    useExternalPagination: true,
            //    useExternalSorting: true,
            //    enableColumnMenus: false,
            //    columnDefs: [{
            //        name: 'Subject',
            //        displayName: '標題'
            //    }],
            //    data: [],
            //    //onRegisterApi: function (gridApi) {}, // 由 GridModalCtrl 控制
            //},
            //crudApi: new CrudApi('api/CommonMessage'),
            disableCreate: false, // 是否不允許新增資料
            disableMultiDelete: false, // 是否不允許多筆刪除資料
            //onCreating: angular.noop,
            //onUpdating: angular.noop,
            //onDeleting: angular.noop,
        }, gridModalCtrlOptions);

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
            //var criteria = {
            //    SearchText: $scope.searchText,
            //    PageIndex: paginationOptions.pageNumber,
            //    PageSize: paginationOptions.pageSize
            //};

            var criteria = angular.extend({}, {
                SearchText: $scope.searchText,
                PageIndex: paginationOptions.pageNumber,
                PageSize: paginationOptions.pageSize
            }, extraCriteria);

            crudApi.GetAll(criteria).then(function (result) {
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
            crudApi.Create(model).then(function () {
                $scope.search();
                $scope.notify('createRow');
            });
        };

        $scope.updateRow = function (model) {
            ($scope.onUpdating || angular.noop)(model); // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
            crudApi.Update(model).then(function () {
                $scope.search();
                $scope.notify('updateRow');
            });
        };

        $scope.deleteRow = function (rowEntity) {
            var selectedRow = rowEntity[0] || rowEntity;

            var message = "確定刪除選取的資料？";
            dialogs.confirm('刪除資料', message).result.then(function (btn) {
                ($scope.onDeleting || angular.noop)(selectedRow); // 在執行 CrudApi 之前，給外部 controller 加入參數的機會
                crudApi.Delete(selectedRow)
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
                crudApi.Delete(selectedRows)
                .then(function () {
                    $scope.search();
                    $scope.notify('deleteSelection');
                });
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        //========================================
        // Events & EventHandlers
        //========================================

        $scope.notify = function (type, parameters) {
            (gridModalCtrlOptions.notify || angular.noop)(type, parameters);
        };

        //========================================
        // Initialize
        //========================================

        $scope.searchText = '';

        $scope.search();
    }]);

})(window, document);