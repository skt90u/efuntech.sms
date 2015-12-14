(function (window, document) {
    'use strict';

    /*
     * 管理畫面右上角，相關功能
     * -->   Hi Administrator 點數： 2147483656.0 (儲值) 我要儲值 | 登出
     */
    angular.module('app').service('CurrentUserManager', ['LookupApi', 'NumberUtil',
        function (LookupApi, NumberUtil) {

            var self = this;
            
            // 更新使用者點數
            this.updateSmsBalance = function (newSmsBalance) {
                
                if (angular.isUndefined(newSmsBalance)) {
                    LookupApi.GetCurrentUser({}, function (data) {
                        self.updateSmsBalance(data.SmsBalance);
                    });
                }
                else {
                    var oldValue = $('#SmsBalance').text();
                    var newValue = NumberUtil.formatNumber(newSmsBalance, 1 /* 到小數點第一位　*/);

                    if (oldValue !== newValue) {
                        $('#SmsBalance').text(newValue);
                    }
                }

                // 20151104 Norman, 原本邏輯
                //LookupApi.GetCurrentUser({}, function (data) {
                //    var SmsBalance = NumberUtil.formatNumber(data.SmsBalance, 1 /* 到小數點第一位　*/);
                //    $('#SmsBalance').text(SmsBalance);
                //});
            };

        }]);

})(window, document);
