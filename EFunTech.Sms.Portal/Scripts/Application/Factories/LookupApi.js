// 請勿修改這個檔案，因為這個檔案是由 EFunTech.Sms.CodeGenerator 自動產生的，任何修改都將被覆蓋掉
(function (window, document) {
    'use strict';

    angular.module('app').factory('LookupApi', ['$http', '$log', 'toaster', function ($http, $log, toaster) {

        var functions = {};

        var functionNames = [
            'GetExistentUserNames', // GetExistentUserNames ()
            'GetCurrentUser', // GetCurrentUser ()
            'GetAvailableRoles', // GetAvailableRoles ()
            'GetAvailableDepartments_DepartmentManager', // GetAvailableDepartments_DepartmentManager ()
            'GetAvailableDepartments_ShareContact', // GetAvailableDepartments_ShareContact ()
        ];

        angular.forEach(functionNames, function (functionName) {

            functions[functionName] = function (params, successFn, errorFn) {

                var url = 'api/LookupApi/' + functionName;

                var res = $http.get(url, { params: angular.copy(params) });

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    (successFn || angular.noop).apply(null, arguments);
                });

                res.error(function (data, status, headers, config, statusText) {
                    $log.error(arguments);
                    toaster.pop({
                        type: 'error',
                        title: functionName,
                        body: '查詢失敗',
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };
        });

        return functions; // LookupApi
    }]);

})(window, document);
