(function (window, document) {
    'use strict';

    // http://www.angulartutorial.net/2014/05/set-headers-for-all-http-calls-in.html
    angular.module('app').run(['$window', function ($window) {


        /**
         * 20151021 Norman
         *	在瀏覽器中使用預設印表機，不顯示確認視窗，直接列印PDF。
         *  
         * 參數：與 window.open 參數一致
         */
        var silentPrint = function () {
            // https://jsfiddle.net/wvbdnb8o/
            var isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
            var isFirefox = typeof InstallTrigger !== 'undefined';
            var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
            var isChrome = !!window.chrome && !isOpera;
            var isIE = /*@cc_on!@*/false || !!document.documentMode;

            if (isOpera) {
                throw "window.silentPrint 尚未支援瀏覽器: Opera";
            }

            if (isSafari) {
                throw "window.silentPrint 尚未支援瀏覽器: Safari";
            }

            if (isIE) {
                // 參考資料
                // 	http://matthewkwong.blogspot.tw/2013/03/click-button-to-print-embedded-pdf-file.html

                var pdf_path = arguments[0];
                var elem = document.getElementById("pdf");
                if (elem) {
                    elem.parentNode.removeChild(elem);
                }
                document.body.insertAdjacentHTML('beforeEnd', '<object id="pdf" classid="clsid:CA8A9780-280D-11CF-A24D-444553540000" width="0" height="0"><param name="src" value="' + pdf_path + '"/></object>');
                if (pdf)
                    pdf.printAll();
            }

            if (isFirefox || isChrome) {
                // 參考資料
                // 	http://stackoverflow.com/questions/21908/silent-printing-in-a-web-application
                // 	http://peter.sh/experiments/chromium-command-line-switches/

                // 需要針對瀏覽器做額外設定，才能達到 print silently
                // firegox
                //	changing about:config. Add print.always_print_silent and set it to true.
                // chrome:
                //	--user-data-dir="C:/Chrome dev session" --kiosk-printing
                // https://mycartpos.zendesk.com/hc/en-us/articles/200868343-Enable-kiosk-printing-print-automatically-for-Google-Chrome-on-Windows
                // 實際發布時： "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --user-data-dir="C:/Chrome dev session" --kiosk-printing
                // 離線開發時： "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --user-data-dir="C:/Chrome dev session" --disable-web-security --kiosk-printing
                var win = window.open.apply(null, arguments);
                if (win) {
                    win.focus();
                    win.print();
                }
            }
        }

        $window.silentPrint = silentPrint;
        window.silentPrint = silentPrint;
    }]);

})(window, document);

