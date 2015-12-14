(function (window, document) {
    'use strict';

    angular.module('app')
           .filter('EnumFilter', ['EnumMapping', '$log',
               function (EnumMapping, $log) {
        return function (input, filterType) {

            var enumDefine = EnumMapping[filterType];

            if (angular.isUndefined(enumDefine)) {
                $log.error('未定義' + filterType);
                return input;
            }

            var text = null;

            _.each(enumDefine, function (v, k) {

                if (v.value === input) {
                    text = v.text;
                    // TODO: i don't know how to break this loop
                }
            });

            if (text == null) {
                $log.error('在' + filterType + '中，找不到 value = ' + input + '的定義');
                return input;
            }

            return text;
        };
    }]);

})(window, document);
