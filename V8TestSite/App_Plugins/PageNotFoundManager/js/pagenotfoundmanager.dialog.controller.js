(function() {
    "use strict";
    function PageNotFoundManagerDialog($scope, pageNotFoundManagerResource, navigationService, userService, entityResource, iconHelper) {
        var node = $scope.currentNode;
        var vm = this;
        var initUserDetails;
        var initNotFound;
        $scope.nav = navigationService;
        $scope.dialogTreeApi = {};
        $scope.treeModel = {
            hideHeader: false
        };
        $scope.dialogTreeEventHandler = $({});

        $scope.searchInfo = {
            searchFromId: null,
            searchFromName: null,
            showSearch: false,
            results: [],
            selectedSearchResults: []
        };

        function init() {
            initUserDetails = false;
            initNotFound = false;
            userService.getCurrentUser().then((userData) => {
                $scope.treeModel.hideHeader =
                    userData.startContentIds.length > 0 && userData.startContentIds.indexOf(-1) === -1;
                initUserDetails = true;
                vm.busy = !(initNotFound && initUserDetails);
            });

            pageNotFoundManagerResource.getNotFoundPage(node.id).then((resp) => {
                const val = parseInt(resp.data);
                if (!isNaN(val) && angular.isNumber(val) && val > 0) {
                    $scope.pageNotFoundId = val;
                    entityResource.getById(val, "Document").then(function(item) {
                        item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                        $scope.pageNotFoundNode = item;
                    });
                }
                vm.loaded = true;
                initNotFound = true;
                vm.busy = !(initNotFound && initUserDetails);
            });
        }


        function nodeSelectHandler(args) {
            if (args && args.event) {
                args.event.preventDefault();
                args.event.stopPropagation();
            }

            if ($scope.target) {
                //un-select if there's a current one selected
                $scope.target.selected = false;
            }

            $scope.target = args.node;
            $scope.target.selected = true;
        }


        function nodeExpandedHandler(args) {
            // open mini list view for list views
            if (args.node.metaData.isContainer) {
                openMiniListView(args.node);
            }
        }

        $scope.hideSearch = function() {
            $scope.searchInfo.showSearch = false;
            $scope.searchInfo.searchFromId = null;
            $scope.searchInfo.searchFromName = null;
            $scope.searchInfo.results = [];
        };

        // method to select a search result
        $scope.selectResult = function(evt, result) {
            result.selected = result.selected === true ? false : true;
            nodeSelectHandler(evt, { event: evt, node: result });
        };

        //callback when there are search results
        $scope.onSearchResults = function(results) {
            $scope.searchInfo.results = results;
            $scope.searchInfo.showSearch = true;
        };

        $scope.setNotFoundPage = function() {

            vm.busy = true;
            $scope.error = false;

            var parentId = 0;
            if (node !== null)
                parentId = node.id;

            var notFoundPageId = 0;
            if ($scope.target !== null)
                notFoundPageId = $scope.target.id;

            pageNotFoundManagerResource.setNotFoundPage(parentId, notFoundPageId).then(function() {
                $scope.error = false;
                $scope.success = true;
                vm.busy = false;
            }, function(err) {
                $scope.success = false;
                $scope.error = err;
                vm.busy = false;
            });
        };

        $scope.clear = () => $scope.pageNotFoundNode = null;

        function treeLoadedHandler() {
            if ($scope.source && $scope.source.path) {
                $scope.dialogTreeApi.syncTree({ path: $scope.source.path, activate: false });
            }
        }

        $scope.onTreeInit = function() {
            $scope.dialogTreeApi.callbacks.treeLoaded(treeLoadedHandler);
            $scope.dialogTreeApi.callbacks.treeNodeSelect(nodeSelectHandler);
            $scope.dialogTreeApi.callbacks.treeNodeExpanded(nodeExpandedHandler);
        };

        init();
    }

    angular.module("umbraco").controller("PageNotFoundManager.Dialog.Controller", PageNotFoundManagerDialog);
})();