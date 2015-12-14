(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'GlobalSettings', function (SchemaFormCache, GlobalSettings) {

        SchemaFormCache.put('DepartmentModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    Name: {
                        type: "string",
                        title: "名稱",
                        required: true,
                    },
                    Description: {
                        type: "string",
                        title: "說明",
                        required: true,
                    },
                }
            };

            var form = [{
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Name",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Description",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                ]
            },
            ];

            return {
                schema: schema,
                form: form,
            };

        }); // $cacheFactory('SchemaFormFactory').put('DepartmentModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);