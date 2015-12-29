//國內電話 domestic call 
//市內電話 local call 
//家裡電話 home call 

(function (window, document) {
    'use strict';

    angular.module('app').factory('GlobalSettings', [function () {

        //var pageSizes = [10, 100, 500, 2000];
        var pageSizes = [10, 50, 100, 200]; // 尚未測試200筆數，在進行多筆刪除時是否會發生錯誤(100筆是OK的)
        return {

            isSPA: true, // is single page application

            tokenSeparators: [
                ',',
                '，',
            ],

            schedulerOptions: {
                checkUserIntervals: 10 * 1000, // 10 seconds
            },

            paginationOptions: {
                pageNumber: 1,
                pageSize: pageSizes[0],
                pageSizes: pageSizes,
            },

            // every8d
            //patterns: {
            //    // 英文數字
            //    NumOrEn: /^\w{6,36}$/,
            //    // 數字
            //    Number: /^[0-9]*$/,
            //    // 電話號碼
            //    PhoneNumber: /^[+][1-9][0-9]{6,18}$|^[0-9]{8,20}$/,
            //    // Email
            //    Email: /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/,
            //},

            //patterns: {
            //    // 行動電話
            //    PhoneNumber: /^(09\d{8})$|^(886\d{9})$/,
            //    // 市內電話
            //    LocalCall: /^\d+$/,
            //    // 市內電話分機
            //    LocalCallExt: /^\d+$/,
            //    // Email
            //    Email: /^[\w-.]+@([\w-]+\.)+[\w-]+$/,
            //    // 日期格式(e.g. 12/31)
            //    Date_MMdd: /^(([1-9])|(0[1-9])|(1[0-2]))\/(([0-9])|([0-2][0-9])|(3[0-1]))$/,
            //    // 發送時間格式(e.g. 201512312359)
            //    SendTime: /^(19|20)\d{2}(0[1-9]|1[012])(0[1-9]|[12]\d|3[01])([01][0-9]|2[0-3])([0-5][0-9])$/,
            //},

            //validationMessage: {
            //    // 行動電話
            //    PhoneNumber: '不符合行動電話格式。範例：0912345678 或 886912345678。',

            //    // 市內電話
            //    LocalCall: '不符合市內電話格式。範例：1234567。',

            //    // 市內電話分機
            //    LocalCallExt: '不符合市內電話分機格式。範例：123。',

            //    // Email
            //    Email: '不符合電子郵件格式。範例：abc@gmail.com。',

            //    // 日期格式(e.g. 12/31)
            //    Date_MMdd: '不符合日期格式。範例：12/31。',

            //    // 發送時間格式(e.g. 201512312359)
            //    SendTime: '不符合發送時間格式。範例：201512312359。',
            //},

        };
    }]);

})(window, document);

