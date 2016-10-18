(function () {
    'use strict';
    var controllerId = 'MainPageList';
    angular.module('app').controller(controllerId, ["$http", "$q", "$window", "$scope", "$filter", 'common', 'dataService', MainPageList]);

    function MainPageList($http, $q, $window, $scope, $filter, common, dataService) {
        //var getLogFn = common.logger.getLogFn;
        //var log = getLogFn(controllerId);

        var vm = this;


        vm.title = 'Programs';
        vm.rowCollection = [];
        vm.defaultSortType = function () {
            vm.sortType = 'Name';
            vm.sortReverse = false;
        };
        vm.defaultSortType();
        vm.searchType = "";

        vm.selected = {};
        vm.showError = false;
        vm.errorMessage = "";
        vm.searchTable = "";
        vm.searchTable2 = vm.searchTable;

        vm.searchClick = function () {
            vm.searchTable = vm.searchTable2;
            document.getElementById("inputSearch").select();
            setChartData();
        };

        vm.itemClicked = function (item, type) {
            vm.searchTable2 = item;
            vm.searchType = type;
            vm.searchClick();
        };

        vm.formClick = function () {
            vm.showError = false;
        };

        vm.loading = false;

        activate();

        function activate() {
            var promises = [getRowCollection()];
            common.activateController(promises, controllerId)
                .then(function () {
                    //log('Activated Main View');
                });
        }

        function setChartData()
        {
            vm.rowCollection2 = vm.rowCollection;
            vm.rowCollection2 = $filter('filter')(vm.rowCollection2, vm.searchTable);

            $scope.openQueriesLabels = [];
            $scope.openQueriesData = [];
            for (var i = 0; i < vm.rowCollection2.length; i++) {
                var b = $scope.openQueriesLabels.indexOf(vm.rowCollection2[i].SiteName);
                if (b == -1) {
                    $scope.openQueriesLabels.push(vm.rowCollection2[i].SiteName);
                    $scope.openQueriesData.push(vm.rowCollection2[i].OpenCount);
                }
                else {
                    $scope.openQueriesData[b] += vm.rowCollection2[i].OpenCount;
                }
            }

            $scope.answeredQueriesLabels = [];
            $scope.answeredQueriesData = [];
            for (var i = 0; i < vm.rowCollection2.length; i++) {
                var b = $scope.answeredQueriesLabels.indexOf(vm.rowCollection2[i].SiteName);
                if (b == -1) {
                    $scope.answeredQueriesLabels.push(vm.rowCollection2[i].SiteName);
                    $scope.answeredQueriesData.push(vm.rowCollection2[i].AnsweredCount);
                }
                else {
                    $scope.answeredQueriesData[b] += vm.rowCollection2[i].AnsweredCount;
                }
            }
        }

        function getRowCollection() {
            vm.loading = true;
            var promiseGet = dataService.getData();
            return promiseGet.then(function (result) {
                vm.rowCollection = result.data;
                setChartData();

                vm.loading = false;
                return $q.when(vm.rowCollection);
            }, function (err) {
                //log('An Error Occurred. See more details at the top of the form.');
            });
        }
    }

})();