// 請勿修改這個檔案，因為這個檔案是由 EFunTech.Sms.CodeGenerator 自動產生的，任何修改都將被覆蓋掉
(function (window, document) {
    'use strict';

    angular.module('app').factory('ValidationApi', ['$http', '$log', 'toaster', function ($http, $log, toaster) {
        
        var functions = {};

        var functionNames = [
            'MakeSureUserNameNotExists', // MakeSureUserNameNotExists (UserName)
        ];
        
        angular.forEach(functionNames, function (functionName) {

            functions[functionName] = function (params, successFn, errorFn) {

                var url = '/api/ValidationApi/' + functionName;

                var res = $http.get(url, { params: angular.copy(params) });

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    (successFn || angular.noop).apply(null, arguments);
                });

                res.error(function (data, status, headers, config, statusText) {
                    $log.error(arguments);
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };
        });

        return functions; // ValidationApi

    }]);

})(window, document);
