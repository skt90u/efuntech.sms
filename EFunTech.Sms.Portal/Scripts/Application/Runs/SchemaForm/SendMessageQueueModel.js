(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'GlobalSettings', function (SchemaFormCache, GlobalSettings) {

        SchemaFormCache.put('SendMessageQueueModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    SendMessageType: {
                        type: "string",
                        title: "發送訊息類型(手機簡訊|APP簡訊)",
                        enum: ['0', '1'],
                        required: true,
                    },
                    SendTime: {
                        type: "string",
                        title: "發送時間",
                        required: true,
                    },
                    SendTitle: {
                        type: "string",
                        title: "簡訊類別描述",
                        required: true,
                    },
                    SendBody: {
                        type: "string",
                        title: "發送內容",
                        required: true,
                    },
                    SendCustType: {
                        type: "string",
                        title: "單向|雙向 簡訊發送",
                        enum: ['0', '1'],
                        required: true,
                    },
                    TotalReceiverCount: {
                        type: "integer",
                        title: "發送通數",
                        required: true,
                    },
                    TransmissionCount: {
                        type: "integer",
                        title: "傳送中通數(當使用電信商API發送內容的時候，此欄位加1)",
                        required: true,
                    },
                    SuccessCount: {
                        type: "integer",
                        title: "成功接收通數(當接收電信商回報成功訊息，此欄位加一，並把傳送中通數減一)",
                        required: true,
                    },
                    FailureCount: {
                        type: "integer",
                        //title: "逾期收訊(當接收電信商回報失敗訊息，此欄位加一，並把傳送中通數減一)",
                        title: "傳送失敗(當接收電信商回報失敗訊息，此欄位加一，並把傳送中通數減一)",
                        required: true,
                    },
                    TotalMessageCost: {
                        type: "string",
                        title: "花費點數(發送所需點數)",
                        required: true,
                    },
                    SendMessageRuleId: {
                        type: "integer",
                        title: "簡訊發送規則",
                        required: true,
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
                        key: "SendMessageType",
                        type: "radios-inline",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                        titleMap: [{
                            value: '0',
                            name: 'SmsMessage'
                        },
                        {
                            value: '1',
                            name: 'AppMessage'
                        },
                        ],
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "SendTime",
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
                        key: "SendTitle",
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
                        key: "SendBody",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
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
                        key: "SendCustType",
                        type: "radios-inline",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                        titleMap: [{
                            value: '0',
                            name: 'OneWay'
                        },
                        {
                            value: '1',
                            name: 'TwoWay'
                        },
                        ],
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "TotalReceiverCount",
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
                        key: "TransmissionCount",
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
                        key: "SuccessCount",
                        type: "number",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
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
                        key: "FailureCount",
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
                        key: "TotalMessageCost",
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
                        key: "SendMessageRuleId",
                        type: "number",
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

        }); // $cacheFactory('SchemaFormFactory').put('SendMessageQueueModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);