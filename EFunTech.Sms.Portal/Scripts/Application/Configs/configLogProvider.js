(function (window, document) {
    'use strict';

    // http://stackoverflow.com/questions/15561853/how-to-turn-on-off-log-debug-in-angularjs
    angular.module('app').config(['$logProvider', function ($logProvider) {
        //$logProvider.debugEnabled(window.enableDebug || false);
        $logProvider.debugEnabled(true);
    }]);

})(window, document);

