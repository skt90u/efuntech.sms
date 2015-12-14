(function (window, document) {
    'use strict';

    angular.module('app').service('ManualInputManager', ['RegularExpressionPatterns', 'MobileUtil', 'GlobalSettings',
        function (RegularExpressionPatterns, MobileUtil, GlobalSettings) {

            var self = this;

            this.checkManualInput = function () {
                var tokenSeparators = GlobalSettings.tokenSeparators;
                var tokens = self.manualInput.split(new RegExp(tokenSeparators.join('|'), 'g'));
                self.phoneNumbers = _.filter(tokens, function (token) {
                    return MobileUtil.isPossibleNumber(token);
                });
            };

            this.init = function () {
                self.manualInput = '';
                self.phoneNumbers = [];
            };

            this.manualInput = '';
            this.phoneNumbers = [];

        }]);

})(window, document);
