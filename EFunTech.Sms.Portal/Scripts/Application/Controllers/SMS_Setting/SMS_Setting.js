(function (window, document) {
    'use strict';

    angular.module('app').controller('SMS_Setting', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'WebApi', 'SchemaFormFactory', 'GlobalSettings', 'SchemaFormHelper', '$translate', 'dialogs',
    function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, WebApi, SchemaFormFactory, GlobalSettings, SchemaFormHelper, $translate, dialogs) {

        //========================================
        // Settings
        //========================================

        var schemaForm = SchemaFormFactory.create('SMS_SettingModel', {
            "NewPassword.$validators": {
                passwordNotMatch: function (value) {
                    var NewPassword = value;
                    var NewPasswordConfirmed = SchemaFormHelper.getViewValue('NewPasswordConfirmed');
                    if (NewPassword == null) NewPassword = '';
                    if (NewPasswordConfirmed == null) NewPasswordConfirmed = '';
                    if (NewPassword === NewPasswordConfirmed) {
                        SchemaFormHelper.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', true);
                        SchemaFormHelper.$broadcast('schemaForm.error.NewPasswordConfirmed', '302', true); // 首先輸入[再次輸入密碼]，之後才輸入[密碼]，都會出現此為必填欄位，目前不知道怎麼解決。
                    }
                    else {
                        SchemaFormHelper.$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', false); // false -> 表示 NewPasswordConfirmed 的 passwordNotMatch 驗證不通過
                    }

                    return true; // 永遠都回傳true, 目的是要在先輸入確認密碼，後輸入密碼，改變確認密碼錯誤訊息。
                },
            },
            "NewPasswordConfirmed.$validators": {
                passwordNotMatch: function (value) {
                    var NewPassword = SchemaFormHelper.getViewValue('NewPassword');
                    var NewPasswordConfirmed = value;
                    if (NewPassword == null) NewPassword = '';
                    if (NewPasswordConfirmed == null) NewPasswordConfirmed = '';
                    return NewPassword === NewPasswordConfirmed;
                },
            },
            //"DepartmentId.enum": _.pluck(availableDepartments, 'value'),
            //"RoleId.enum": _.pluck(availableRoles, 'value'),
            //"DepartmentId.titleMap": availableDepartments,
            //"RoleId.titleMap": availableRoles,
        });

        $scope.schema = schemaForm.schema;
        $scope.form = schemaForm.form;
        $scope.model = {};
        $scope.options = {
            validationMessage: {
                202: '不符合欄位格式',
                302: '此為必填欄位',
            }
        };

        angular.extend($scope, {
            CurrentUser: {
                CreditWarning: {
                    Enabled: false,
                    BySmsMessage: false,
                    ByEmail: false,
                    SmsBalance: 0,
                },
                ReplyCc: {
                    Enabled: false,
                    BySmsMessage: false,
                    ByEmail: false,
                },
                ForeignSmsEnabled: false,
                EditProfile: false,
                UserName: '',
                FullName: '',
                Gender: 1,
                NewPassword: '',
                NewPasswordConfirmed: '',
                PhoneNumber: '',
                ContactPhone: '',
                Email: '',
                AddressArea: '',
            }
        });

        //========================================
        // Functions
        //========================================

        $scope.search = function () {
            var webApi = new WebApi('api/SMS_Setting');
            webApi.Get()
            .then(function (result) {
                $scope.CurrentUser = result.data;
            });
        };

        $scope.submit = function (form) {
            
            // 使用內建驗證功能
            $scope.$broadcast('schemaFormValidate');

            // 使用自訂驗證功能
            //if (!$scope.validateBeforeSubmit($scope, form))
            //    return;

            // 所有驗證都通過，才回傳model，以及後續存檔動作。
            if (form.$valid) {
                var webApi = new WebApi('api/SMS_Setting');
                webApi.Put($scope.CurrentUser)
                .then(function (result) {
                    $scope.search();
                });
            }
            else {
                // 20151028 Norman, 當檢核不通過，需要顯示[個人資料維護]，不應該收合這個panel
                $scope.CurrentUser.EditProfile = true;
            }
        };

        //========================================
        // Initialize
        //========================================
        $scope.search(); 

        // $scope.CurrentUser.EditProfile = !form.$valid; // TODO: how to get form object
    }]);

})(window, document);
