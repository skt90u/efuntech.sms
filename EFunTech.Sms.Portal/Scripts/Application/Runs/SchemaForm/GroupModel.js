(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', function (SchemaFormCache, RegularExpressionPatterns) {

        SchemaFormCache.put('GroupModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    Name: {
                        type: "string",
                        title: "群組名稱",
                        required: true,
                        maxLength: "100",
                    },
                    Description: {
                        type: "string",
                        title: "群組說明",
                    },
                }
            };

            var form = [{
                type: "section",
                htmlClass: "row",
                items: [
                {
                    type: "section",
                    htmlClass: "col-xs-12",
                    items: [{
                        key: "Name",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }
                ]
            }, {
                type: "section",
                htmlClass: "row",
                items: [
                {
                    type: "section",
                    htmlClass: "col-xs-12",
                    items: [{
                        key: "Description",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            }];

            return {
                schema: schema,
                form: form,
            };

        }); // $cacheFactory('SchemaFormFactory').put('GroupModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);