(function (window, document) {
    'use strict';

    //========================================
    // FormModalCtrl
    //  共用單筆資料編輯 Controller
    //
    // @param schema: sf-schema 物件
    // @param form: sf-form 物件
    // @param model: sf-model 物件
    // @param options: sf-options 物件
    // @param validateBeforeSubmit: 存檔前，執行驗證輸入數值驗證的 function
    //========================================
    angular.module('app').
    controller('FormModalCtrl', ['$scope', '$modalInstance', 'options',  
    function ($scope, $modalInstance, options) {

        //var isNew = !angular.isDefined(options) ||
        //            !angular.isDefined(options.model) ||
        //            !angular.isDefined(options.model.Id);

        options = angular.extend({}, {
            title: function (model) {
                return !angular.isDefined(model.Id) ? "新增資料" : "編輯資料";
            },
            schema: function (model) {
                return void 0;
            },
            form: function (model) {
                return void 0;
            },
            model: {},
            options: function (model) {
                return {
                    validationMessage: {
                        202: '不符合欄位格式',
                        302: '此為必填欄位',
                        /*
                        // 302 是 tv4 定義的其中一種 ErrorCode
                        302: function (context) { // OBJECT_REQUIRED
                            // The context variables available to you are:
                            // Name	     Value
                            // error	 The error code
                            // title	 Title of the field
                            // value	 The model value
                            // viewValue The view value (probably the one you want)
                            // form	     form definition object for this field
                            // schema	 schema for this field
                            return context.title + '為必填欄位';
                        },
                        */
                    },
                };
            },
            //validateBeforeSubmit: function (model) {
            //    return function (modalScope, form) {
            //        //modalScope.$broadcast('schemaForm.error.欄位名稱', 'YourErrorCode', 'ErrorMessage for Email');
            //        //modalScope.$broadcast('schemaForm.error.Email', 'YourErrorCode', 'ErrorMessage for Email');
            //        //modalScope.$broadcast('schemaForm.error.Birthday', 'YourErrorCode', 'ErrorMessage for Birthday');

            //        // 使用 tv4 內建錯誤訊息
            //        //modalScope.$broadcast('schemaForm.error.Birthday', 'tv4-302', false);

            //        // true | false : 是否移除錯誤訊息 (當自訂驗證通過時，要設為 true)
            //        // modalScope.$broadcast('schemaForm.error.Birthday', 'tv4-302', true | false);
            //        return true; // 驗證通過
            //    };
            //},
            validateBeforeSubmit: function (modalScope, form) {
                //modalScope.$broadcast('schemaForm.error.欄位名稱', 'YourErrorCode', 'ErrorMessage for Email');
                //modalScope.$broadcast('schemaForm.error.Email', 'YourErrorCode', 'ErrorMessage for Email');
                //modalScope.$broadcast('schemaForm.error.Birthday', 'YourErrorCode', 'ErrorMessage for Birthday');

                // 使用 tv4 內建錯誤訊息
                //modalScope.$broadcast('schemaForm.error.Birthday', 'tv4-302', false);

                // true | false : 是否移除錯誤訊息 (當自訂驗證通過時，要設為 true)
                // modalScope.$broadcast('schemaForm.error.Birthday', 'tv4-302', true | false);
                return true; // 驗證通過
            },
        }, options);

        function result(attribute, model) {
            var value = attribute;
            if (angular.isUndefined(value)) return void 0;
            return _.isFunction(value) ? value.call(null, model) : value;
        }

        $scope.title = result(options.title, options.model);
        $scope.schema = result(options.schema, options.model);
        $scope.form = result(options.form, options.model);
        $scope.model = result(options.model, options.model);
        $scope.options = result(options.options, options.model);
        $scope.validateBeforeSubmit = options.validateBeforeSubmit;

        //========================================
        // ### angular schema Form
        //========================================
        // 1. [Extending Schema Form](https://github.com/Textalk/angular-schema-form/blob/master/docs/extending.md)
        // 2. [SourceCode](https://github.com/Textalk/angular-schema-form)
        // 2. [Documentation](https://github.com/Textalk/angular-schema-form/blob/development/docs/index.md)
        // 3. [Examples](http://schemaform.io/examples/bootstrap-example.html)

        //========================================
        // 1. 欄位型別
        //========================================
        // 預設的 schema type 與 form type 對應
        // Schema type                          Form type
        // string 	                            text
        // number 	                            number
        // integer 	                            number
        // boolean	                            checkbox
        // object	                            fieldset
        // string and a enum	                select
        // array and a enum in array type	    checkboxes
        // array	                            array
        //========================================
        // 2. 欄位說明
        //========================================
        // title
        // description
        // readonly
        //========================================
        // 3. 欄位範圍 (select, checkboxes, radios)
        //========================================
        // enum
        //========================================
        // 4. 欄位驗證預設規則
        //========================================
        // required
        //--------------------
        // 字串
        //--------------------
        // minLength
        // maxLength
        // pattern
        //--------------------
        // 數值
        //--------------------
        // divisibleBy
        // minimum
        // maximum
        // exclusiveMinimum: true | false
        // exclusiveMaximum: true | false
        //--------------------
        // 陣列(不知道怎麼用)
        //--------------------
        // minItems
        // maxItems
        //--------------------
        // 不知道怎麼用
        //--------------------
        // minProperties
        // maxProperties
        //========================================
        // 5. 自訂驗證 (必須放在 form 定義之中才有效)
        //========================================
        // 同步驗證
        //--------------------
        //   {
        //     key: 'name',
        //     validationMessage: {
        //   'noBob': 'Bob is not OK! You here me?'
        //     },
        //     $validators: {
        //   noBob: function(value) {
        //     if (angular.isString(value) && value.indexOf('Bob') !== -1) {
        //       return false;
        //     }
        //     return true
        //   }
        //     }
        //   }
        //--------------------
        // 同步驗證
        //--------------------
        //   {
        //     key: 'name',
        //     validationMessage: {
        //   'noBob': 'Bob is not OK! You here me?'
        //     },
        //     $asyncValidators: {
        //   noBob: function(value) {
        //     var deferred = $q.defer();
        //     $timeout(function(){
        //       if (angular.isString(value) && value.indexOf('bob') !== -1) {
        //         deferred.reject();
        //       } else {
        //         deferred.resolve();
        //       }
        //     }, 500);
        //     return deferred.promise;
        //   }
        //     }
        //========================================
        // 6. 欄位驗證錯誤訊息
        //========================================
        // 6.1 固定字串
        //--------------------
        // validationMessage: { 302: 'Do not forget me!' }
        //--------------------
        // 6.2 Message Interpolation 
        //--------------------
        //
        // The context variables available to you are:
        //
        // Name	     Value
        // error	 The error code
        // title	 Title of the field
        // value	 The model value
        // viewValue The view value (probably the one you want)
        // form	     form definition object for this field
        // schema	 schema for this field
        //
        // validationMessage: {
        //  101: 'Seriously? Value {{value}} totally less than {{schema.minimum}}, which is NOT OK.',
        // }
        //--------------------
        // 6.3 functions as validationMessages
        //--------------------
        // validationMessage: {
        //  302: function(ctx) { return Jed.gettext('This value is required.'); },
        // }
        //========================================
        // scope.$broadcast('schemaForm.error.name','usernameAlreadyTaken','The username is already taken');
        // scope.$broadcast('schemaForm.error.name', 'usernameAlreadyTaken', true);
        // scope.$broadcast('schemaForm.error.name', 'usernameAlreadyTaken', false);
        // scope.$broadcast('schemaForm.error.name','tv4-302',false);

        //========================================
        // 1. 欄位型別
        //========================================
        // Form Type     Becomes
        //--------------------
        // fieldset      a fieldset with legend
        // section       just a div
        // actions       horizontal button list, can only submit and buttons as items
        // text          input with type text
        // textarea      a textarea
        // number        input type number
        // password      input type password
        // checkbox      a checkbox
        // checkboxes    list of checkboxes
        // select        a select (single value)
        // submit        a submit button
        // button        a button
        // radios        radio buttons
        // radios-inline radio buttons in one line
        // radiobuttons  radio buttons with bootstrap buttons
        // help          insert arbitrary html
        // template      insert an angular template
        // tab           tabs with content
        // array         a list you can add, remove and reorder
        // tabarray      a tabbed version of array
        //========================================
        // 2. 所有屬性(排除與 schema 重複的部分)
        //========================================
        // {
        //   key: "address.street",      // The dot notatin to the attribute on the model
        //   type: "text",               // Type of field
        //   notitle: false,             // Set to true to hide title
        //   onChange: "valueChanged(form.key,modelValue)", // onChange event handler, expression or function
        //   feedback: false,             // Inline feedback icons
        //   disableSuccessState: false,  // Set true to NOT apply 'has-success' class to a field that was validated successfully
        //   disableErrorState: false,    // Set true to NOT apply 'has-error' class to a field that failed validation
        //   placeholder: "Input...",     // placeholder on inputs and textarea
        //   ngModelOptions: { ... },     // Passed along to ng-model-options
        //   readonly: true,              // Same effect as readOnly in schema. Put on a fieldset or array
        //                                // and their items will inherit it.
        //   fieldAddonLeft: 'your new title'
        //   htmlClass: "street foobar",  // CSS Class(es) to be added to the container div
        //   fieldHtmlClass: "street"     // CSS Class(es) to be added to field input (or similar)
        //   labelHtmlClass: "street"     // CSS Class(es) to be added to the label of the field (or similar)
        //   copyValueTo: ["address.street"],     // Copy values to these schema keys.
        //   condition: "person.age < 18" // Show or hide field depending on an angular expression
        //   destroyStrategy: "remove"    // One of "null", "empty" , "remove", or 'retain'. Changes model on $destroy event. default is "remove".
        //   $validators: {
        //      noBob: function(value) {
        //        if (angular.isString(value) && value.indexOf('Bob') !== -1) {
        //          return false;
        //        }
        //        return true
        //      }
        //   },
        //   $asyncValidators: {
        //     noBob: function(value) {
        //   	var deferred = $q.defer();
        //   	$timeout(function(){
        //   	  if (angular.isString(value) && value.indexOf('bob') !== -1) {
        //   		deferred.reject(); // deferred.reject  : 驗證失敗
        //   	  } else {
        //   		deferred.resolve(); // deferred.resolve : 驗證通過
        //   	  }
        //   	}, 500);
        //   	return deferred.promise;
        //     }
        //   }
        // }

        // https://github.com/Textalk/angular-schema-form/blob/development/docs/index.md
        // 若要查詢欄位類型，請搜尋 Form types

        // reference:
        //  https://github.com/Textalk/angular-schema-form/blob/development/docs/index.md
        $scope.submit = function (form) {

            // 使用內建驗證功能
            $scope.$broadcast('schemaFormValidate');

            // 使用自訂驗證功能
            if (!$scope.validateBeforeSubmit($scope, form))
                return;

            // 所有驗證都通過，才回傳model，以及後續存檔動作。
            if (form.$valid) {
                $modalInstance.close($scope.model);
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

    }]);

})(window, document);