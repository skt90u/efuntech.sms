(function (window, document) {
    'use strict';

    angular.module('app').factory('SchemaFormCache', ['$cacheFactory', function ($cacheFactory) {
          return $cacheFactory('SchemaFormCache');
    }]);

})(window, document);

