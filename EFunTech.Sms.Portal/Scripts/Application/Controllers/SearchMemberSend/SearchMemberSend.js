(function (window, document) {
    'use strict';

    angular.module('app').controller('SearchMemberSend', ['$scope', '$log', function ($scope, $log) {

        $scope.onSelect = function (tabName) {
            $scope.tabName = tabName;
            $scope.$broadcast('tab.onSelect', tabName);
        };
        $scope.tabName = 'MemberSendMessage';
    }]);

})(window, document);