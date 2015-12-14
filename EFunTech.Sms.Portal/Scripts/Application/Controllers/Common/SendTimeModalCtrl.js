(function (window, document) {
    'use strict';

    function paddingZero(strlength, value){
        var str = '';
        for(var i=0; i<strlength; i++){
            str = str + '0';
        }

        str = str + (value || '').toString();

        return str.slice(-1 * strlength);

        // 補零
        //return String('00' + value).slice(-2);
    }

    function createOptions(array) {
        return _.map(array, function (value) {
            // 補零
            return paddingZero(2, value);
        });
    }

    // 判斷起始時間到結束時間是否包含指定條件
    function checkInDruation(startDate, endDate, fnPredicate){
        var startDate = moment(startDate);
        var endDate = moment(endDate);
        var dt = startDate;

        if (startDate.diff(endDate) > 0) // 結束時間比開始時間還早
            return false; 

        while (dt.diff(endDate) <= 0) {
            if (fnPredicate(dt.toDate()))
                return true;
            dt = dt.add(1, 'day');
        }

        return false;
    }
    
    function getUtcDayOfWeeks(sendTime, weekSelection) {

        var utcDayOfWeeks = '';
        var utcDayOfWeekIds = [];

        for (var i = 0; i < 7; i++) {
            var dayOfWeek = moment(sendTime).add(i, 'day').day();
            var found = _.findWhere(weekSelection, { id: dayOfWeek });
            if (found) {
                var utcDayOfWeek = moment(sendTime).add(i, 'day').utc().day();
                utcDayOfWeekIds.push(utcDayOfWeek);
            }
        }

        for (var i = 0; i < 7; i++) {
            utcDayOfWeeks += _.contains(utcDayOfWeekIds, i) ? '1' : '0';
        }

        return utcDayOfWeeks;
    }

    // [00, ..., 23]
    var Hours = createOptions(_.range(0, 24));
    // [00, ..., 59]
    var Minutes = createOptions(_.range(0, 60));
    // [01, ..., 31]
    var Days = createOptions(_.range(1, 32));
    // [01, ..., 12]
    var Months = createOptions(_.range(1, 13));

    var Weeks = [
        { id: 0, text: '週日' },
        { id: 1, text: '週一' },
        { id: 2, text: '週二' },
        { id: 3, text: '週三' },
        { id: 4, text: '週四' },
        { id: 5, text: '週五' },
        { id: 6, text: '週六' },
    ];

    angular.module('app').controller('SendTimeModalCtrl', ['$scope', 'CurrentUserManager', 'NumberUtil', 'LookupApi', 'DateUtil', 'dialogs', 'CrudApi', '$modalInstance',
        'SelectUtil', 'SendTimeType', 'SendCycleType',
        'options',
        function ($scope, CurrentUserManager, NumberUtil, LookupApi, DateUtil, dialogs, CrudApi, $modalInstance,
            SelectUtil, SendTimeType, SendCycleType,
            options) {

            var getDateTime = DateUtil.getDateTime;
            var getDateBegin = DateUtil.getDateBegin;
            var getDateEnd = DateUtil.getDateEnd;

            var SendTimeTypeOptions = SelectUtil.getEnumOptions(SendTimeType);
            var SendCycleTypeOptions = SelectUtil.getEnumOptions(SendCycleType);

            $scope.SendMessageRule = options.SendMessageRule;

            //========================================
            // Settings - SendTimeManager
            //========================================

            function SendTimeManager() {
                var self = this;

                var now = new Date();

                this.MinDate = now;

                this.SendTimeTypeOptions = SendTimeTypeOptions;
                this.SendCycleTypeOptions = SendCycleTypeOptions;
                this.Hours = Hours;
                this.Minutes = Minutes;
                this.Days = Days;
                this.Months = Months;
                this.Weeks = Weeks;

                this.SendTimeType = this.SendTimeTypeOptions.Immediately; // 發送時間的第一個類型
                this.SendCycleType = this.SendCycleTypeOptions.EveryDay;

                this.SendDeliver = {
                    Hour: paddingZero(2, now.getHours()),
                    Minute: paddingZero(2, now.getMinutes()),
                    Date: getDateEnd(now),
                    Date_DatepickerOpend: false,
                };

                this.SendCycleEveryDay = {
                    Hour: paddingZero(2, now.getHours()),
                    Minute: paddingZero(2, now.getMinutes()),
                    StartDate: getDateBegin(now),
                    EndDate: getDateEnd(now),

                    StartDate_DatepickerOpend: false,
                    EndDate_DatepickerOpend: false,
                };

                this.SendCycleEveryWeek = {
                    Hour: paddingZero(2, now.getHours()),
                    Minute: paddingZero(2, now.getMinutes()),
                    // dayOfWeeks 對應關係
                    // 0000000 <- 每一天都沒選
                    // 1000001 <- [Sunday, Saturday]
                    DayOfWeeks: '0000000',
                    StartDate: getDateBegin(now),
                    EndDate: getDateEnd(now),

                    StartDate_DatepickerOpend: false,
                    EndDate_DatepickerOpend: false,
                    // http://stackoverflow.com/questions/14514461/how-can-angularjs-bind-to-list-of-checkbox-values
                    WeekSelection: [],
                    ToggleWeekSelection: function (Week) {
                        if (_.includes(self.SendCycleEveryWeek.WeekSelection, Week)) {
                            self.SendCycleEveryWeek.WeekSelection = _.without(self.SendCycleEveryWeek.WeekSelection, Week);
                        }
                        else {
                            self.SendCycleEveryWeek.WeekSelection.push(Week);
                        }

                        var DayOfWeeks = '';
                        for (var i = 0; i < 7; i++) {
                            var found = -1 != _.findIndex(self.SendCycleEveryWeek.WeekSelection, function (item) {
                                return item.id == i;
                            });

                            DayOfWeeks += found ? '1' : '0';
                        }

                        self.SendCycleEveryWeek.DayOfWeeks = DayOfWeeks;
                    }
                };

                this.SendCycleEveryMonth = {
                    Hour: paddingZero(2, now.getHours()),
                    Minute: paddingZero(2, now.getMinutes()),
                    Day: paddingZero(2, now.getDate()),
                    StartDate: getDateBegin(now),
                    EndDate: getDateEnd(now),
                    
                    StartDate_DatepickerOpend: false,
                    EndDate_DatepickerOpend: false,
                };

                this.SendCycleEveryYear = {
                    Hour: paddingZero(2, now.getHours()),
                    Minute: paddingZero(2, now.getMinutes()),
                    Day: paddingZero(2, now.getDate()),
                    Month: paddingZero(2, now.getMonth()+1),
                    StartDate: getDateBegin(now),
                    EndDate: getDateEnd(now),

                    StartDate_DatepickerOpend: false,
                    EndDate_DatepickerOpend: false,
                };

                this.check = function () {

                    var SendMessageRule = $scope.SendMessageRule;
                    var message = '';
                    var errors = [];

                    SendMessageRule.SendTimeType = /* 不知道為什麼是字串，因此手動轉換成整數 */ +self.SendTimeType;

                    switch (SendMessageRule.SendTimeType) {
                        case SendTimeTypeOptions.Immediately:
                            {
                                // 無須額外檢驗
                            } break;
                        case SendTimeTypeOptions.Deliver:
                            {
                                var now = new Date();
                                var Hour = +self.SendDeliver.Hour;
                                var Minute = +self.SendDeliver.Minute;

                                var SendTime = new Date(
                                    self.SendDeliver.Date.getFullYear(),
                                    self.SendDeliver.Date.getMonth(),
                                    self.SendDeliver.Date.getDate(),
                                    +self.SendDeliver.Hour,
                                    +self.SendDeliver.Minute,
                                    0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差

                                if (SendTime < now)
                                    errors.push('預約發送時間不得小於等於目前時間');

                                if (errors.length == 0) {
                                    // 設定參數到 SendMessageRule
                                    SendMessageRule.SendDeliver = {
                                        SendTime: SendTime,
                                    };
                                    SendMessageRule.SendCycleEveryDay = null;
                                    SendMessageRule.SendCycleEveryWeek = null;
                                    SendMessageRule.SendCycleEveryMonth = null;
                                    SendMessageRule.SendCycleEveryYear = null;
                                }
                            } break;
                        case SendTimeTypeOptions.Cycle:
                            {
                                var SendCycleType = /* 不知道為什麼是字串，因此手動轉換成整數 */ +self.SendCycleType;

                                switch (SendCycleType) {
                                    case SendCycleTypeOptions.EveryDay:
                                        {
                                            var now = new Date();
                                            var Hour = +self.SendCycleEveryDay.Hour;
                                            var Minute = +self.SendCycleEveryDay.Minute;
                                            var StartDate = getDateBegin(self.SendCycleEveryDay.StartDate);
                                            var EndDate = getDateEnd(self.SendCycleEveryDay.EndDate);

                                            var SendTime = new Date(
                                                StartDate.getFullYear(),
                                                StartDate.getMonth(),
                                                StartDate.getDate(),
                                                Hour,
                                                Minute,
                                                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差

                                            if(StartDate < getDateBegin(now)) {
                                                errors.push('起始時間不得小於目前時間');
                                            }
                                            if (EndDate < StartDate) {
                                                errors.push('結束時間不得小於起始時間');
                                            }

                                            if (errors.length == 0) {
                                                // 設定參數到 SendMessageRule
                                                SendMessageRule.SendDeliver = null;
                                                SendMessageRule.SendCycleEveryDay = {
                                                    SendTime: SendTime,
                                                    StartDate: StartDate,
                                                    EndDate: EndDate,
                                                };
                                                SendMessageRule.SendCycleEveryWeek = null;
                                                SendMessageRule.SendCycleEveryMonth = null;
                                                SendMessageRule.SendCycleEveryYear = null;
                                            }
                                        } break;
                                    case SendCycleTypeOptions.EveryWeek:
                                        {
                                            var now = new Date();
                                            var Hour = +self.SendCycleEveryWeek.Hour;
                                            var Minute = +self.SendCycleEveryWeek.Minute;
                                            var StartDate = getDateBegin(self.SendCycleEveryWeek.StartDate);
                                            var EndDate = getDateEnd(self.SendCycleEveryWeek.EndDate);

                                            var SendTime = new Date(
                                                StartDate.getFullYear(),
                                                StartDate.getMonth(),
                                                StartDate.getDate(),
                                                Hour,
                                                Minute,
                                                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差
                                            
                                            var DayOfWeeks = getUtcDayOfWeeks(SendTime, self.SendCycleEveryWeek.WeekSelection); // UtcTime的星期
                                            //var DayOfWeeks = self.SendCycleEveryWeek.DayOfWeeks; // LocalTime的星期

                                            if (self.SendCycleEveryWeek.WeekSelection.length == 0){
                                                errors.push('每周發送請至少選擇一天');
                                            }
                                            if(StartDate < getDateBegin(now)) {
                                                errors.push('起始時間不得小於目前時間');
                                            }
                                            if (EndDate < StartDate) {
                                                errors.push('結束時間不得小於起始時間');
                                            }

                                            // 判斷起始時間到結束時間是否包含任何發送時間
                                            var weekIds = _.pluck(self.SendCycleEveryWeek.WeekSelection, 'id');
                                            var pass = checkInDruation(StartDate, EndDate, function (dt) {
                                                return _.contains(weekIds, dt.getDay());
                                            });
                                            if (!pass) {
                                                errors.push('在指定起訖時間中，不包含任何發送時間');
                                            }

                                            if (errors.length == 0) {
                                                // 設定參數到 SendMessageRule
                                                SendMessageRule.SendDeliver = null;
                                                SendMessageRule.SendCycleEveryDay = null;
                                                SendMessageRule.SendCycleEveryWeek = {
                                                    SendTime: SendTime,
                                                    DayOfWeeks: DayOfWeeks,
                                                    StartDate: StartDate,
                                                    EndDate : EndDate,
                                                };
                                                SendMessageRule.SendCycleEveryMonth = null;
                                                SendMessageRule.SendCycleEveryYear = null;
                                            }

                                            
                                        } break;
                                    case SendCycleTypeOptions.EveryMonth:
                                        {
                                            var now = new Date();
                                            var Hour = +self.SendCycleEveryMonth.Hour;
                                            var Minute = +self.SendCycleEveryMonth.Minute;
                                            var Day = + self.SendCycleEveryMonth.Day;
                                            var StartDate = getDateBegin(self.SendCycleEveryMonth.StartDate);
                                            var EndDate = getDateEnd(self.SendCycleEveryMonth.EndDate);

                                            var SendTime = new Date(
                                                StartDate.getFullYear(),
                                                StartDate.getMonth(),
                                                Day,
                                                Hour,
                                                Minute,
                                                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差

                                            if(StartDate < getDateBegin(now)) {
                                                errors.push('起始時間不得小於目前時間');
                                            }
                                            if (EndDate < StartDate) {
                                                errors.push('結束時間不得小於起始時間');
                                            }

                                            // 判斷起始時間到結束時間是否包含任何發送時間
                                            var pass = checkInDruation(StartDate, EndDate, function (dt) {
                                                return dt.getDate() == SendTime.getDate();
                                            });
                                            if (!pass) {
                                                errors.push('在指定起訖時間中，不包含任何發送時間');
                                            }

                                            if (errors.length == 0) {
                                                // 設定參數到 SendMessageRule
                                                SendMessageRule.SendDeliver = null;
                                                SendMessageRule.SendCycleEveryDay = null;
                                                SendMessageRule.SendCycleEveryWeek = null;
                                                SendMessageRule.SendCycleEveryMonth = {
                                                    SendTime: SendTime,
                                                    StartDate: StartDate,
                                                    EndDate: EndDate,
                                                };
                                                SendMessageRule.SendCycleEveryYear = null;
                                            }
                                        } break;
                                    case SendCycleTypeOptions.EveryYear:
                                        {
                                            var now = new Date();
                                            var Hour = + self.SendCycleEveryYear.Hour;
                                            var Minute = +self.SendCycleEveryYear.Minute;
                                            var Day = +self.SendCycleEveryYear.Day;
                                            var Month = +self.SendCycleEveryYear.Month;
                                            var StartDate = getDateBegin(self.SendCycleEveryYear.StartDate);
                                            var EndDate = getDateEnd(self.SendCycleEveryYear.EndDate);
                                                
                                            var SendTime = new Date(
                                                StartDate.getFullYear(),
                                                Month - 1, /* javascript month range = [0...11] */
                                                Day,
                                                Hour,
                                                Minute,
                                                0); // 必須設為 0，否則 Hangfire 會發生一分鐘的誤差

                                            if(StartDate < getDateBegin(now)) {
                                                errors.push('起始時間不得小於目前時間');
                                            }
                                            if (EndDate < StartDate) {
                                                errors.push('結束時間不得小於起始時間');
                                            }

                                            // 判斷起始時間到結束時間是否包含任何發送時間
                                            var pass = checkInDruation(StartDate, EndDate, function (dt) {
                                                return dt.getMonth() == SendTime.getMonth() &&
                                                       dt.getDate() == SendTime.getDate();
                                            });
                                            if (!pass) {
                                                errors.push('在指定起訖時間中，不包含任何發送時間');
                                            }

                                            if (errors.length == 0) {
                                                // 設定參數到 SendMessageRule
                                                SendMessageRule.SendDeliver = null;
                                                SendMessageRule.SendCycleEveryDay = null;
                                                SendMessageRule.SendCycleEveryWeek = null;
                                                SendMessageRule.SendCycleEveryMonth = null;
                                                SendMessageRule.SendCycleEveryYear = {
                                                    SendTime: SendTime,
                                                    StartDate: StartDate,
                                                    EndDate: EndDate,
                                                };
                                            }
                                        } break;
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

                }; // this.check
            }

            $scope.SendTimeManager = new SendTimeManager();

            $scope.submit = function () {

                var SendTimeManager = $scope.SendTimeManager;
                if (!SendTimeManager.check()) return;
                var SendMessageRule = $scope.SendMessageRule; // 請放在 SendTimeManager.check 之後，因為 SendTimeManager.check 會更新 SendMessageRule

                var message = '';
                switch (SendMessageRule.SendTimeType) {
                    case SendTimeTypeOptions.Immediately:
                        {
                            message = "確認使用立即發送？";
                        } break;
                    case SendTimeTypeOptions.Deliver:
                        {
                            message = "確認使用預約發送？";
                        } break;
                    case SendTimeTypeOptions.Cycle:
                        {
                            message = "確認使用週期發送？";
                        } break;
                }
                dialogs.confirm('簡訊發送', message).result.then(function (btn) {
                    // 發送簡訊需求
                    var model = SendMessageRule;
                    var crudApi = new CrudApi('api/SendMessageRule');
                    crudApi.Create(model)
                    .then(function (result) {
                        // 更新使用者點數
                        CurrentUserManager.updateSmsBalance();

                        // 最後關閉視窗
                        $modalInstance.close(result.data);
                    });
                });
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };

        }]);

   
})(window, document);
