(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'GlobalSettings', function (SchemaFormCache, GlobalSettings) {

        SchemaFormCache.put('DeliveryReportQueueModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    SendMessageQueueId: {
                        type: "integer",
                        title: "SendMessageQueueId",
                        required: true,
                    },
                    RequestId: {
                        type: "string",
                        title: "發送簡訊識別碼(RequestId), 格式範例. 14348799713001264",
                        required: true,
                        maxLength: "256",
                    },
                    ProviderName: {
                        type: "string",
                        title: "簡訊供應商, 目前有 Every8dSmsProvider、InfobipSmsProvider",
                        required: true,
                        maxLength: "256",
                    },
                    CreatedTime: {
                        type: "string",
                        title: "建立時間",
                    },
                    SendMessageResultItemCount: {
                        type: "integer",
                        title: "SendMessageResultItemCount",
                    },
                    DeliveryReportCount: {
                        type: "integer",
                        title: "DeliveryReportCount",
                    },
                }
            };

            var form = [{
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "SendMessageQueueId",
                        type: "number",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "RequestId",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "ProviderName",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "CreatedTime",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                ]
            },
            {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "SendMessageResultItemCount",
                        type: "number",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "DeliveryReportCount",
                        type: "number",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
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

        }); // $cacheFactory('SchemaFormFactory').put('DeliveryReportQueueModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);