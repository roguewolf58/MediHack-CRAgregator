(function () {
    'use strict';
    var controllerId = 'mainPageList';
    angular.module('app').controller(controllerId, ["$http", "$q", "$window", "$state", 'dataService', mainPageList]);

    function ProgramList($http, $q, $window, $state, common, clinicalAdminService) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Programs';
        vm.rowCollection = [];
        vm.defaultSortType = function () {
            vm.sortType = 'Name';
            vm.sortReverse = false;
        };
        vm.defaultSortType();

        vm.selected = {};
        vm.showError = false;
        vm.errorMessage = "";
        vm.formClick = function () {
            vm.showError = false;
        };

        vm.loading = false;

        activate();

        function activate() {
            var promises = [getRowCollection()];
            common.activateController(promises, controllerId)
                .then(function () { log('Activated Main View'); });
        }

        function getRowCollection() {
            vm.loading = true;
            var promiseGet = dataService.getTestData();
            return promiseGet.then(function (result) {
                vm.rowCollection = result.data;
                vm.loading = false;
                return $q.when(vm.rowCollection);
            }, function (err) {
                log('An Error Occurred. See more details at the top of the form.');
            });
        }
    }

})();