(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', 'MobileUtil', function (SchemaFormCache, RegularExpressionPatterns, MobileUtil) {

        SchemaFormCache.put('BlackListModel', function (options) {
            var schema = {
                type: "object",
                properties: {
                    Name: {
                        type: "string",
                        title: "姓名",
                    },
                    Mobile: {
                        type: "string",
                        title: "行動電話",
                    },
                    Enabled: {
                        type: "boolean",
                        title: "開啟 /關閉",
                    },
                    Remark: {
                        type: "string",
                        title: "備註",
                    },
                }
            };

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
                                  key: "Enabled",
                                  type: "radios-inline",
                                  titleMap: [
                                      { value: false, name: '關閉' },
                                      { value: true, name: '開啟' },
                                  ],
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
                            htmlClass: "col-xs-12",
                            items: [
                            {
                                key: "Remark",
                                type: "textarea",
                                placeholder: "",
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

        }); // $cacheFactory('SchemaFormFactory').put('BlackListModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);

