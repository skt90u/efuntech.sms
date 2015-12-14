(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', function (SchemaFormCache, RegularExpressionPatterns) {

        SchemaFormCache.put('DepartmentPointManagerManuallyModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    point: {
                        type: "integer",
                        title: "匯入點數",
                        required: true,
                    },
                }
            }

            var form = [
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-12",
                            items: [
                            {
                                key: "point",
                                type: "number",
                                // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                htmlClass: 'has-feedback',
                            },
                            ]
                        },
                    ]
                },
            ];

            return { schema: schema, form: form, };

        }); // $cacheFactory('SchemaFormFactory').put('DepartmentPointManagerManuallyModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);