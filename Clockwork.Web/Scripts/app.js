var app = angular.module('clockwork', []);

app.controller('mainController', function (menuService, tableService, alertService) {
    var vm = this;
    
    menuService.getZones().then(response => {
        vm.timezones = response.data;
        vm.selectedOption = { id: '3', name: 'Option C' };
    });

    tableService.getValues().then(response => {
        vm.tuples = response.data;
    });

    this.UserActionAlert = function () {
        alertService.getValue().then(response => {
            vm.tuple = response.data;
            vm.tuple.time = vm.tuple.time.replace("Z", "");
            vm.tuple.utcTime = vm.tuple.utcTime.replace("Z", "");
            tableService.getValues().then(response => {
                vm.tuples = response.data;
            });
        });
    }
});

app.service('menuService', function ($http) {
    this.getZones = function () {
        return $http.get('http://localhost:25648/api/currenttime/zoneselector');
    }
});
app.service('alertService', function ($http) {
    this.getValue = function () {
        var timeOffset = document.getElementById("timeZone").value.split(':');
        var timeObj = {
            Hours: timeOffset[0],
            Minutes: timeOffset[1],
            Seconds: timeOffset[2],
            IsDaylightSavings: document.getElementById("daylightsavings").checked
        };
        document.getElementById('insertion').hidden = false;
        return $http.post('http://localhost:25648/api/currenttime', JSON.stringify(timeObj));
    }
});
app.service('tableService', function ($http) {
    this.getValues = function () {
        return $http.get('http://localhost:25648/api/currenttime/selectall');
    }
});