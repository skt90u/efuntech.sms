(function (window, document) {
    'use strict';

    angular.module('app').filter('UtcToLocalTimeFilter', [function () {
        return function (input, momentFormat) {
            
            var DateTimeMinValue = '0001-01-01T00:00:00';

            if (input == null) return '';
            if (input == DateTimeMinValue) return '';
            
            if (angular.isUndefined(momentFormat)) return '尚未指定轉換字串格式';

            var localTime = moment.utc(input).toDate();

            return moment(localTime).format(momentFormat);
        };
    }]);

})(window, document);
