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

        function getTitleMap(enumDefine)
        {
            var nameValues = angular.copy(_.values(enumDefine));

            angular.forEach(nameValues, function (v, k) {
                v.value = v.value.toString(); // schema form want string to present value's type
                v.name = v.text;
            });

            return nameValues;
        }

        function getEnum(enumDefine) {
            return _.pluck(getTitleMap(enumDefine), 'value');
        }

        return {

            getScope: getScope,

            //cleanupError: cleanupError,

            getViewValue: getViewValue,

            getEnum: getEnum,

            getTitleMap: getTitleMap,

            $broadcast: $broadcast,
        };
    }]);

})(window, document);

