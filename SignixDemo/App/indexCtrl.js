module.controller("signixCtrl", [
    "$scope", "signixService", function ($scope, signixService) {

        $scope.showSignixWizard = false;

        $scope.signer1 = { firstName: "", middleName: "", lastName: "", email: "" };

        $scope.signer2 = { firstName: "", middleName: "", lastName: "", email: "" };

        var blockElement = function () {
            $("#documentDetails").block({ message: '<i class="fa fa-spinner fa-spin"></i>' });
        };

        var unblockElement = function () {
            $("#documentDetails").unblock();
        };

        var isAllFieldsValid = function () {

            if ($scope.signer1.firstName == undefined || $scope.signer1.firstName == "" ||
                $scope.signer1.lastName == undefined || $scope.signer1.lastName == "" ||
                $scope.signer1.email == undefined || $scope.signer1.email == "") {
                alert("Please fill all Signer1 details.");
                return false;
            }

            if ($scope.contractDocument == undefined || $scope.contractDocument.name == undefined || $scope.contractDocument.name == "") {
                alert("Please upload document.");
                return false;
            }
            return true;
        };

        $scope.submitDocument = function () {

            //if (!isAllFieldsValid())
            //    return;

            blockElement();

            var file = $scope.contractDocument;

            signixService.uploadContract(file).then(function (fileName) {
                var data = {
                    signers: [$scope.signer1, $scope.signer2],
                    fileName: fileName,
                    emailContent: $scope.emailContent,
                    docSetDescription: $scope.docSetDescription,
                    submitterEmail: $scope.submitterEmail,
                    contactInfo: $scope.contactInfo,
                    distributionEmailList: $scope.distributionEmailList,
                    distributionEmailContent: $scope.distributionEmailContent,
                    originalFileName: $scope.contractDocument.name
                };
                signixService.submitDocument(data).then(function (documentSetId) {
                    $scope.getAccessLink(documentSetId);
                    unblockElement();
                });
            });
        };

        $scope.getAccessLink = function (documentSetId) {

            if (documentSetId == undefined || documentSetId == "") {
                alert("Error while connecting to Signix");
                return;
            }

            signixService.getAccessLink(documentSetId).then(function (url) {

                if (url == undefined || url == "") {
                    alert("Error while connecting to Signix");
                    return;
                }

                $scope.openSignixWizard(url);

                $scope.showSignixWizard = true;

                //loadSIGNiX(url, //The access link obtained by the server.
                //            function (url) //Called when the SIGNiX object is loaded.
                //            {
                //                // Code to complete initialization of your page.
                //                //$scope.openSignixWizard(url);
                //                SIGNiX.openEmbeddingApi(data.token);

                //                SIGNiX.openUI("#signixWizardIframe", url, function signixUIObserver(act, param) {
                //                    console.log(act);
                //                    console.log(param);
                //                }, true);
                //            },
                //            function (error, url) //Called if the SIGNiX object fails to load.
                //            {
                //                // Code to handle error.

                //            });

            });
        };

        $scope.openSignixWizard = function (url) {
            $scope.showSignixWizard = true;
            angular.element("#signixWizardIframe").attr("src", url);
        };

        $scope.closeSignixWizard = function () {
            $scope.showSignixWizard = false;
            angular.element("#signixWizardIframe").attr("src", "");
            refreshFields();
        };

        function refreshFields() {
            $scope.signer1 = { firstName: "", middleName: "", lastName: "", email: "" };
            $scope.signer2 = { firstName: "", middleName: "", lastName: "", email: "" };
            $scope.contractDocument = "";
            $scope.emailContent = "";
            $scope.docSetDescription = "";
            $scope.submitterEmail = "";
            $scope.contactInfo = "";
            $scope.distributionEmailList = "";
            $scope.distributionEmailContent = "";
            angular.element("#documentFileId").val("");
        };

        // Function provided by SIGNiX for loading support code from the appropriate site.
        //function loadSIGNiX(url, successCallback, failCallback, passthrough) {
        //    SIGNiX = null; //Remove any previous instance.
        //    var end = url.indexOf("/", 8); if (end == -1) end = url.length;
        //    $.getScript(url.slice(0, end) + "/ui/common/js/embedded-client.js")
        //    .done(function () {
        //        // In case the current jQuery getScript doc is right that the script
        //        // is only loaded and not necessarily executed.
        //        var i = 0,
        //            timer = setInterval(
        //                function () {
        //                    if (typeof SIGNiX != "undefined") {
        //                        clearInterval(timer);
        //                        successCallback(url, passthrough);
        //                    } else if (i++ > 50) {
        //                        clearInterval(timer);
        //                        failCallback(new Error("Can't access SIGNiX."), url, passthrough);
        //                    }
        //                },
        //                200);
        //    })
        //    .fail(function (x, y, error) { failCallback(error, url, passthrough) });
        //}
    }
]);