(function (window, document) {
    'use strict';

    angular.module('app').run(['$templateCache', function ($templateCache) {

        var markup = [
            '<form name="modalForm">',
                '<div class="modal-header">',
                    '<h3 class="modal-title">{{title}}</h3>',
                '</div>',
                '<div class="modal-body">',
                    '<div sf-schema="schema" sf-form="form" sf-model="model" sf-options="options"></div>',
                '</div>',
                '<div class="modal-footer">',
                '<div class="btn-group">',
                    '<button class="btn btn-success" ng-click="submit(modalForm)">確定</button>',
                    '<button class="btn btn-warning" ng-click="cancel()">取消</button>',
                '</div>',
                '</div>',
            '</form>'
        ].join('\n');

        $templateCache.put("template/modal/editRow.html", markup);
    }]);

})(window, document);