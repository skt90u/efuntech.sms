(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', 'PhoneAreaCode', 'MobileUtil', function (SchemaFormCache, RegularExpressionPatterns, PhoneAreaCode, MobileUtil) {

        SchemaFormCache.put('ContactModel', function (options) {

            var phoneAreaCodeEnum = _.pluck(PhoneAreaCode, 'value');
            phoneAreaCodeEnum.unshift('');

            var phoneAreaCodeOptionTitleMap = _.map(PhoneAreaCode, function (areaCode) { return { value: areaCode.value, name: areaCode.value + ' - ' + areaCode.text }; });
            phoneAreaCodeOptionTitleMap.unshift({ value: '', name: '不指定' });

            var schema = {
                type: "object",
                properties: {
                    Name: {
                        type: "string",
                        title: "姓名",
                        required: true,
                    },
                    Mobile: {
                        type: "string",
                        title: "行動電話",
                    },
                    HomePhoneArea: {
                        type: "string",
                        title: "住家電話區碼",
                        enum: phoneAreaCodeEnum,
                    },
                    HomePhone: {
                        type: "string",
                        title: "住家電話",
                        pattern: RegularExpressionPatterns.LocalCall.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.LocalCall.errorMessage,
                        },
                    },
                    CompanyPhoneArea: {
                        type: "string",
                        title: "公司電話區碼",
                        enum: phoneAreaCodeEnum,
                    },
                    CompanyPhone: {
                        type: "string",
                        title: "公司電話",
                        pattern: RegularExpressionPatterns.LocalCall.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.LocalCall.errorMessage,
                        },
                    },
                    CompanyPhoneExt: {
                        type: "string",
                        title: "分機",
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
                    Msn: {
                        type: "string",
                        title: "MSN",
                        pattern: RegularExpressionPatterns.Email.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.Email.errorMessage,
                        },
                    },
                    Description: {
                        type: "string",
                        title: "聯絡人概述",
                    },
                    Birthday: {
                        type: "string",
                        title: "生日",
                        pattern: RegularExpressionPatterns.Date_MMdd.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.Date_MMdd.errorMessage,
                        },
                    },
                    ImportantDay: {
                        type: "string",
                        title: "重要日期",
                        pattern: RegularExpressionPatterns.Date_MMdd.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.Date_MMdd.errorMessage,
                        },
                    },
                    Gender: {
                        type: "string",
                        title: "性別",
                        enum: ['0', '1', '2'],
                    },
                }
            }

            var form = [
                // row 1
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                            {
                                key: "Name",
                                type: "text",
                                // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                htmlClass: 'has-feedback',
                            },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                              {
                                  key: "Mobile",
                                  type: "text",
                                  // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
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
                        },
                        {
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
                        },
                    ]
                },
                // row 2
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                            {
                                key: "HomePhoneArea",
                                type: "select",
                                title: '住家電話',
                                // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                htmlClass: 'has-feedback',
                                titleMap: phoneAreaCodeOptionTitleMap,
                            },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                              {
                                  key: "HomePhone",
                                  type: "text",
                                  // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                  feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                  htmlClass: 'has-feedback',
                                  // 想辦法隱藏此欄位，但仍占用空間
                                  labelHtmlClass: 'invisible',
                              },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                              {
                                  key: "Msn",
                                  type: "text",
                                  // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                  feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                  htmlClass: 'has-feedback',
                              },
                            ]
                        },

                    ]
                },
                // row 3
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                            {
                                key: "CompanyPhoneArea",
                                type: "select",
                                title: '公司電話',
                                // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                htmlClass: 'has-feedback',
                                titleMap: phoneAreaCodeOptionTitleMap,
                            },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                              {
                                  key: "CompanyPhone",
                                  type: "text",
                                  // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                  feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                  htmlClass: 'has-feedback',
                                  // 想辦法隱藏此欄位，但仍占用空間
                                  labelHtmlClass: 'invisible',
                              },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                              {
                                  key: "CompanyPhoneExt",
                                  type: "text",
                                  // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                  feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                  htmlClass: 'has-feedback',
                              },
                            ]
                        },
                    ]
                },
                // row 4
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                            {
                                key: "Birthday",
                                type: "text",
                                // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                htmlClass: 'has-feedback',
                            },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-4",
                            items: [
                              {
                                  key: "ImportantDay",
                                  type: "text",
                                  // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                  feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                  htmlClass: 'has-feedback',
                              },
                            ]
                        },
                        {
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
                        },
                    ]
                },
                // row 5
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-12",
                            items: [
                            {
                                key: "Description",
                                type: "textarea",
                                placeholder: "",
                                // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                htmlClass: 'has-feedback',
                                labelHtmlClass: 'control-label',
                            },
                            ]
                        },
                    ]
                },
            ];

            return { schema: schema, form: form, };

        }); // $cacheFactory('SchemaFormFactory').put('ContactModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);