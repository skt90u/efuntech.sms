(function (window, document) {
    'use strict';

    angular.module('app').run(['$templateCache', function ($templateCache) {

        var markup = [
            '<form name="modalForm">',
                '<div class="modal-header">',
                    '<h3 class="modal-title">{{title}}</h3>',
                '</div>',
                '<div class="modal-body">',
                    '<div class="search-area">',
                        '<div class="input-group add-on">',
                            '<input type="text" class="form-control" ng-model="searchText" placeholder="關鍵字搜尋" />',
                            '<div class="input-group-btn">',
                                '<button class="btn btn-default" ng-click="search()"><i class="glyphicon glyphicon-search"></i></button>',
                                '<button class="btn btn-info" ng-if="!disableCreate" ng-click="editRow(null)">新增</button>',
                                '<button class="btn btn-danger" ng-if="!disableMultiDelete" ng-click="deleteSelection()">刪除</button>',
                            '</div>',
                        '</div>',
                    '</div>	',
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
                '<div class="modal-footer">',
                '<div class="btn-group">',
                    '<button class="btn btn-warning" ng-click="cancel()">關閉</button>',
                '</div>',
                '</div>',
            '</form>'
        ].join('\n');

        $templateCache.put("template/modal/gridModal.html", markup);
    }]);

})(window, document);