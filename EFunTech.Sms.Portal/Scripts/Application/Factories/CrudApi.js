(function (window, document) {
    'use strict';

    angular.module('app').factory('CrudApi', ['$http', '$log', 'toaster', '$location', function ($http, $log, toaster, $location) {

        return function (url) {

            this.GetUrl = function () { return url; };

            this.Download = function Download(criteria) {

                // http://localhost:20155/api/TradeDetail?StartDate=Thu+Sep+17+2015+16%3A57%3A47+GMT%2B0800+(%E5%8F%B0%E5%8C%97%E6%A8%99%E6%BA%96%E6%99%82%E9%96%93)&EndDate=Thu+Sep+17+2015+16%3A57%3A47+GMT%2B0800+(%E5%8F%B0%E5%8C%97%E6%A8%99%E6%BA%96%E6%99%82%E9%96%93)&TradeType=0&SearchText=&PageIndex=1&PageSize=10&IsDownload=true
                var params = angular.copy(criteria || {});
                if (angular.isUndefined(params.SearchText)) params.SearchText = '';
                //if (angular.isUndefined(params.PageIndex)) params.PageIndex = 0;
                //if (angular.isUndefined(params.PageSize)) params.PageSize = 10;
                params.IsDownload = true;

                angular.forEach(params, function (value, key) {
                    if (angular.isDate(value)) {
                        params[key] = value.toJSON(); // 20150918 Norman, Date型別需要額外處理
                    }
                });
                
                var pathname = url + '?' + $.param(params);
                var openUrl = window.location.origin + pathname;
                window.open(openUrl, '_self'); // TODO: TimezoneOffset 無法傳遞過去，要如何設定 Request Header ??
            };

            this.GetAll = function GetAll(criteria, successFn, errorFn) {
                var params = angular.copy(criteria || {});
                if (angular.isUndefined(params.SearchText)) params.SearchText = '';
                if (angular.isUndefined(params.PageIndex)) params.PageIndex = 0;
                if (angular.isUndefined(params.PageSize)) params.PageSize = 10;
                params.IsDownload = false;
                // webapi gziip 壓縮功能尚未測試成功
                //params.headers = { 'Accept-Encoding': 'gzip', 'Accept-Charset': undefined };

                var res = $http.get(url, { params: params });

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    (successFn || angular.noop).apply(null, arguments);
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
                        title: '查詢資料(多筆)',
                        body: message,
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };

            this.GetById = function GetById(id, successFn, errorFn) {
                var res = $http.get(url + '/' + id);

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    (successFn || angular.noop).apply(null, arguments);
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
                        title: '查詢資料',
                        body: message,
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };

            this.Create = function Create(model, successFn, errorFn) {
                var res = $http.post(url, model);

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    toaster.pop({
                        type: 'success',
                        title: '新增資料',
                        body: '成功',
                        showCloseButton: true,
                    });
                    (successFn || angular.noop).apply(null, arguments);
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
                        title: '新增資料',
                        body: message,
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };

            this.Update = function Update(model, successFn, errorFn) {
                var id = model.Id;
                var res = $http.put(url + '/' + id, model);

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    toaster.pop({
                        type: 'success',
                        title: '更新資料',
                        body: '成功',
                        showCloseButton: true,
                    });
                    (successFn || angular.noop).apply(null, arguments);
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
                        title: '更新資料)',
                        body: message,
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };

            this.Delete = function Delete(data, successFn, errorFn) {
                var res = null;

                if (!angular.isArray(data)) {
                    var model = data;
                    var id = model.Id;
                    res = $http.delete(url + '/' + id);
                }
                else {
                    var models = data;
                    var ids = _.pluck(models, 'Id');
                    var querystring = _.map(ids, function (id) { return 'ids=' + id; }).join('&');
                    res = $http.delete(url + '?' + querystring);
                }

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    toaster.pop({
                        type: 'success',
                        title: '刪除資料',
                        body: '成功',
                        showCloseButton: true,
                    });
                    (successFn || angular.noop).apply(null, arguments);
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
                        title: '刪除資料',
                        body: message,
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };
        };
    }]);

})(window, document);