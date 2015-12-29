(function (window, document) {
    'use strict';

    angular.module('app').controller('DepartmentPointManager', ['$scope', 'CompanyDeaprtmentManager', '$log',
        function ($scope, CompanyDeaprtmentManager, $log) {

        $scope.onSelect = function (tabName) {
            $scope.tabName = tabName;
            $scope.$broadcast('tab.onSelect', tabName);
        };

        $scope.CompanyDeaprtmentManager = CompanyDeaprtmentManager;
        $scope.tabName = 'DepartmentPointManagerManually';
    }]);

})(window, document);
