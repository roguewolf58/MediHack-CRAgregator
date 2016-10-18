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
                { Sponsor: 'Sponsor1', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 },
                { Sponsor: 'Sponsor2', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 },
                { Sponsor: 'Sponsor2', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 },
                { Sponsor: 'Sponsor2', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 },
                { Sponsor: 'Sponsor3', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 },
                { Sponsor: 'Sponsor3', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 },
                { Sponsor: 'Sponsor4', Study: 'Study1', Site: 'Site1', OpenQueries: 5, AnsweredQueries: 6 }
            ]};
            return $q.when(testData);
        }

    }
})();