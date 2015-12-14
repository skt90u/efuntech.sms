(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', 'MobileUtil', function (SchemaFormCache, RegularExpressionPatterns, MobileUtil) {

        SchemaFormCache.put('UploadedMessageReceiverModel', function (options) {

            var useParam = options.UseParam || false;

            var schema = {
                type: "object",
                properties: {
                    RowNo: {
                        type: "integer",
                        title: "資料編號",
                    },
                    Name: {
                        type: "string",
                        title: "姓名",
                    },
                    Mobile: {
                        type: "string",
                        title: "手機門號",
                    },
                    Email: {
                        type: "string",
                        title: "電子郵件",
                        pattern: RegularExpressionPatterns.Email.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.Email.errorMessage,
                        },
                    },
                    SendTimeString: {
                        type: "string",
                        title: "發送時間",
                        pattern: RegularExpressionPatterns.SendTime.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.SendTime.errorMessage,
                        },
                    },
                    UseParam: {
                        type: "boolean",
                        title: "UseParam",
                    },
                    Param1: {
                        type: "string",
                        title: "參數一",
                    },
                    Param2: {
                        type: "string",
                        title: "參數二",
                    },
                    Param3: {
                        type: "string",
                        title: "參數三",
                    },
                    Param4: {
                        type: "string",
                        title: "參數四",
                    },
                    Param5: {
                        type: "string",
                        title: "參數五",
                    },
                    IsValid: {
                        type: "boolean",
                        title: "是否為有效名單",
                    },
                    InvalidReason: {
                        type: "string",
                        title: "此筆資料無效原因",
                    },
                    CreatedTime: {
                        type: "string",
                        title: "建立時間",
                    },
                    UploadedSessionId: {
                        type: "integer",
                        title: "上傳名單識別碼",
                    },
                }
            };

            var formUseParam = [{
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Name",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Mobile",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                        required: true,
                        validationMessage: {
                            'isPossibleNumber': RegularExpressionPatterns.PhoneNumber.errorMessage,
                        },
                        $validators: {
                            isPossibleNumber: function (value) {
                                // 如果有輸入，就驗證，如果沒有輸入，就忽略
                                if (value)
                                    return MobileUtil.isPossibleNumber(value);
                                else
                                    return true;
                            },
                        }
                    },
                    ]
                }]
            },
            {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Email",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }, {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "SendTimeString",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            },
            {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Param1",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }, {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Param2",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            },
            {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Param3",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }, {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Param4",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            },
            {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Param5",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            }];

            var formNoUseParam = [{
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Name",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Mobile",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            },
            {
                type: "section",
                htmlClass: "row",
                items: [{
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "Email",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }, {
                    type: "section",
                    htmlClass: "col-xs-6",
                    items: [{
                        key: "SendTimeString",
                        type: "text",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                }]
            }];

            var form = useParam ? formUseParam : formNoUseParam;

            return {
                schema: schema,
                form: form,
            };

        }); // $cacheFactory('SchemaFormFactory').put('UploadedMessageReceiverModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);