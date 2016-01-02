(function (window, document) {
    'use strict';

    angular.module('app').service('MenuManager', ['$log',
        function ($log) {


            //var menuItems = [];

            //function getMenuItems() {
                
            //    if (menuItems.length !== 0) return menuItems;
                
            //    console.log('bbbb');

            //    var links = angular.element('#menu').find('a');
            //    angular.forEach(links, function (link, idx) {
                    
            //        var $link = angular.element(link);
                    
            //        var href = $link.attr('href');
            //        var mapRouteUrl = href.replace('#!/', '');
            //        var name = $link.html();

            //        menuItems.push({
            //            href: href,
            //            mapRouteUrl: mapRouteUrl,
            //            name: name,
            //        });
                    
            //    });

            //    return menuItems;
            //}

            
            //this.getMenuItems = getMenuItems;

        }]);

})(window, document);
