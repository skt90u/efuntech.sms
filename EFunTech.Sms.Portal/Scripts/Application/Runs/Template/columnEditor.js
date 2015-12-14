(function (window, document) {
    'use strict';

    angular.module('app').run(['$templateCache', function ($templateCache) {

        var markup = [
            '<style>',
                '.ColumnEditor ul {',
                  'columns: 3;',
                  '-webkit-columns: 3;',
                  '-moz-columns: 3;',
                  'list-style-type: none;',
                '}',
            '</style>',
            '<div class="ColumnEditor">',
                '<div class="modal-header">',
                    '<h3 class="modal-title">{{title}}</h3>',
                '</div>',
                '<div class="modal-body">',
                    '<h4>請勾選以下欄位項目(最多勾選{{maxColumns}}項):</h4>',
                    '<ul>',
                        '<li ng-repeat="columnDef in columnDefs">',
                            '<label>',
                                '<input type="checkbox" ng-model="columnDef.visible" ng-click="checkMaxColumns(columnDef)" />',
                                '{{columnDef.displayName}}',
                            '</label>	',
                        '</li>',
                    '</ul>',
                '</div>',
                '<div class="modal-footer">',
                    '<button class="btn btn-success" ng-click="submit()">確定</button>',
                    '<button class="btn btn-warning" ng-click="cancel()">取消</button>',
                '</div>',
            '</div>',

        ].join('\n');

        $templateCache.put("template/modal/columnEditor.html", markup);
    }]);

})(window, document);