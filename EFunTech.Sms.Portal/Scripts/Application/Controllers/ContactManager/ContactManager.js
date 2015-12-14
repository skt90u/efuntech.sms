(function (window, document) {
    'use strict';

    angular.module('app').controller('ContactManager', ['$scope', '$log', function ($scope, $log) {

        $scope.onSelect = function (tabName) {
            $scope.$broadcast('tab.onSelect', tabName);
        };

    }]);

})(window, document);