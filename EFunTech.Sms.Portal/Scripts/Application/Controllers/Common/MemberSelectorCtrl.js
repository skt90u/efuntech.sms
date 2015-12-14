(function (window, document) {
    'use strict';

    //========================================
    // MemberSelectorCtrl (選擇要匯出報表的成員)
    //
    //========================================
    angular.module('app').controller('MemberSelectorCtrl', ['$scope', '$modalInstance', '$translate', 'dialogs', 'GlobalSettings', 'CrudApi',
        function ($scope, $modalInstance, $translate, dialogs, GlobalSettings, CrudApi) {

            //========================================
            // Settings
            //========================================

            var crudApi = new CrudApi('api/DepartmentUser');
            var paginationOptions = angular.copy(GlobalSettings.paginationOptions);

            $scope.departments = [];
            $scope.selectedDepartment = null;
            $scope.searchText = '';
            $scope.gridOptions = {
                // http://ui-grid.info/docs/#/api/ui.grid.class:GridOptions
                paginationPageSizes: paginationOptions.pageSizes,
                paginationPageSize: paginationOptions.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableColumnMenus: false,
                columnDefs: [
                    { name: 'EmployeeNo', displayName: '員工編號' },
                    { name: 'UserName', displayName: '帳號' },
                    { name: 'FullName', displayName: '姓名' },
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

            $scope.init = function () {
                
                // 查詢目前使用者部門
                var crudApi = new CrudApi('api/Department');
                var criteria = {
                    SearchText: '',
                    PageIndex: 1,
                    PageSize: -1, // 抓取全部，目的在正確更新有效與無效筆數
                };

                crudApi.GetAll(criteria).then(function (result) {
                    var data = result.data;
                    $scope.deaprtments = data.Result;

                    $scope.deaprtments.unshift({ Name: '所有部門', Id: -1 });

                    if ($scope.deaprtments.length != 0) {
                        $scope.selectedDepartment = $scope.deaprtments[0];
                        $scope.onChangeDepartment();
                    }
                });
            };

            $scope.onChangeDepartment = function () {
                $scope.searchText = ''
                $scope.search();
            };

            $scope.search = function () {
                var criteria = {
                    DepartmentId: $scope.selectedDepartment.Id,
                    SearchText: $scope.searchText,
                    PageIndex: paginationOptions.pageNumber,
                    PageSize: paginationOptions.pageSize
                };

                crudApi.GetAll(criteria)
                .then(function (result) {
                    if ($scope.gridApi) $scope.gridApi.selection.clearSelectedRows();

                    var data = result.data;
                    $scope.gridOptions.totalItems = data.TotalCount;
                    $scope.gridOptions.data = data.Result;
                });
            };

            $scope.submit = function () {

                var selectedRows = $scope.gridApi.selection.getSelectedRows();

                if (selectedRows.length == 0) {
                    dialogs.error('操作錯誤', '請至少選擇一名成員');
                    return;
                }

                $modalInstance.close(selectedRows);
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };

            //========================================
            // Initialize
            //========================================

            $scope.init();

        }]);


})(window, document);