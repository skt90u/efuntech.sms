(function (window, document) {
    'use strict';

    angular.module('app').factory('SelectUtil', [function () {

        /*
        將
        var Gender = {
            Unknown: {value: 0,  text: '不詳'},
            Male: {value: 1,  text: '男性'},
            Female: {value: 2,  text: '女性'},
        };
         */
        function getEnumOptions(enumDefine, propertyName) {

            propertyName = propertyName || 'value';

            var result = {};

            _.each(enumDefine, function (v, k) {
                result[k] = v[propertyName];
            })

            return result;
        }

        return {
            getEnumOptions: getEnumOptions,
        };

    }]);

})(window, document);