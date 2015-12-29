(function (window, document) {
    'use strict';

    angular.module('app').controller('RecurringSMS', ['$scope', '$log', function ($scope, $log) {

        $scope.onSelect = function (tabName) {
            $scope.tabName = tabName;
            $scope.$broadcast('tab.onSelect', tabName);
        };
        $scope.tabName = 'SendDeliverRule';
    }]);

})(window, document);