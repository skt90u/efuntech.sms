(function (window, document) {
    'use strict';

    angular.module('app').controller('Scheduler', ['$scope', '$log', '$interval', 'LookupApi', '$window', 'dialogs', 'CurrentUserManager', 'GlobalSettings',
        function ($scope, $log, $interval, LookupApi, $window, dialogs, CurrentUserManager, GlobalSettings) {
            
            var checkUserIntervals = GlobalSettings.schedulerOptions.checkUserIntervals;

            ////////////////////////////////////////
            // 避免某一個與實際登錄使用者不相同
            ////////////////////////////////////////
            var promise = $interval(function () {

                var frontend_UserName = $("meta[name=user-login]").attr("content") || null;
                if (!frontend_UserName) return;

                // 取得目前使用者名稱
                LookupApi.GetCurrentUser({}, function (data) {
                    var backend_UserName = data ? data.UserName : null;
                    
                    if (backend_UserName !== frontend_UserName) {

                        $interval.cancel(promise);

                        if (backend_UserName == null) {
                            var message = "使用者 " + frontend_UserName + " 已經登入，請重新登入此頁面";
                            dialogs.notify('登出使用者', message).result.then(function (btn) {
                                $window.location.reload();
                            });
                        }
                        else {
                            var message = "使用者 " + frontend_UserName + " 已切換為使用者 " + backend_UserName + " ，將重新載入此頁面";
                            dialogs.notify('切換使用者', message).result.then(function (btn) {
                                $window.location.reload();
                            });
                        }

                    }
                    else {
                        if (data != null) {
                            CurrentUserManager.updateSmsBalance(data.SmsBalance);
                        }
                    }
                });

            }, checkUserIntervals); // 避免某一個與實際登錄使用者不相同

        }]);

})(window, document);
