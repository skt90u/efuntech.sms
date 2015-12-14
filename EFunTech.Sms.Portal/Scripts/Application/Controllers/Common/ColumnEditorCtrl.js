(function (window, document) {
    'use strict';

    //========================================
    // ColumnEditorCtrl (設定欄位顯示隱藏)
    //
    // @param title: 自訂聯絡人資料顯示欄位
    // @param maxColumns: 最多可勾選幾個欄位(不限制的話，請輸入 -1)
    // @param columnDefs: gridOptions.columnDefs
    // @param exclusiveNames: 不要被改變顯示隱藏屬性的欄位，['Mobile', 'Update', 'Delete']
    //========================================
    angular.module('app').controller('ColumnEditorCtrl', ['$scope', '$modalInstance', '$translate', 'dialogs', 'options',
        function ($scope, $modalInstance, $translate, dialogs, options) {

            $scope.title = options.title;

            $scope.maxColumns = options.maxColumns || -1; // 不限制

            //========================================

            // 初始化 columnDefs
             
            var columnDefs = angular.copy(options.columnDefs);
            _.each(columnDefs, function (columnDef) {
                columnDef.visible = columnDef.visible || columnDef.visible === undefined;
            });

            // 排除例外(exclusiveNames，例如：編輯按鈕，刪除按鈕)
            var exclusiveNames = options.exclusiveNames;
            _.each(exclusiveNames, function (exclusiveName) {
                var found = _.findWhere(columnDefs, { name: exclusiveName });
                if (found) {
                    columnDefs = _.without(columnDefs, found);
                }
            });

            $scope.columnDefs = angular.copy(columnDefs);
            $scope.lastColumnDefs = angular.copy(columnDefs);

            $scope.checkMaxColumns = function (columnDef) {
                var maxColumns = $scope.maxColumns;
                if (maxColumns == -1) return;

                var curColumns = _.countBy($scope.columnDefs, function (obj) { return !!obj.visible }).true;
                if (curColumns > maxColumns) {
                    dialogs.error('操作錯誤', '最多勾選' + maxColumns + '項');
                    $scope.columnDefs = $scope.lastColumnDefs;
                }
                else {
                    $scope.lastColumnDefs = angular.copy($scope.columnDefs);
                }
            };

            $scope.submit = function () {

                // 回存到 options.columnDefs
                _.each(options.columnDefs, function (columnDef) {
                    var found = _.findWhere($scope.columnDefs, { name: columnDef.name });
                    if (found) {
                        columnDef.visible = found.visible;
                    }
                });

                $modalInstance.close(options.columnDefs /* 其實並不需要回傳結果 */);

                // 呼叫端請呼叫
                // $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        }]);


})(window, document);