(function () {
    'use strict';

    var app = angular.module('app');

    // Configure Toastr
    //toastr.options.timeOut = 4000;
    //toastr.options.positionClass = 'toast-bottom-right';

    // For use with the HotTowel-Angular-Breeze add-on that uses Breeze
    var remoteServiceName = 'breeze/Breeze';

    var events = {
        controllerActivateSuccess: 'controller.activateSuccess',
        spinnerToggle: 'spinner.toggle'
    };

    var auth = {
        name: 'auth',
        enums: {
            authorised: {
                authorised: 0,
                loginRequired: 1,
                notAuthorised: 2
            },
            permissionCheckType: {
                atLeastOne: 0,
                combinationRequired: 1
            }
        },
        events: {
            userLoggedIn: 'auth:user:loggedIn',
            userLoggedOut: 'auth:user:loggedOut'
        },
        controllers: {
            login: 'loginCtrl'
        },
        services: {
            authentication: 'authentication',
            authorization: 'authorization'
        },
        routes: {
            login: '/login',
            notAuthorised: '/not-authorized'
        }
    };

    var baseWebAPIURL = "SamAppAPI/";

    var config = {
        appErrorPrefix: '[SA Error] ', //Configure the exceptionHandler decorator
        docTitle: 'SamApp: ',
        events: events,
        remoteServiceName: remoteServiceName,
        version: '20150730003',
        auth: auth,
        baseWebAPIURL: baseWebAPIURL
    };

    app.value('config', config);

    app.config(['$logProvider', function ($logProvider) {
        // turn debugging off/on (no info or warn)
        if ($logProvider.debugEnabled) {
            $logProvider.debugEnabled(true);
        }
    }]);

    //#region Configure the common services via commonConfig
    app.config(['commonConfigProvider', function (cfg) {
        cfg.config.controllerActivateSuccessEvent = config.events.controllerActivateSuccess;
        cfg.config.spinnerToggleEvent = config.events.spinnerToggle;
        cfg.config.auth = config.auth;
        cfg.config.baseWebAPIURL = config.baseWebAPIURL;
        cfg.config.version = config.version;

    }]);
    //#endregion
})();