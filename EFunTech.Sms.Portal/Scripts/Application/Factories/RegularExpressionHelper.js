//國內電話 domestic call 
//市內電話 local call 
//家裡電話 home call 

(function (window, document) {
    'use strict';

    angular.module('app').factory('RegularExpressionPatterns', [function () {
        return {
            // 行動電話
            PhoneNumber: {
                pattern: /^(09\d{8})$|^(886\d{9})$/,
                //errorMessage: '不符合行動電話格式。範例：0912345678 或 886912345678。',
                errorMessage: [
                    //'不符合行動電話格式。範例如下',
                    '行動電話格式範例如下，',
                    '<br/>',
                    '<ul>',
                    '   <li>',
                    '       台灣地區：0912345678',
                    '   </li>',
                    '   <li>',
                    '       國際地區：+886912345678',
                    '   </li>',
                    '</ul>'
                ].join('\n'),
            },
            // 市內電話
            LocalCall: {
                pattern: /^\d+$/,
                errorMessage: '不符合市內電話格式。範例：1234567。',
            },
            // 市內電話分機
            LocalCallExt: {
                pattern: /^\d+$/,
                errorMessage: '不符合市內電話分機格式。範例：123。',
            },
            // Email
            Email: {
                pattern: /^[\w-.]+@([\w-]+\.)+[\w-]+$/,
                errorMessage: '不符合電子郵件格式。範例：abc@gmail.com。',
            },
            // 日期格式(e.g. 12/31)
            Date_MMdd: {
                pattern: /^(([1-9])|(0[1-9])|(1[0-2]))\/(([0-9])|([0-2][0-9])|(3[0-1]))$/,
                errorMessage: '不符合日期格式。範例：12/31。',
            },
            // 發送時間格式(e.g. 201512312359)
            SendTime: {
                pattern: /^(19|20)\d{2}(0[1-9]|1[012])(0[1-9]|[12]\d|3[01])([01][0-9]|2[0-3])([0-5][0-9])$/,
                errorMessage: '不符合發送時間格式。範例：201512312359。',
            },
        };
    }]);

})(window, document);

