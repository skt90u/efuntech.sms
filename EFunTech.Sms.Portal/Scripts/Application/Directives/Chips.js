// https://docs.angularjs.org/guide/directive
// https://docs.angularjs.org/api/ng/service/$compile#directive-definition-object
// http://stackoverflow.com/questions/15896985/callback-function-inside-directive-attr-defined-in-different-attr
// http://stackoverflow.com/questions/12801593/pass-parameters-from-directive-to-callback
// http://stackoverflow.com/questions/19889615/can-an-angular-directive-pass-arguments-to-functions-in-expressions-specified-in
(function (window, document) {
    'use strict';

    ////////////////////////////////////////
    // ChipTransclude
    ////////////////////////////////////////

    angular.module('app').directive('chipTransclude', ['$compile', function ($compile) {
        return {
            restrict: 'EA',
            terminal: true,
            require: '^^chips',
            scope: false,
            link: function (scope, element, attr, chipsCtrl) {
                var ctrl = chipsCtrl,
                    newScope = ctrl.parent.$new(false, ctrl.parent);

                newScope.chips = ctrl;
                newScope.$$replacedScope = scope;
                newScope.$chip = scope.$chip;

                element.html(ctrl.$scope.$eval(attr.chipTransclude));
                $compile(element.contents())(newScope);
            }
        };
    }]);

    ////////////////////////////////////////
    // ChipsCtrl
    ////////////////////////////////////////

    var ChipsCtrl = function ($scope, $element) {

        /** @type {angular.$scope} */
        this.$scope = $scope;

        /** @type {angular.$scope} */
        this.appScope = $element.closest('div[ng-controller]').scope();
        /** @type {angular.$scope} */
        this.parent = $scope.$parent;

        /** @type {$element} */
        this.$element = $element;

        /** @type {angular.NgModelController} */
        this.ngModelCtrl = null;

        /** @type {Array.<Object>} */
        this.items = [];

        this.chipSelectChange = $scope.chipSelectChange || angular.noop;

        this.multiSelect = false;
        this.selectedIndices = []; // 選取的索引
        this.selectedItems = []; // 選取的內容
    };

    ChipsCtrl.$inject = ["$scope", "$element"];

    ChipsCtrl.prototype.resetSelectedChip = function () {
        this.selectedIndices = [];
        this.selectedItems = [];
    };

    ChipsCtrl.prototype.include = function (index) {
        return _.includes(this.selectedIndices, index)
    };

    ChipsCtrl.prototype.selectAll = function () {
        this.selectedIndices = [];
        this.selectedItems = [];
        for (var index = 0; index < this.items.length; index++)
        {
            this.selectedIndices.push(index);
            this.selectedItems.push(this.items[index]);
        }
    };

    ChipsCtrl.prototype.selectChip = function (index) {

        if (index < 0) return;
        if (index >= this.items.length) return;

        // 如果是多選
        //  判斷是否已經選取
        //      如果是，取消選取
        //      如果否，加入選取
        //----------------------------------------
        // 如果是單選
        //  取消所有選取項目，只包含這個選取項目
        if (this.multiSelect) {
            if (_.includes(this.selectedIndices, index)) {
                this.selectedIndices = _.without(this.selectedIndices, index);
                this.selectedItems = _.without(this.selectedItems, this.items[index]);
            }
            else {
                this.selectedIndices.push(index);
                this.selectedItems.push(this.items[index]);
            }
        }
        else {
            this.selectedIndices = [index];
            this.selectedItems = [this.items[index]];
        }

        this.chipSelectChange(this.appScope, { selectedItems: this.selectedItems });

        //if (index >= -1 && index <= this.items.length) {
        //    this.selectedChip = index;
        //    this.$scope.$emit("chips.selectChip", this.items[index]);
        //    // in your controller
        //    // $scope.$on("chips.selectChip", function (event, item) { $log.log(item);});
        //}
    };

    angular.module('app').controller('ChipsCtrl', ChipsCtrl);

    ////////////////////////////////////////
    // ChipsDirective
    ////////////////////////////////////////

    angular.module('app').directive('chips', ['$parse', function ($parse) {

        return {
            template: function (element, attrs) {
                // Clone the element into an attribute. By prepending the attribute
                // name with '$', Angular won't write it into the DOM. The cloned
                // element propagates to the link function via the attrs argument,
                // where various contained-elements can be consumed.
                var content = attrs['$UserTemplate'] = element.clone();

                var CHIPS_TEMPLATE = [
                    '<chips-wrap class="chips">',
                        '<chip class="chip" ng-repeat="$chip in $ChipsCtrl.items" index="{{$index}}" ng-class="{\'focused\': $ChipsCtrl.include($index) }">',
                            '<div class="chip-content" chip-transclude="$ChipsCtrl.chipContentTemplate" ng-click="$ChipsCtrl.selectChip($index)"></div>', // 20150901 Norman, 只需要 ng-click 即可
                            '<div class="chip-action"  chip-transclude="$ChipsCtrl.chipActionTemplate"></div>',
                        '</chip>',
                    '</chips-wrap>'].join('\n');

                return CHIPS_TEMPLATE;
            },
            restrict: 'E',
            require: ['chips', 'ngModel'],
            controller: 'ChipsCtrl',
            controllerAs: '$ChipsCtrl', // a alias that can use in template
            bindToController: true,
            scope: {
                //type: '@',
                //readonly: '=readonly',
                //close: '&' not working
            },
            link: function (scope, element, attrs, ctrls) {

                var userTemplate = attrs['$UserTemplate'];
                attrs['$UserTemplate'] = null;

                var chipsCtrl = ctrls[0], ngModelCtrl = ctrls[1];

                // <chips ng-model="GroupManager.dataResult" multi-select="true">

                chipsCtrl.multiSelect = angular.isDefined(attrs.multiSelect) ? scope.$eval(attrs.multiSelect) : false;
                chipsCtrl.chipContentTemplate = getTemplateByQuery('chips > chip-content');
                chipsCtrl.chipActionTemplate = getTemplateByQuery('chips > chip-action');
                chipsCtrl.chipSelectChange = $parse(attrs.chipSelectChange);

                // 將 ng-model 設定 設定到 chipsCtrl.items
                ngModelCtrl.$render = function () {
                    chipsCtrl.items = ngModelCtrl.$viewValue;
                };

                function getTemplateByQuery(query) {
                    var element = userTemplate[0].querySelector(query);
                    return (element && element.outerHTML) || '';
                }
            }
        };
    }]);


})(window, document);

