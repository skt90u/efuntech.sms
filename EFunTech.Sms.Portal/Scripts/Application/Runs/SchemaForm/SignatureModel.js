(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', function (SchemaFormCache, RegularExpressionPatterns) {

        SchemaFormCache.put('SignatureModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    Subject: {
                        type: "string",
                        title: "標題",
                    },
                    Content: {
                        type: "string",
                        title: "內容",
                        required: true,
                    },
                }
            };

            var form = [{
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-12",
                    items: [{
                        key: "Subject",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            }, {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-12",
                    items: [{
                        key: "Content",
                        type: "textarea",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            },
            ];

            return {
                schema: schema,
                form: form,
            };

        }); // $cacheFactory('SchemaFormFactory').put('SignatureModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);