(function (window, document) {
    'use strict';

    angular.module('app').run(['$templateCache', function ($templateCache) {

        var markup = [
            '<div>',
                '<div class="modal-header">',
                    '<h3 class="modal-title">成員搜尋條件</h3>',
                '</div>',
                '<div class="modal-body">',
                    '<form role="form">',
                        '<div class="form-group">',
                            '<label>部門</label>',
                            '<select class="form-control"',
                                    'ng-options="department.Name for department in deaprtments"',
                                    'ng-model="selectedDepartment"',
                                    'ng-change="onChangeDepartment()"></select>',
                        '</div>',
                        '<div class="form-group">',
                            '<label>成員</label>',
                            '<div class="search-area">',
                                '<div class="input-group add-on" style="width:100%">',
                                    '<input type="text" class="form-control" ng-model="searchText" placeholder="關鍵字搜尋" />',
                                    '<div class="input-group-btn">',
                                        '<button class="btn btn-default" ng-click="search()"><i class="glyphicon glyphicon-search"></i></button>',
                                    '</div>',
                                '</div>',
                            '</div>',
                            '<div class="grid"',
                                 'ui-grid="gridOptions"',
                                 'ui-grid-selection',
                                 'ui-grid-pagination',
                                 'ui-grid-resize-columns',
                                 'ui-grid-move-columns',
                                 'ui-grid-pinning',
                                 'ui-grid-exporter',
                                 'ui-grid-auto-resize>',
                                '<div class="watermark" ng-show="!gridOptions.data.length">查無資料</div>',
                            '</div>',
                        '</div>',
                    '</form>',
                '</div>',
                '<div class="modal-footer">',
                    '<button class="btn btn-success" ng-click="submit()">確定</button>',
                    '<button class="btn btn-warning" ng-click="cancel()">取消</button>',
                '</div>',
            '</div>',

        ].join('\n');

        $templateCache.put("template/modal/memberSelector.html", markup);
    }]);

})(window, document);


