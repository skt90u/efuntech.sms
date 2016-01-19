(function (window, document) {
    'use strict';

    angular.module('app').
        controller('FileDescriptionCtrl', ['$scope', '$modalInstance', 'data', 
            function ($scope, $modalInstance, data) {

                $scope.data = data || {};

                $scope.cancel = function () {
                    $modalInstance.dismiss('cancel');
                };

               

            }]);

})(window, document);