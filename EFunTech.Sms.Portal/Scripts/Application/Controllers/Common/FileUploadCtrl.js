(function (window, document) {
    'use strict';

    //========================================
    // FileUploadCtrl
    //  共用檔案上傳 Controller
    //
    // @param url: 上傳伺服器位址，例如：'FileManagerApi/UploadBlacklist'
    // @param extensionPattern: 可支援上傳的檔案類型，例如：/^(xlsx|csv|zip)$/i
    // @param extraParameters: 要上傳到伺服器的參數(排除上傳檔案參數 attachment)
    //========================================
    angular.module('app').
        controller('FileUploadCtrl', ['$scope', '$modalInstance', 'FileManagerApi', 'url', 'extensionPattern', 'extraParameters',
            function ($scope, $modalInstance, FileManagerApi, url, extensionPattern, extraParameters) {

                $scope.uploading = false;

                //$scope.url = url;
                //$scope.extensionPattern = extensionPattern;
                //$scope.extraParameters = angular.copy(extraParameters || {});

                $scope.cancel = function () {
                    $modalInstance.dismiss('cancel');
                };

                $scope.selectFile = function () {
                    $('input[type=file]').click();
                };

                $scope.uploadFile = function () {
                    //var $scope = angular.element($file).scope();
                    // 由於這個function的呼叫是來自 angular.element(this).scope().fileNameChanged()，必須要使用 $scope.$apply 才能達到 $scope two way binding
                    $scope.$apply(function () {
                        var $file = $('input[type=file]');

                        if (!FileManagerApi.checkFile($file, extensionPattern)) {
                            $file.val(""); // 清空 input[type=file]，避免對同一個檔案上傳無法觸發onchange事件
                            return false;
                        }

                        $scope.uploading = true;
                        FileManagerApi.uploadFile(url, $file, extraParameters)
                        .then(function () {
                            $modalInstance.close({
                                extraParameters: $scope.extraParameters,
                                uploadedFilename: file.name,
                            });
                        })
                        .finally(function () {
                            $scope.uploading = false;
                            $file.val(""); // 清空 input[type=file]，避免對同一個檔案上傳無法觸發onchange事件
                        });
                    });
                };

            }]);

})(window, document);