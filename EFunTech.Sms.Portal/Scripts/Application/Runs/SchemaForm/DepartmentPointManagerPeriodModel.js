(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', function (SchemaFormCache, RegularExpressionPatterns) {

        SchemaFormCache.put('DepartmentPointManagerPeriodModel', function (options) {

            var schema = {
                type: "object",
                properties: {
                    MonthlyAllot: {
                        type: "boolean",
                        title: "自動撥數類型",
                    },
                    MonthlyAllotDay: {
                        type: "string",
                        title: "每月撥點日期",
                        enum: ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31'],
                    },
                    MonthlyAllotPoint: {
                        type: "number",
                        title: "匯入點數",
                    },
                    LimitMinPoint: {
                        type: "number",
                        title: "額度下限",
                    },
                    LimitMaxPoint: {
                        type: "number",
                        title: "自動補至",
                    },
                }
            }

            var form = [
                {
                    key: "MonthlyAllot",
                    type: "radios-inline",
                    title: "",
                    titleMap: [
                        { value: true, name: '每月定期撥點' },
                        { value: false, name: '低於設定點數時，自動撥點' },
                    ],
                    feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                    htmlClass: 'has-feedback',
                },
                {
                    type: "conditional",
                    condition: "model.MonthlyAllot",
                    items: [
                        {
                            type: "section",
                            htmlClass: "row",
                            items: [
                                {
                                    type: "section",
                                    htmlClass: "col-xs-6",
                                    items: [
                                        {
                                            key: "MonthlyAllotDay",
                                            type: "select",
                                            // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                            feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess()}",
                                            htmlClass: 'has-feedback',
                                        },
                                    ]
                                },
                                {
                                    type: "section",
                                    htmlClass: "col-xs-6",
                                    items: [
                                      {
                                          key: "MonthlyAllotPoint",
                                          type: "number",
                                          // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                          feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                          htmlClass: 'has-feedback',
                                      },
                                    ]
                                },
                            ]
                        },
                    ]
                },
                {
                    type: "conditional",
                    condition: "!model.MonthlyAllot",
                    items: [
                        {
                            type: "section",
                            htmlClass: "row",
                            items: [
                                {
                                    type: "section",
                                    htmlClass: "col-xs-6",
                                    items: [
                                      {
                                          key: "LimitMinPoint",
                                          type: "number",
                                          // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                          feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                          htmlClass: 'has-feedback',
                                      },
                                    ]
                                },
                                {
                                    type: "section",
                                    htmlClass: "col-xs-6",
                                    items: [
                                      {
                                          key: "LimitMaxPoint",
                                          type: "number",
                                          // 目前 feedback 沒有作用，還找不到原因，使用增加 htmlClass: 'has-feedback' 可解決問題
                                          feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                          htmlClass: 'has-feedback',
                                      },
                                    ]
                                },
                            ]
                        },
                    ]
                },
            ];
           
            return { schema: schema, form: form, };

        }); // $cacheFactory('SchemaFormFactory').put('DepartmentPointManagerPeriodModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);