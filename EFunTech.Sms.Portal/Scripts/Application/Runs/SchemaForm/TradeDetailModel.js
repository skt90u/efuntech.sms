(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'GlobalSettings', function (SchemaFormCache, GlobalSettings) {

        SchemaFormCache.put('TradeDetailModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    TradeTime: {
                        type: "string",
                        title: "交易時間",
                    },
                    TradeType: {
                        type: "string",
                        title: "交易類別",
                        enum: ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'],
                        required: true,
                    },
                    Point: {
                        type: "string",
                        title: "交易點數",
                    },
                    Remark: {
                        type: "string",
                        title: "交易說明",
                    },
                    OwnerId: {
                        type: "string",
                        title: "OwnerId",
                        required: true,
                    },
                    TargetId: {
                        type: "string",
                        title: "TargetId",
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
                        key: "TradeTime",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "TradeType",
                        type: "radios-inline",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                        titleMap: [{
                            value: '0',
                            name: 'All'
                        },
                        {
                            value: '1',
                            name: 'DeductionOfSendMessage'
                        },
                        {
                            value: '2',
                            name: 'CoverOfSendMessage'
                        },
                        {
                            value: '3',
                            name: 'Deposit'
                        },
                        {
                            value: '4',
                            name: 'Cover'
                        },
                        {
                            value: '5',
                            name: 'ExportPoints'
                        },
                        {
                            value: '6',
                            name: 'ImportPoints'
                        },
                        {
                            value: '7',
                            name: 'MonthlyFeeDeductionOfInteractiveNewsletter'
                        },
                        {
                            value: '8',
                            name: 'BudgetDeductionOfInteractiveNewsletter'
                        },
                        {
                            value: '9',
                            name: 'CoveringBudgetOfInteractiveNewsletter'
                        },
                        ],
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "Point",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-4",
                    items: [{
                        key: "Remark",
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
                        key: "OwnerId",
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
                        key: "TargetId",
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

        }); // $cacheFactory('SchemaFormFactory').put('TradeDetailModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);