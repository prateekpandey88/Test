module.controller("signixCtrl", [
    "$scope", "signixService", function ($scope, signixService) {

        $scope.refresh = function () {
            blockElement();
            signixService.getNotificationUrlsData().then(function (items) {
                $scope.items = items;
                unblockElement();
            });
        };

        $scope.refres();

        function blockElement() {
            $("table").block({ message: '<i class="fa fa-spinner fa-spin"></i>' });
        };

        function unblockElement() {
            $("table").unblock();
        };

    }
]);