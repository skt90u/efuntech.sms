(function (window, document) {
    'use strict';

    angular.module('app').factory('NumberUtil', [function () {
        
        function formatNumber(number, nDecimalPlace) {

            nDecimalPlace = nDecimalPlace || 1;

            var number = number.toFixed(nDecimalPlace) + '';
            var x = number.split('.');
            var x1 = x[0];
            var x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        }

        return {
            formatNumber: formatNumber,
        };
        
    }]);

})(window, document);