(function (window, document) {
    'use strict';

    // https://github.com/m-e-conroy/angular-dialog-service
    // https://codepen.io/m-e-conroy/pen/rkIqv

    angular.module('app')
    .config(['dialogsProvider', '$translateProvider', function (dialogsProvider, $translateProvider) {

        dialogsProvider.useBackdrop('static');
        dialogsProvider.useCopy(false);
        dialogsProvider.useEscClose(false);
        dialogsProvider.setSize('sm');
        dialogsProvider.useClass('center-dialog');

        $translateProvider.translations('zh-TW', {
            DIALOGS_ERROR: "錯誤",
            DIALOGS_ERROR_MSG: "出現未知錯誤。",
            DIALOGS_CLOSE: "關閉",
            DIALOGS_PLEASE_WAIT: "請稍後",
            DIALOGS_PLEASE_WAIT_ELIPS: "請稍後...",
            DIALOGS_PLEASE_WAIT_MSG: "請等待操作完成。",
            DIALOGS_PERCENT_COMPLETE: "% 已完成",
            DIALOGS_NOTIFICATION: "通知",
            DIALOGS_NOTIFICATION_MSG: "未知應用程式的通知。",
            DIALOGS_CONFIRMATION: "確認",
            DIALOGS_CONFIRMATION_MSG: "確認要求。",
            DIALOGS_OK: "確定",
            DIALOGS_YES: "確認",
            DIALOGS_NO: "取消"
        });

        $translateProvider.preferredLanguage('zh-TW');

    }])
    //.run(['$templateCache', function ($templateCache) {
    //    $templateCache.put('/dialogs/custom.html', '<div class="modal-header"><h4 class="modal-title"><span class="glyphicon glyphicon-star"></span> User\'s Name</h4></div><div class="modal-body"><ng-form name="nameDialog" novalidate role="form"><div class="form-group input-group-lg" ng-class="{true: \'has-error\'}[nameDialog.username.$dirty && nameDialog.username.$invalid]"><label class="control-label" for="course">Name:</label><input type="text" class="form-control" name="username" id="username" ng-model="user.name" ng-keyup="hitEnter($event)" required><span class="help-block">Enter your full name, first &amp; last.</span></div></ng-form></div><div class="modal-footer"><button type="button" class="btn btn-default" ng-click="cancel()">Cancel</button><button type="button" class="btn btn-primary" ng-click="save()" ng-disabled="(nameDialog.$dirty && nameDialog.$invalid) || nameDialog.$pristine">Save</button></div>');
    //    $templateCache.put('/dialogs/custom2.html', '<div class="modal-header"><h4 class="modal-title"><span class="glyphicon glyphicon-star"></span> Custom Dialog 2</h4></div><div class="modal-body"><label class="control-label" for="customValue">Custom Value:</label><input type="text" class="form-control" id="customValue" ng-model="data.val" ng-keyup="hitEnter($event)"><span class="help-block">Using "dialogsProvider.useCopy(false)" in your applications config function will allow data passed to a custom dialog to retain its two-way binding with the scope of the calling controller.</span></div><div class="modal-footer"><button type="button" class="btn btn-default" ng-click="done()">Done</button></div>')
    //}])
    ;

})(window, document);

