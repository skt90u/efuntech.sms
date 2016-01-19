(function (window, document) {
    'use strict';

    angular.module('app').controller('SendMessage', ['$scope', '$http', 'uiGridConstants', '$modal', '$log', 'CrudApi', 'SchemaFormFactory', 'GlobalSettings', 'WebApi', '$translate', 'dialogs', 'FileManagerApi', 'toaster', '$element', 'RegularExpressionPatterns', 'MessageCostInfo',
        'SelectUtil', 'RecipientFromType', 'SendCustType', 'SendMessageType',
        'CommonMessageManager', 'SignatureManager',
        'FileUploadManager', 'CommonContactManager', 'ContactOfGroupManager', 'ManualInputManager',
        function ($scope, $http, uiGridConstants, $modal, $log, CrudApi, SchemaFormFactory, GlobalSettings, WebApi, $translate, dialogs, FileManagerApi, toaster, $element, RegularExpressionPatterns, MessageCostInfo,
            SelectUtil, RecipientFromType, SendCustType, SendMessageType,
            CommonMessageManager, SignatureManager,
            FileUploadManager, CommonContactManager, ContactOfGroupManager, ManualInputManager) {

            //========================================
            // Settings
            //========================================

            var RecipientFromTypeOptions = SelectUtil.getEnumOptions(RecipientFromType);
            var SendCustTypeOptions = SelectUtil.getEnumOptions(SendCustType);
            var SendMessageTypeOptions = SelectUtil.getEnumOptions(SendMessageType);

            $scope.RecipientFromTypeOptions = RecipientFromTypeOptions; // 讓 html 檔案也可以存取到這個 enum

            //========================================
            // Settings - SendMessageRule
            //========================================
            
            var SendMessageRule = {

                SendTitle: '',
                SendBody: '',

                RecipientFromType: '',
                RecipientFromFileUpload: null,
                RecipientFromCommonContact: null,
                RecipientFromGroupContact: null,
                RecipientFromManualInput: null,

                SendTimeType: null,
                SendDeliver: null,
                SendCycleEveryDay: null,
                SendCycleEveryWeek: null,
                SendCycleEveryMonth: null,
                SendCycleEveryYear: null,

                SendCustType: SendCustTypeOptions.OneWay, // 單向|雙向 簡訊發送
                UseParam: false, // 是否為參數式
                SendMessageType: SendMessageTypeOptions.SmsMessage, // 發送訊息類型(手機簡訊|APP簡訊)

                // 畫面呈現
                MessageLength: 0, // 簡訊字數
                MessageNum: 0, // 簡訊總共幾則
                MessageCost: 0, // 簡訊花費點數
                MessageFormatError: '',
            };

            //========================================
            // Functions
            //========================================

            $scope.checkSendBody = function () {

                var messageCostInfo = new MessageCostInfo($scope.SendMessageRule.SendBody, {
                    "@space1@": "",
                    "@space2@": "",
                    "@space3@": "",
                    "@space4@": "",
                    "@space5@": "",
                });

                $scope.SendMessageRule.MessageLength = messageCostInfo.MessageLength; // 簡訊字數
                $scope.SendMessageRule.MessageNum = messageCostInfo.MessageNum; // 簡訊總共幾則
                $scope.SendMessageRule.MessageCost = messageCostInfo.MessageCost; // 簡訊花費點數
                $scope.SendMessageRule.MessageFormatError = messageCostInfo.MessageFormatError;

                if ($scope.SendMessageRule.MessageFormatError) {
                    toaster.pop({
                        type: 'error',
                        title: '發送內容',
                        body: $scope.SendMessageRule.MessageFormatError,
                        showCloseButton: true,
                    });
                }
            };

            $scope.onSelect = function (RecipientFromType) {
                $scope.SendMessageRule.RecipientFromType = RecipientFromType;

                switch (RecipientFromType) {
                    case RecipientFromTypeOptions.FileUpload:
                        {
                            $scope.FileUploadManager.init();
                        } break;

                    case RecipientFromTypeOptions.CommonContact:
                        {
                            $scope.CommonContactManager.init();
                        } break;

                    case RecipientFromTypeOptions.GroupContact:
                        {
                            $scope.ContactOfGroupManager.init();
                        } break;

                    case RecipientFromTypeOptions.ManualInput:
                        {
                            $scope.ManualInputManager.init();
                        } break;
                }
            };

            $scope.checkAllParameter = function () {

                var errors = [];

                var SendMessageRule = $scope.SendMessageRule;
                var SendBody = SendMessageRule.SendBody;
                var RecipientFromType = SendMessageRule.RecipientFromType;
                var UseParam = SendMessageRule.UseParam;

                if (SendBody.length == 0) {
                    errors.push('發送內容尚未輸入');
                }

                if (UseParam) {
                    if (SendBody.indexOf("@space1@") == -1 &&
                        SendBody.indexOf("@space2@") == -1 &&
                        SendBody.indexOf("@space3@") == -1 &&
                        SendBody.indexOf("@space4@") == -1 &&
                        SendBody.indexOf("@space5@") == -1) {
                        errors.push('請輸入參數內容@space1@、@space2@、@space3@、@space4@或@space5@！');
                    }
                }
                else {
                    if (SendBody.indexOf("@space1@") != -1 ||
                        SendBody.indexOf("@space2@") != -1 ||
                        SendBody.indexOf("@space3@") != -1 ||
                        SendBody.indexOf("@space4@") != -1 ||
                        SendBody.indexOf("@space5@") != -1) {
                        errors.push('發送內容不允許出現@space1@、@space2@、@space3@、@space4@以及@space5@！');
                    }
                }


                switch (RecipientFromType) {
                    case RecipientFromTypeOptions.FileUpload:
                        {
                            var fileName = $scope.FileUploadManager.uploadedMessageReceiverListResult ? $scope.FileUploadManager.uploadedMessageReceiverListResult.FileName : '';
                            var validCount = $scope.FileUploadManager.uploadedMessageReceiverListResult ? $scope.FileUploadManager.uploadedMessageReceiverListResult.ValidCount : 0;

                            if (fileName == "") {
                                errors.push("您尚未上傳名單，請先上傳名單！");
                            } else {
                                if (validCount == 0) {
                                    errors.push("有效名單數量為零，請修改或重新上傳名單！");
                                }
                            }

                            if (errors.length == 0) {
                                // 設定參數到 SendMessageRule
                                SendMessageRule.RecipientFromFileUpload = {
                                    UploadedFileId: $scope.FileUploadManager.uploadedMessageReceiverListResult.UploadedSessionId,
                                    AddSelfToMessageReceiverList: $scope.FileUploadManager.addSelfToMessageReceiverList,
                                };
                                SendMessageRule.RecipientFromCommonContact = null;
                                SendMessageRule.RecipientFromGroupContact = null;
                                SendMessageRule.RecipientFromManualInput = null;
                            }

                        } break;

                    case RecipientFromTypeOptions.CommonContact:
                        {
                            var selectedRows = $scope.CommonContactManager.gridApi.selection.getSelectedRows();

                            if (selectedRows.length == 0) {
                                errors.push("您尚未選取聯絡人，請先選取聯絡人！");
                            }

                            if (errors.length == 0) {
                                // 設定參數到 SendMessageRule
                                SendMessageRule.RecipientFromFileUpload = null;
                                SendMessageRule.RecipientFromCommonContact = {
                                    ContactIds: _.pluck(selectedRows, 'Id').join(','),
                                };
                                SendMessageRule.RecipientFromGroupContact = null;
                                SendMessageRule.RecipientFromManualInput = null;
                            }
                        } break;

                    case RecipientFromTypeOptions.GroupContact:
                        {
                            var selectedRows = $scope.ContactOfGroupManager.SharedContactManager.gridApi.selection.getSelectedRows();

                            if (selectedRows.length == 0) {
                                errors.push("您尚未選取聯絡人，請先選取聯絡人！");
                            }

                            if (errors.length == 0) {
                                // 設定參數到 SendMessageRule
                                SendMessageRule.RecipientFromFileUpload = null;
                                SendMessageRule.RecipientFromCommonContact = null;
                                SendMessageRule.RecipientFromGroupContact = {
                                    ContactIds: _.pluck(selectedRows, 'Id').join(','),
                                };
                                SendMessageRule.RecipientFromManualInput = null;
                            }
                        } break;

                    case RecipientFromTypeOptions.ManualInput:
                        {
                            var manualInput = $scope.ManualInputManager.manualInput;
                            var phoneNumbers = $scope.ManualInputManager.phoneNumbers;

                            if (manualInput == "") {
                                errors.push("您尚未輸入收訊門號，請先輸入收訊門號！");
                            } else {
                                if (phoneNumbers.length == 0) {
                                    errors.push("您所輸入的有效門號筆數為零！");
                                }
                            }

                            if (errors.length == 0) {
                                // 設定參數到 SendMessageRule
                                SendMessageRule.RecipientFromFileUpload = null;
                                SendMessageRule.RecipientFromCommonContact = null;
                                SendMessageRule.RecipientFromGroupContact = null;
                                SendMessageRule.RecipientFromManualInput = {
                                    PhoneNumbers: phoneNumbers.join(','),
                                };
                            }
                        } break;
                }

                if (errors.length == 0) {
                    return true;
                }
                else {
                    var messages = _.map(errors, function (error, index) {
                        return '<li>' + error + '</li>';
                    });
                    dialogs.error('操作錯誤', '<ol>' + messages.join('') + '</ol>');
                    return false;
                }
            };

            $scope.editSendTime = function () {

                if (!$scope.checkAllParameter()) return;

                var modalInstance = $modal.open({
                    templateUrl: 'template/modal/sendTime.html',
                    windowClass: 'center-modal',
                    controller: 'SendTimeModalCtrl',
                    //size: size,
                    resolve: {
                        options: function () {
                            return {
                                // 傳遞 SendMessageRule
                                SendMessageRule: $scope.SendMessageRule,
                            };
                        },
                    }
                });

                modalInstance.result.then(function (savedModel) {
                    // 新增簡訊發送規則成功，重設頁面欄位
                    $scope.reset(savedModel);
                });
            };

            /**
             * 回復畫面剛開始的狀態
             */
            $scope.reset = function (savedModel) {

                var SendMessageRule = $scope.SendMessageRule;

                SendMessageRule.SendTitle = '';
                SendMessageRule.SendBody = '';

                SendMessageRule.RecipientFromType = '';
                SendMessageRule.RecipientFromFileUpload = null;
                SendMessageRule.RecipientFromCommonContact = null;
                SendMessageRule.RecipientFromGroupContact = null;
                SendMessageRule.RecipientFromManualInput = null;

                SendMessageRule.SendTimeType = null;
                SendMessageRule.SendDeliver = null;
                SendMessageRule.SendCycleEveryDay = null;
                SendMessageRule.SendCycleEveryWeek = null;
                SendMessageRule.SendCycleEveryMonth = null;
                SendMessageRule.SendCycleEveryYear = null;

                SendMessageRule.SendCustType = SendCustTypeOptions.OneWay; // 單向|雙向 簡訊發送
                SendMessageRule.UseParam = false; // 是否為參數式
                SendMessageRule.SendMessageType = SendMessageTypeOptions.SmsMessage; // 發送訊息類型(手機簡訊|APP簡訊)

                // 畫面呈現
                SendMessageRule.MessageLength = 0; // 簡訊字數
                SendMessageRule.MessageNum = 0; // 簡訊總共幾則
                SendMessageRule.MessageCost = 0; // 簡訊花費點數
                SendMessageRule.MessageFormatError = '';

                $scope.CommonMessageManager.visible = false;
                $scope.SignatureManager.visible = false;

                SendMessageRule.RecipientFromType = savedModel.RecipientFromType; // 必須保存起來，否則當沒有重新改變收訊聯絡人種類時，在一次新增簡訊規則時，會變成空值
                $scope.onSelect(SendMessageRule.RecipientFromType);
            };

            //========================================
            // Events & EventHandlers
            //========================================


            //========================================
            // Initialize
            //========================================
            $scope.SendMessageRule = SendMessageRule;
            $scope.SendMessageRule.RecipientFromType = RecipientFromTypeOptions.FileUpload; // 收訊聯絡人的第一個類型

            $scope.CommonMessageManager = CommonMessageManager;
            $scope.SignatureManager = SignatureManager;

            $scope.FileUploadManager = new FileUploadManager();
            $scope.FileUploadManager.UseParam = $scope.SendMessageRule.UseParam;

            $scope.CommonContactManager = CommonContactManager;
            $scope.ContactOfGroupManager = ContactOfGroupManager;
            $scope.ManualInputManager = ManualInputManager;
        }]);

})(window, document);
