(function (window, document) {
    'use strict';

    // http://www.angulartutorial.net/2014/05/set-headers-for-all-http-calls-in.html
    angular.module('app').run(['$http', 'TimezoneDetect', '$cookies', function ($http, TimezoneDetect, $cookies) {

        var TimezoneOffset = moment().format("Z"); // "+08:00"
        // https://github.com/Canop/tzdetect.js
        var TimezoneIds = TimezoneDetect.matches().join(','); // for example : "Antarctica/Casey,Asia/Brunei,Asia/Chongqing,Asia/Chungking,Asia/Harbin,Asia/Hong_Kong,Asia/Kuala_Lumpur,Asia/Kuching,Asia/Macao,Asia/Macau,Asia/Makassar,Asia/Manila,Asia/Shanghai,Asia/Singapore,Asia/Taipei,Asia/Ujung_Pandang,Australia/Perth,Australia/West,Etc/GMT-8,Hongkong,PRC,ROC,Singapore"

        $http.defaults.headers.common.TimezoneOffset = TimezoneOffset;
        $http.defaults.headers.common.TimezoneIds = TimezoneIds;

        // 20151021 Norman, window.open 目前不知道如何設定 header，因此再把相關資訊加入 cookies 之中
        $cookies.remove('TimezoneOffset');
        $cookies.remove('TimezoneIds');
        $cookies.put('TimezoneOffset', TimezoneOffset);
        $cookies.put('TimezoneIds', TimezoneIds);
    }]); 

})(window, document);

