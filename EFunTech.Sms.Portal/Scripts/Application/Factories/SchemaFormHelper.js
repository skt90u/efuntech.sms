(function (window, document) {
    'use strict';

    angular.module('app').factory('SchemaFormHelper', ['$log', 'toaster', 'SchemaFormCache', function ($log, toaster, SchemaFormCache) {

        function getScope() {
            var $form = $('form[name=modalForm]');
            if ($form.length == 0) {
                $log.error('找不到 modalForm');
                return void 0;
            }

            return angular.element($('form[name=modalForm]')).scope();
        }

        function $broadcast() {
            var scope = getScope();
            scope.$broadcast.apply(scope, arguments);
        }

        //function cleanupError(fieldName, errorCode) {
        //    // angular.element($('form[name=modalForm]')).scope().$broadcast('schemaForm.error.NewPasswordConfirmed', 'passwordNotMatch', true);
        //    $broadcast('schemaForm.error.' + fieldName, errorCode, true);
        //}

        function getViewValue(fieldName) {
            var scope = getScope();
            
            if (!_.has(scope.modalForm, fieldName)) {
                $log.error('modalForm 指定欄位不存在 - ' + fieldName);
                return void 0;
            }

            return scope.modalForm[fieldName].$viewValue;
        }
        return {

            getScope: getScope,

            //cleanupError: cleanupError,

            getViewValue: getViewValue,

            $broadcast: $broadcast,
        };
    }]);

})(window, document);

