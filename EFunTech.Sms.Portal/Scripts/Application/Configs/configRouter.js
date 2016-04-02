(function (window, document) {
    'use strict';

    angular.module('app').config(['$routeProvider', '$locationProvider', 
        function ($routeProvider, $locationProvider) {

            function getAvailableViews() {
                var views = [];
                var links = angular.element('#menu').find('a');
                angular.forEach(links, function (link, idx) {
                    var view = angular.element(link).attr('href');
                    view = view.replace('#!/', '');
                    views.push(view);
                });
                return views;
            }

            var views = getAvailableViews();

            $locationProvider.hashPrefix('!');

            angular.forEach(views, function (view, idx) {
                $routeProvider.
                    when("/" + view, { templateUrl: "partials/" + view + ".html", caseInsensitiveMatch: true });
            });

            if (views.length != 0)
            {
                $routeProvider.
                    otherwise({ redirectTo: "/" + views[0] });

                // 修正 <a href="/"><img class="logo"></a> 無法正確導入首頁
                //$('.logo').parent().attr('href', window.location.origin);
                $('.logo').parent().attr('href', '#!/' + views[0]);
            }
            
        
    }]);

})(window, document);

