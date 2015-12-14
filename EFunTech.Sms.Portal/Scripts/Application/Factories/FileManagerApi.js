(function (window, document) {
    'use strict';

    angular.module('app').factory('FileManagerApi', ['$http', '$log', 'toaster', function ($http, $log, toaster) {

        /**
         * 檢查準備要上傳的檔案
         */
        function checkFile($file, extensionPattern) {
            var files = $file.get(0).files;
            if (files.length == 0) {
                toaster.pop({
                    type: 'error',
                    title: '上傳檔案',
                    body: '未選擇任何檔案！',
                    showCloseButton: true,
                });
                return false;
            }

            // check file extension
            var file = files[0];
            var filename = file.name;
            var ext = (/[.]/.exec(filename)) ? /[^.]+$/.exec(filename) : undefined;
            if (!ext) {
                toaster.pop({
                    type: 'error',
                    title: '上傳檔案',
                    body: '此檔案無附檔名，無法判定檔案格式！',
                    showCloseButton: true,
                });
                return false;
            }

            if (!(ext && extensionPattern.test(ext))) {
                toaster.pop({
                    type: 'error',
                    title: '上傳檔案',
                    body: '上傳檔案格式不正確，建議使用範例檔案！',
                    showCloseButton: true,
                });
                return false;
            }
            return true;
        }

        function uploadFile(url, $file, extraParameters) {
            var files = $file.get(0).files;
            var file = files[0];

            var formData = new FormData();
            formData.append('attachment', file);
            // 增加額外參數
            angular.forEach(extraParameters || {}, function (value, key) {
                formData.append(key, value);
            });

            var res = $http({
                url: url,
                method: "POST",
                data: formData,
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            });

            res.success(function (data, status, headers, config, statusText) {
                toaster.pop({
                    type: 'success',
                    title: '上傳檔案',
                    body: data.Message,
                    showCloseButton: true,
                });
            });
            
            res.error(function (data, status, headers, config, statusText) {
                $log.error(arguments);
                var message = '失敗';
                var ExceptionMessage = data.ExceptionMessage || data || void 0;
                if (ExceptionMessage) {
                    message = '失敗( ' + ExceptionMessage + ' )';
                }
                toaster.pop({
                    type: 'error',
                    title: '上傳檔案',
                    body: message,
                    showCloseButton: true,
                });
            });

            return res;
        }

        return {
            checkFile: checkFile,
            uploadFile: uploadFile,
        };
    }]);

})(window, document);