(function (window, document) {
    'use strict';

    angular.module('app').run(['SchemaFormCache', 'RegularExpressionPatterns', 'MobileUtil', function (SchemaFormCache, RegularExpressionPatterns, MobileUtil) {

        SchemaFormCache.put('DepartmentManagerModel', function (options) {

            options = options || {};

            var isNew = angular.isDefined(options.isNew) ? options.isNew : true;

            var editRow = {
                schema: {
                    type: "object",
                    properties: {
                        UserName: {
                            type: "string",
                            title: "帳號",
                            required: true,
                            readonly: options["UserName.readonly"] || false,
                            validationMessage: {
                                'usernameAlreadyTaken': '無法使用此帳號，帳號 {{viewValue}} 已經存在',
                            },
                        },
                        DepartmentId: {
                            type: "integer",
                            title: "部門",
                            required: true,
                            enum: options["DepartmentId.enum"] || [],
                        },
                        RoleId: {
                            type: "string",
                            title: "角色",
                            required: true,
                            enum: options["RoleId.enum"] || [],
                        },
                        FullName: {
                            type: "string",
                            title: "姓名",
                            required: true,
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
                        EmployeeNo: {
                            type: "string",
                            title: "員工編號",
                        },
                        PhoneNumber: {
                            type: "string",
                            title: "行動電話",
                        },
                        Email: {
                            type: "string",
                            title: "電子郵件",
                            pattern: RegularExpressionPatterns.Email.pattern,
                            validationMessage: {
                                202: RegularExpressionPatterns.Email.errorMessage,
                            },
                        },
                    }
                },

                form: [
                    {
                        type: "section",
                        htmlClass: "row",
                        items: [
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                    {
                                        key: "UserName",
                                        type: "text",
                                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                        htmlClass: 'has-feedback',
                                        //$asyncValidators: options["UserName.$asyncValidators"], // 當此欄位是唯讀，不需要驗證。
                                    },
                                ]
                            },
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                    {
                                        key: "FullName",
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
                        items: [
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                    {
                                        key: "DepartmentId",
                                        type: "select",
                                        titleMap: options["DepartmentId.titleMap"] || [],
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
                                    key: "RoleId",
                                    type: "select",
                                    titleMap: options["RoleId.titleMap"] || [],
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
                        items: [
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                    {
                                        key: "NewPassword",
                                        type: "password",
                                        placeholder: "若不變更，請留空",
                                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                        htmlClass: 'has-feedback',
                                        $validators: options["NewPassword.$validators"],
                                    },
                                ]
                            },
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                    {
                                        key: "NewPasswordConfirmed",
                                        type: "password",
                                        placeholder: "若不變更，請留空",
                                        feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                        htmlClass: 'has-feedback',
                                        $validators: options["NewPasswordConfirmed.$validators"],
                                    },
                                ]
                            },
                        ]
                    },
                    {
                        type: "section",
                        htmlClass: "row",
                        items: [
                           {
                               type: "section",
                               htmlClass: "col-xs-6",
                               items: [
                               {
                                   key: "EmployeeNo",
                                   type: "text",
                                   feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                                   htmlClass: 'has-feedback',
                               },
                               ]
                           },
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                {
                                    key: "PhoneNumber",
                                    type: "text",
                                    feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
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
                        ]
                    },
                    {
                        type: "section",
                        htmlClass: "row",
                        items: [
                            {
                                type: "section",
                                htmlClass: "col-xs-6",
                                items: [
                                {
                                    key: "Email",
                                    type: "text",
                                    feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                                    htmlClass: 'has-feedback',
                                },
                                ]
                            },
                        ]
                    },
                ]
        };

        var createRow = {
            schema: {
                type: "object",
                properties: {
                    UserName: {
                        type: "string",
                        title: "帳號",
                        required: true,
                        readonly: options["UserName.readonly"] || false,
                        validationMessage: {
                            'usernameAlreadyTaken': '無法使用此帳號，帳號 {{viewValue}} 已經存在',
                        },
                    },
                    DepartmentId: {
                        type: "integer",
                        title: "部門",
                        required: true,
                        enum: options["DepartmentId.enum"] || [],
                    },
                    RoleId: {
                        type: "string",
                        title: "角色",
                        required: true,
                        enum: options["RoleId.enum"] || [],
                    },
                    FullName: {
                        type: "string",
                        title: "姓名",
                        required: true,
                    },
                    NewPassword: {
                        type: "string",
                        title: "密碼",
                        required: true,
                    },
                    NewPasswordConfirmed: {
                        type: "string",
                        title: "再次確認密碼",
                        required: true,
                        validationMessage: {
                            'passwordNotMatch': '與密碼不一致，請重新輸入',
                        },
                    },
                    EmployeeNo: {
                        type: "string",
                        title: "員工編號",
                    },
                    PhoneNumber: {
                        type: "string",
                        title: "行動電話",
                    },
                    Email: {
                        type: "string",
                        title: "電子郵件",
                        pattern: RegularExpressionPatterns.Email.pattern,
                        validationMessage: {
                            202: RegularExpressionPatterns.Email.errorMessage,
                        },
                    },
                }
            },

            form: [
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                                {
                                    key: "UserName",
                                    type: "text",
                                    feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                    htmlClass: 'has-feedback',
                                    $asyncValidators: options["UserName.$asyncValidators"],
                                },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                                {
                                    key: "FullName",
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
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                                {
                                    key: "DepartmentId",
                                    type: "select",
                                    titleMap: options["DepartmentId.titleMap"] || [],
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
                                key: "RoleId",
                                type: "select",
                                titleMap: options["RoleId.titleMap"] || [],
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
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                                {
                                    key: "NewPassword",
                                    type: "password",
                                    feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                    htmlClass: 'has-feedback',
                                    $validators: options["NewPassword.$validators"],
                                },
                            ]
                        },
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                                {
                                    key: "NewPasswordConfirmed",
                                    type: "password",
                                    feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess(), 'glyphicon-star': !hasSuccess() }",
                                    htmlClass: 'has-feedback',
                                    $validators: options["NewPasswordConfirmed.$validators"],
                                },
                            ]
                        },
                    ]
                },
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                       {
                           type: "section",
                           htmlClass: "col-xs-6",
                           items: [
                           {
                               key: "EmployeeNo",
                               type: "text",
                               feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                               htmlClass: 'has-feedback',
                           },
                           ]
                       },
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                            {
                                key: "PhoneNumber",
                                type: "text",
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
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
                    ]
                },
                {
                    type: "section",
                    htmlClass: "row",
                    items: [
                        {
                            type: "section",
                            htmlClass: "col-xs-6",
                            items: [
                            {
                                key: "Email",
                                type: "text",
                                feedback: "{'glyphicon': true, 'glyphicon-ok': hasSuccess() }",
                                htmlClass: 'has-feedback',
                            },
                            ]
                        },
                    ]
                },
            ],
        };

        var data = isNew ? createRow : editRow;

        return { schema: data.schema, form: data.form, };

    }); // $cacheFactory('SchemaFormFactory').put('DepartmentManagerModel', function (options) {

}]); //  angular.module('app').run(['$cacheFactory', function ($cacheFactory) {

})(window, document);