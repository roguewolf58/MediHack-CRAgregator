(function () {
    'use strict';

    var serviceId = 'dataService';
    angular.module('app').factory(serviceId, ['$http', 'common', dataService]);

    function dataService($http, common) {
        var $q = common.$q;

        var data = {
            SearchText: ''
        };

        var service = {
            getSearchText: function () { return data.SearchText; },
            setSearchText: function (searchText) { data.SearchText = searchText; },
            getData: getData,
            getTestData: getTestData
        };

        return service;

        // Employee
        function getData() {
            var url = 'API/api/Employee/Employees/';
            var req = $http.get(url);
            return req;

        }

        function getTestData() {
            var testData = { data : [
                { Sponsor: 'Sponsor1', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 1 },
                { Sponsor: 'Sponsor2', Study: 'Study2', Site: 'Site2', OpenQueries: 10, AnsweredQueries: 4 },
                { Sponsor: 'Sponsor2', Study: 'Study3', Site: 'Site3', OpenQueries: 8, AnsweredQueries: 7 },
                { Sponsor: 'Sponsor2', Study: 'Study4', Site: 'Site4', OpenQueries: 3, AnsweredQueries: 8 },
                { Sponsor: 'Sponsor3', Study: 'Study5', Site: 'Site5', OpenQueries: 1, AnsweredQueries: 3 },
                { Sponsor: 'Sponsor3', Study: 'Study6', Site: 'Site6', OpenQueries: 2, AnsweredQueries: 2 },
                { Sponsor: 'Sponsor4', Study: 'Study7', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 5 }
            ]};
            return $q.when(testData);
        }

    }
})();