(function (window, document) {
    'use strict';

    angular.module('app').factory('DateUtil', [function () {
        
        function getDateTime(date, hour, minute, second) {
            return new Date(
                date.getFullYear(),
                date.getMonth(),
                date.getDate(),
                hour,
                minute,
                second);
        }

        function getDateBegin(date) {
            return getDateTime(date, 0, 0, 0);
        }

        function getDateEnd(date) {
            return getDateTime(date, 23, 59, 59);
        }

        function getDateEnd(date) {
            return getDateTime(date, 23, 59, 59);
        }

        function toLocalTime(input) {
            return moment.utc(input).toDate();
        }

        function toDate(input) {
            return _.isDate(input)
                ? input 
                : moment(input).toDate();
        }

        return {
            getDateTime: getDateTime,
            getDateBegin: getDateBegin,
            getDateEnd: getDateEnd,
            toLocalTime: toLocalTime,
            toDate: toDate,
        };
        
    }]);

})(window, document);