(function (window, document) {
    'use strict';

    angular.module('app').filter('SelectedMembersFilter', [function () {
        return function (input) {

            if (angular.isUndefined(input) ||
                input == null ||
                input.length == 0) return '';

            var arrFullName = _.pluck(input, 'FullName');

            var maxSize = Math.min(arrFullName.length, 5 /* 最多顯示筆數 */);

            return arrFullName.slice(0, maxSize).join('、') + '...等' + arrFullName.length + '個成員';
        };
    }]);

})(window, document);
