(function (window, document) {
    'use strict';

    // https://github.com/McNull/angular-block-ui

    angular.module('app').config(['blockUIConfig', function (blockUIConfig) {

        // http://tobiasahlin.com/spinkit/
        var spinkit = [
            '<div class="sk-fading-circle">',
            '  <div class="sk-circle1 sk-circle"></div>',
            '  <div class="sk-circle2 sk-circle"></div>',
            '  <div class="sk-circle3 sk-circle"></div>',
            '  <div class="sk-circle4 sk-circle"></div>',
            '  <div class="sk-circle5 sk-circle"></div>',
            '  <div class="sk-circle6 sk-circle"></div>',
            '  <div class="sk-circle7 sk-circle"></div>',
            '  <div class="sk-circle8 sk-circle"></div>',
            '  <div class="sk-circle9 sk-circle"></div>',
            '  <div class="sk-circle10 sk-circle"></div>',
            '  <div class="sk-circle11 sk-circle"></div>',
            '  <div class="sk-circle12 sk-circle"></div>',
            '</div>',
        ].join('\n');
        // Change the default overlay message
        //blockUIConfig.message = 'Please stop clicking!';
        //blockUIConfig.template = spinkit;

        // Change the default delay to 100ms before the blocking is visible
        blockUIConfig.delay = 100;
        blockUIConfig.blockBrowserNavigation = true;
        blockUIConfig.autoInjectBodyBlock = true;
        blockUIConfig.autoBlock = true;
        blockUIConfig.requestFilter = function (config) {
            // If the request starts with '/api/quote' ...

            var exclusives = [
                'api/LookupApi/GetCurrentUser'
            ];

            if (_.include(exclusives, config.url))
                return false;

            //if (config.url.match(/^\/api\/quote($|\/).*/)) {
            //    return false; // ... don't block it.
            //}

        };
        //blockUIConfig.cssClass = 'block-ui my-custom-class';
    }]);

})(window, document);

