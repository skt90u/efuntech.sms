(function (window, document) {
    'use strict';

    angular.module('app').factory('SchemaFormFactory', ['$log', 'toaster', 'SchemaFormCache', function ($log, toaster, SchemaFormCache) {

        return {

            create: function (modelName, options) {

                if (angular.isUndefined(options)) {
                    $log.warn('SchemaFormFactory.create(' + modelName + '), 未指定 options ');
                }

                var fn = SchemaFormCache.get(modelName);

                if (angular.isUndefined(fn)) {
                    var message = '取得SchemaForm資料失敗，找不到模組 - ' + modelName + ' 的對應的 factory function';
                    $log.error(message);
                    toaster.pop({
                        type: 'error',
                        title: '取得SchemaForm資料失敗',
                        body: '找不到模組 - ' + modelName + ' 的對應設定',
                        showCloseButton: true,
                    });
                    return void 0;
                }

                if (!angular.isFunction(fn)) {
                    var message = '取得SchemaForm資料失敗，找不到模組 - ' + modelName + ' 的對應的資料並不是 function 物件';
                    $log.error(message);
                    toaster.pop({
                        type: 'error',
                        title: '取得SchemaForm資料失敗',
                        body: '找不到模組 - ' + modelName + ' 的對應的資料並不是 function 物件',
                        showCloseButton: true,
                    });
                    return void 0;
                }

                var result = fn.apply(null, [options]);

                return angular.copy(result);
            },
        };
    }]);

})(window, document);

