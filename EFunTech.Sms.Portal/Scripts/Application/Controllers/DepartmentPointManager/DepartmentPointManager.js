(function (window, document) {
    'use strict';

    angular.module('app').controller('DepartmentPointManager', ['$scope', 'CompanyDeaprtmentManager', '$log', 'GlobalSettings',
        function ($scope, CompanyDeaprtmentManager, $log, GlobalSettings) {

        $scope.onSelect = function (tabName) {
            $scope.tabName = tabName;
            $scope.$broadcast('tab.onSelect', tabName);
        };

        $scope.CompanyDeaprtmentManager = CompanyDeaprtmentManager;
        $scope.tabName = 'DepartmentPointManagerManually';
    }]);

})(window, document);
