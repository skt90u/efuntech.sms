(function (window, document) {
    'use strict';

    angular.module('app').factory('WebApi', ['$http', '$log', 'toaster', function ($http, $log, toaster) {

        return function (url, options) {

            var title = '';

            if (angular.isDefined(options)) {
                title = options.title;
            }
            
            this.Get = function GetById(data, successFn, errorFn) {

                var res = null;

                if (angular.isUndefined(data)) {

                    res = $http.get(url);
                }
                else {
                    if (angular.isObject(data)) {
                        var criteria = data;
                        var params = angular.copy(criteria || {});
                        if (angular.isUndefined(params.SearchText)) params.SearchText = '';
                        if (angular.isUndefined(params.PageIndex)) params.PageIndex = 0;
                        if (angular.isUndefined(params.PageSize)) params.PageSize = 10;

                        res = $http.get(url, { params: params });
                    }
                    
                    if (angular.isString(data)) {
                        var id = data;
                        res = $http.get(url + '/' + id);
                    }
                }

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

            this.Post = function Post(model, successFn, errorFn) {
                var res = $http.post(url, model);

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    toaster.pop({
                        type: 'success',
                        title: title || '新增資料',
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
                        title: title || '新增資料',
                        body: message,
                        showCloseButton: true,
                    });
                    (errorFn || angular.noop).apply(null, arguments);
                });

                return res;
            };

            this.Put = function Put(model, successFn, errorFn) {
                var id = model.Id;
                var res = $http.put(url + '/' + id, model);

                res.success(function (data, status, headers, config, statusText) {
                    $log.debug(arguments);
                    toaster.pop({
                        type: 'success',
                        title: title || '更新資料',
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
                        title: title || '更新資料)',
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
                    //res = $http.delete(url + '/' + id);
                    res = $http.delete(url + '?id=' + id); // 必須使用 [FromUri]
                    /*
                    [System.Web.Http.HttpDelete]
                    [Route("api/DepartmentPointManager/DeleteAllotSetting")]
                    public HttpResponseMessage DeleteAllotSetting([FromUri]string id)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    */
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
                        title: title || '刪除資料',
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
                        title: title || '刪除資料',
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