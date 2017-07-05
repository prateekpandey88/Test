module.factory("signixService", [
    "$http", "$q", function ($http, $q) {

        var baseUrl = "/api/Signix/";

        var submitDocumentPath = baseUrl + "SubmitDocument";
        var getAccessLinkPath = baseUrl + "GetAccessLink";
        var uploadContractPath = baseUrl + "UploadContract";
        var getStatusDataPath = baseUrl + "GetStatusData";
        var getNotificationUrlsDataPath = baseUrl + "GetNotificationUrlsData";

        return {
            submitDocument: function (data) {

                var deferred = $q.defer();

                $http.post(submitDocumentPath, data).then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error);
                });
                return deferred.promise;
            },
            getAccessLink: function (documentSetId) {

                var deferred = $q.defer();

                $http.get(getAccessLinkPath + "?documentSetId=" + documentSetId).then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error);
                });
                return deferred.promise;
            },
            uploadContract: function (file) {
                var fd = new FormData();
                fd.append("file", file);

                var deferred = $q.defer();

                $http.post(uploadContractPath, fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error);
                });
                return deferred.promise;
            },
            getStatusData: function () {

                var deferred = $q.defer();

                $http.get(getStatusDataPath).then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error);
                });
                return deferred.promise;
            },
            getNotificationUrlsData: function () {

                var deferred = $q.defer();

                $http.get(getNotificationUrlsDataPath).then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error);
                });
                return deferred.promise;
            }
        }
    }
]);