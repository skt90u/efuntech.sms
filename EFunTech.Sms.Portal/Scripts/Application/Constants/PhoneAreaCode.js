(function (window, document) {
    'use strict';

    /**
     * 室內電話區碼
     */
    var PhoneAreaCode = {
        臺北: {value: '02',  text: '臺北'},
        桃園: {value: '03',  text: '桃園, 新竹, 花蓮, 宜蘭'},
        苗栗: {value: '037',  text: '苗栗'},
        臺中: {value: '04',  text: '臺中, 彰化'},
        南投: {value: '049',  text: '南投'},
        嘉義: {value: '05',  text: '嘉義, 雲林'},
        臺南: {value: '06',  text: '臺南, 澎湖'},
        高雄: {value: '07',  text: '高雄'},
        屏東: {value: '08',  text: '屏東'},
        金門: {value: '082',  text: '金門'},
        臺東: {value: '089',  text: '臺東'},
        烏坵: {value: '0826',  text: '烏坵'},
        馬祖: {value: '0836',  text: '馬祖'},
    };
    angular.module('app').constant('PhoneAreaCode', PhoneAreaCode);

})(window, document);
