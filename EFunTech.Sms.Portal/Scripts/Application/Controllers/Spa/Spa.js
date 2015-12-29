(function (window, document) {
    'use strict';

    // Single Page Application using location.hash
    // reference: http://demo.tutorialzine.com/2015/02/single-page-app-without-a-framework/#
    angular.module('app').controller('Spa', ['$scope', '$log', function ($scope, $log) {

        // 取得目前使用者可以選擇的選單
        function getAvailableViews() {
            var views = [];
            var links = angular.element('#menu').find('a');
            angular.forEach(links, function (link, idx) {
                var view = angular.element(link).attr('href').replace('#', '');
                views.push(view);
            });
            return views;
        }
        
        $scope.$watch(function () {
            return location.hash;
        }, function (value) {
            // location.hash 改變時，對應處理動作
            var tokens = value.split('/');
            var view = tokens[tokens.length - 1];
            if (_.contains($scope.availableViews, view)) {
                $scope.selectedView = view;

                $scope.$broadcast('menu.onSelect', view);
            }
        });

        $scope.availableViews = getAvailableViews();

        if ($scope.availableViews.length != 0) {
            location.hash = '#' + $scope.availableViews[0];
        }
        
        $scope._ = _;

    }]);

})(window, document);