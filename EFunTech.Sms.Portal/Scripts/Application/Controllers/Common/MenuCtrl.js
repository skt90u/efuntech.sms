(function (window, document) {
    'use strict';

    // Single Page Application using location.hash
    // reference: http://demo.tutorialzine.com/2015/02/single-page-app-without-a-framework/#
    angular.module('app').controller('MenuCtrl', ['$scope', '$interval', function ($scope, $interval) {

        $scope.$watch(function () {
            return location.hash;
        }, function (value) {

            // 設定哪個選單被點選
            var links = angular.element('#menu').find('a');
            angular.forEach(links, function (link) {
                link = angular.element(link);
                var href = link.attr('href');
                if (href == value) {
                    link.addClass('menu-active');
                    //link.removeClass('menu');
                }
                else {
                    //link.addClass('menu');
                    link.removeClass('menu-active');
                }
            });
        });
    }]);

})(window, document);