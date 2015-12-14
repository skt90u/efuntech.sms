// https://docs.angularjs.org/guide/module
(function (window, document) {
    'use strict';

    var deps = [];

    var moduleNames = [
        'ngAnimate',
        'ngCookies',
        'angular-underscore',
        'toaster',
        'ui.bootstrap',
        'schemaForm',
        'ui.grid',
        'ui.grid.edit',
        'ui.grid.pagination',
        'ui.grid.selection',
        'ui.grid.saveState',
        'ui.grid.cellNav',
        'ui.grid.resizeColumns',
        'ui.grid.moveColumns',
        'ui.grid.pinning',
        'ui.grid.autoResize',
        'ngMaterial',
        'blockUI',
        'pascalprecht.translate',
        'dialogs.main',
        'ngFileSaver',
    ];

    while (moduleNames.length) {
        var moduleName = moduleNames.shift();

        try {
            //This throws an expection if module does not exist.
            angular.module(moduleName);
            deps.push(moduleName);
        } catch (e) { }
    }

    var app = angular.module('app', deps);
    
})(window, document);