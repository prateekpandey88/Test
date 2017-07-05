module.controller("signixCtrl", [
    "$scope", "signixService", function ($scope, signixService) {

        signixService.getStatusData().then(function (items) {
            $scope.items = items;
        });

        $scope.refresh = function () {
            blockElement();
            signixService.getStatusData().then(function (items) {
                $scope.items = items;
                unblockElement();
            });
        };

        function blockElement() {
            $("table").block({ message: '<i class="fa fa-spinner fa-spin"></i>' });
        };

        function unblockElement() {
            $("table").unblock();
        };

    }
]);