(function (window, document) {
    'use strict';

    // Single Page Application using location.hash
    // reference: http://demo.tutorialzine.com/2015/02/single-page-app-without-a-framework/#
    angular.module('app').controller('MenuCtrl', ['$scope', '$log', function ($scope, $log) {

        // 取得目前使用者可以選擇的選單
        //function getAvailableViews() {
        //    var views = [];
        //    var links = angular.element('#menu').find('a');
        //    angular.forEach(links, function (link, idx) {
        //        var view = angular.element(link).attr('href');
        //        view = view.replace('#!/', '');
        //        views.push(view);
        //    });
        //    return views;
        //}
        
        //$scope.availableViews = getAvailableViews();

        //$scope.$watch(function () {
        //    return location.hash;
        //}, function (value) {
            
        //    console.log(value);

        //    return;

        //    if (_.contains($scope.availableViews, view)) {
        //        $scope.selectedView = view;

        //        $scope.$broadcast('menu.onSelect', view);

        //        // 設定哪個選單被點選
        //        var links = angular.element('#menu').find('a');
        //        angular.forEach(links, function (link) {
        //            link = angular.element(link);
        //            var href = link.attr('href'); // #SectorStatistics
        //            if (href == '#' + view) {
        //                link.addClass('menu-active');
        //                link.removeClass('menu');
        //            }
        //            else {
        //                link.addClass('menu');
        //                link.removeClass('menu-active');
        //            }
        //        });
        //    }
        //});

        //if (location.hash !== '') {
        //    var value = location.hash;
        //    var tokens = value.split('/');
        //    var view = tokens[tokens.length - 1];
        //    if (!_.contains($scope.availableViews, view)) {
        //        location.hash = '#' + $scope.availableViews[0];
        //    }
        //}
        //else {
        //    if ($scope.availableViews.length != 0) {
        //        location.hash = '#' + $scope.availableViews[0];
        //    }
        //}

        //window.location.hash
        $scope.$window = $window;
        $scope._ = _;
        //$scope.isActive = function () {
        //    return "menu";
        //};
        //$scope.getBackgroundColour = function () {
        //    console.log('c');
        //    return "menu";
        //};
        console.log('a');
        console.log('b');
        console.log('c');
        console.log('d');
        console.log('e');
    }]);

})(window, document);