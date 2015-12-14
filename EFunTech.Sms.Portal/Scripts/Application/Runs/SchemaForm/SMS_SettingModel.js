(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', 'MobileUtil', function (SchemaFormCache, RegularExpressionPatterns, MobileUtil) {

        SchemaFormCache.put('SMS_SettingModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    UserName: {
                        type: "string",
                        title: "帳號",
                        readonly: true,
                        required: true,
                    },
                    FullName: {
                        type: "string",
                        title: "姓名",
                        required: true,
                    },
                    Gender: {
                        type: "string",
                        title: "性別",
                        enum: ['0', '1', '2'],
                    },
                    NewPassword: {
                        type: "string",
                        title: "重設密碼",
                    },
                    NewPasswordConfirmed: {
                        type: "string",
                        title: "再次確認密碼",
                        validationMessage: {
                            'passwordNotMatch': '與重設密碼不一致，請重新輸入',
                        },
                    },
                    PhoneNumber: {
                        type: "string",
                        title: "行動電話",
                    },
                    ContactPhone: {
                        type: "string",
                        title: "聯絡電話",
                        pattern: RegularExpressionPatterns.LocalCall.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.LocalCall.errorMessage,
                        },
                    },
                    ContactPhoneExt: {
                        type: "string",
                        title: "聯絡電話(分機)",
                        pattern: RegularExpressionPatterns.LocalCallExt.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.LocalCallExt.errorMessage,
                        },
                    },
                    Email: {
                        type: "string",
                        title: "電子郵件",
                        pattern: RegularExpressionPatterns.Email.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.Email.errorMessage,
                        },
                    },
                    AddressCountry: {
                        type: "string",
                        title: "通訊地址(縣市)",
                    },
                    AddressArea: {
                        type: "string",
                        title: "通訊地址(鄉鎮市區)",
                    },
                    AddressZip: {
                        type: "string",
                        title: "通訊地址(郵遞區號)",
                    },
                    AddressStreet: {
                        type: "string",
                        title: "通訊地址",
                    },
                }
            };

            var form = [
                {
                    type: "section",
                    htmlClass: "row",
                    items: [{
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [{
                            key: "UserName",
                            type: "text",
                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                            htmlClass: 'has-feedback',
                        },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [{
                            key: "FullName",
                            type: "text",
                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                            htmlClass: 'has-feedback',
                        },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [
                            {
                                key: "PhoneNumber",
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
                    }, ]
                }, {
                    type: "section",
                    htmlClass: "row",
                    items: [
                    {
                        type: "section",
                        htmlClass: "col-xs-2",
                        items: [
                            {
                                key: "ContactPhone",
                                type: "text",
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), }",
                                htmlClass: 'has-feedback',
                            },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-2",
                        items: [
                            {
                                key: "ContactPhoneExt",
                                type: "text",
                                placeholder: '分機',
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), }",
                                htmlClass: 'has-feedback',
                                // 想辦法隱藏此欄位，但仍占用空間
                                labelHtmlClass: 'invisible',
                            },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [
                          {
                              key: "Email",
                              type: "text",
                              // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                              feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                              htmlClass: 'has-feedback',
                          },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [
                          {
                              key: "Gender",
                              type: "radios-inline",
                              titleMap: [
                                  { value: '0', name: '不詳' },
                                  { value: '1', name: '男性' },
                                  { value: '2', name: '女性' },
                              ],
                              // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                              feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                              htmlClass: 'has-feedback',
                          },
                        ]
                    }, ]
                }, {
                    type: "section",
                    htmlClass: "row",
                    items: [{
                        type: "section",
                        htmlClass: "col-xs-2",
                        items: [{
                            key: "AddressCountry",
                            title: '通訊地址',
                            placeholder: '縣市',
                            type: "text",
                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                            htmlClass: 'has-feedback',
                        },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-2",
                        items: [{
                            key: "AddressArea",
                            placeholder: '鄉鎮市區',
                            type: "text",
                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                            htmlClass: 'has-feedback',
                            // 想辦法隱藏此欄位，但仍占用空間
                            labelHtmlClass: 'invisible',
                        },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-2",
                        items: [{
                            key: "AddressZip",
                            placeholder: '郵遞區號',
                            type: "text",
                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                            htmlClass: 'has-feedback',
                            // 想辦法隱藏此欄位，但仍占用空間
                            labelHtmlClass: 'invisible',
                        },
                        ]
                    }, {
                        type: "section",
                        htmlClass: "col-xs-6",
                        items: [{
                            key: "AddressStreet",
                            placeholder: '地址',
                            type: "text",
                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                            htmlClass: 'has-feedback',
                            // 想辦法隱藏此欄位，但仍占用空間
                            labelHtmlClass: 'invisible',
                        },
                        ]
                    }]
                }, {
                    type: "section",
                    htmlClass: "row",
                    items: [{
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [
                            {
                                key: "NewPassword",
                                type: "password",
                                placeholder: "若不變更，請留空",
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                                htmlClass: 'has-feedback',
                                $validators: options["NewPassword.$validators"],
                            },
                        ]
                    },
                    {
                        type: "section",
                        htmlClass: "col-xs-4",
                        items: [
                            {
                                key: "NewPasswordConfirmed",
                                type: "password",
                                placeholder: "若不變更，請留空",
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                                htmlClass: 'has-feedback',
                                $validators: options["NewPasswordConfirmed.$validators"],
                            },
                        ]
                    }, ]
                },
            ];

            return {
                schema: schema,
                form: form,
            };

        }); // $cacheFactory('SchemaFormFactory').put('DepartmentPointManagerModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);