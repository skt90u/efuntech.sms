(function (window, document) {
    'use strict';

    angular.module('app').controller('ContactManager', ['$scope', '$log', function ($scope, $log) {

        $scope.onSelect = function (tabName) {
            $scope.tabName = tabName;
            $scope.$broadcast('tab.onSelect', tabName);
        };
        $scope.tabName = 'AllContact';
    }]);

})(window, document);