(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'GlobalSettings', function (SchemaFormCache, GlobalSettings) {

        SchemaFormCache.put('SystemAnnouncementModel', function (options) {

            var schema = {
                type: "object",
                properties:
                {
                    //IsVisible: {
                    //    type: "boolean",
                    //    title: "顯示 / 隱藏 此公告",
                    //    enum: [true, false]
                    //},
                    PublishDate: {
                        type: "string",
                        title: "公告日期",
                        required: true,
                        format: "date", // (+) angular-schema-form-datepicker
                    },
                    Announcement: {
                        type: "string",
                        title: "公告內容",
                        required: true,
                    },
                    //CreatedTime: {
                    //    type: "string",
                    //    title: "建立時間",
                    //    required: true,
                    //},
                }
            };

            var form = [
            {
                type: "section",
                htmlClass: "row",
                items: [
                {
                    type: "section",
                    htmlClass: "col-xs-12",
                    items: [{
                        key: "PublishDate",
                        //type: "text", // (-) angular-schema-form-datepicker
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                        //minDate: "1995-09-01", // (+) angular-schema-form-datepicker
                        //maxDate: new Date(), // (+) angular-schema-form-datepicker
                        //format: "yyyy-mm-dd", // (+) angular-schema-form-datepicker
                    },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "col-xs-12",
                    items: [{
                        key: "Announcement",
                        type: "textarea",
                        placeholder: "",
                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                        htmlClass: 'has-feedback',
                    },
                    ]
                },
                //{
                //    type: "section",
                //    htmlClass: "col-xs-4",
                //    items: [{
                //        key: "CreatedTime",
                //        type: "text",
                //        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                //        htmlClass: 'has-feedback',
                //    },
                //    ]
                //},
                //{
                //    type: "section",
                //    htmlClass: "col-xs-12",
                //    items: [{
                //        key: "IsVisible",
                //        style: {
                //            "selected": "btn-success",
                //            "unselected": "btn-default"
                //        },
                //        type: "radiobuttons",
                //        notitle: true,
                //        titleMap: [
                //        {
                //            value: true,
                //            name: '顯示此公告'
                //        },
                //        {
                //            value: false,
                //            name: '隱藏此公告'
                //        },
                //        ]
                //    },
                //    ]
                //},
                ]
            },
            ];

            return {
                schema: schema,
                form: form,
            };

        }); // $cacheFactory('SchemaFormFactory').put('SystemAnnouncementModel', function (options) {

    }]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);