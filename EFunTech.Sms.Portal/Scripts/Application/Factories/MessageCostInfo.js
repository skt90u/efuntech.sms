(function (window, document) {
    "use strict";

    angular.module("app").factory("MessageCostInfo", [function () {

        return function (sendBody, replacements) {

            var message = sendBody;

            var SendBody = "";
            var MessageLength = 0; // 簡訊字數
            var MessageNum = 0; // 簡訊總共幾則
            var MessageCost = 0; // 簡訊花費點數
            var MessageFormatError = "";

            if (angular.isObject(replacements)) {
                _.each(replacements, function (val, key) {
                    message = replaceAllStr(message, key, val);
                });
            }
            
            SendBody = message;
            MessageLength = message.length;

            if (MessageLength != 0) {
                if (hasChinese(message)) {
                    // 中文簡訊計算(長簡訊發送計算)
                    if (MessageLength > 333) {
                        var tempCharge = Math.floor(MessageLength / 333);
                        var remainder = MessageLength - (tempCharge * 333);
                        if (remainder <= 70) {
                            MessageCost = tempCharge * 5 + 1;
                        } else {
                            MessageCost = tempCharge * 5 + Math.ceil(remainder / 67);
                        }
                    } else if (MessageLength <= 70) {
                        MessageCost = 1;
                    } else {
                        MessageCost = Math.ceil(MessageLength / 67);
                    }
                    //計算長簡訊的發送則數
                    if (MessageLength > 333) {
                        if (MessageLength <= 656) {
                            MessageNum = 2;
                        } else {
                            var length = MessageLength - 656;
                            MessageNum = 2 + Math.ceil(length / 328);
                        }
                    } else {
                        MessageNum = 1;
                    }
                }
                else {
                    // 英文簡訊計算
                    if (!isPureEnglishCheck(message)) {
                        MessageFormatError = "純英文發送內容時不可有`^";
                    }

                    if (MessageLength > 765) {
                        var tempCharge = Math.floor(MessageLength / 765);
                        var remainder = MessageLength - (tempCharge * 765);
                        if (remainder <= 160) {
                            MessageCost = tempCharge * 5 + 1;
                        } else {
                            MessageCost = tempCharge * 5 + Math.ceil(remainder / 153);
                        }
                    } else if (MessageLength <= 160) {
                        MessageCost = 1;
                    } else {
                        MessageCost = Math.ceil(MessageLength / 153);
                    }
                    //計算長簡訊的發送則數
                    if (MessageLength > 765) {
                        if (MessageLength <= 1520) {
                            MessageNum = 2;
                        } else {
                            var length = MessageLength - 1520;
                            MessageNum = 2 + Math.ceil(length / 760);
                        }
                    } else {
                        MessageNum = 1;
                    }
                }
            }

            this.SendBody = SendBody;
            this.MessageLength = MessageLength; // 簡訊字數
            this.MessageNum = MessageNum; // 簡訊總共幾則
            this.MessageCost = MessageCost; // 簡訊花費點數
            this.MessageFormatError = MessageFormatError;
        };

        function replaceAllStr(str, oldValue, newValue) {
            // http://www.dotblogs.com.tw/topcat/archive/2008/07/07/4449.aspx
            // http://stackoverflow.com/questions/874709/converting-user-input-string-to-regular-expression
            var re = new RegExp(oldValue, "g");
            var str = str.replace(re, newValue);
            return str;
        }

        // 檢查是否有中文
        function hasChinese(str) {
            return -1 != _.findIndex(str, function (ch, i) {
                return str.charCodeAt(i) > 127;
            });
        }

        // 檢查發送內容是否有無法輸入字元
        function isPureEnglishCheck(str) {
            /*
            含全形字元時
            這是發送測試~`^{}[]|\\
        
            純英文時候
            ~=-
            `=@
            ^= 
            {=(
            }=)
            [=<
            ]=>
            |=/
            /=/
            */
            var illegals = ["`", "^"];

            return -1 == _.findIndex(str, function (ch, i) {
                return _.contains(illegals, ch);
            });
        }

    }]);

})(window, document);