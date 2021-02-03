var DashboardCampaign = angular.module("DashboardCampaign", []);

DashboardCampaign.config(
    function ($routeProvider, $locationProvider) {

        $routeProvider
        .when('/campaign/:campaign/step/:step', {
            controller: "Base"
        }).otherwise({
            controller: "Base"
        });


    });

DashboardCampaign.controller("Base"
    , function ($q, $scope, $http, $route, $routeParams, $location, $timeout) {

        /* Start map setup */
        nokia.Settings.set("app_id", $("#hereMapsAppID").val());
        nokia.Settings.set("app_code", $("#hereMapsAppCode").val());
        $scope.mapContainer = document.getElementById("hereMap");
        $scope.map = new nokia.maps.map.Display($scope.mapContainer, {
            // initial center and zoom level of the map
            center: [52.009077, 4.358321],
            zoomLevel: 12,
            components: [
                // ZoomBar provides a UI to zoom the map in & out
                new nokia.maps.map.component.ZoomBar(),
                // We add the behavior component to allow panning / zooming of the map
                new nokia.maps.map.component.Behavior(),
                // ZoomRectangle component adds zoom rectangle functionality to the map
                new nokia.maps.map.component.ZoomRectangle(),
                new nokia.maps.positioning.component.Positioning()
            ]
        });
        $scope.geoFenceContainer = new nokia.maps.map.Container();
        $scope.deviceContainer = new nokia.maps.map.Container();
        /* End map Setup */
        $scope.step = 0; // step 1= Select campaign, step 2=Compose message, step 3= SElect recipients, step 4=preview, step 5 = result
        $scope.currentWimUser = parseInt($('.wimUserId').val(), 10);
        $scope.heartBeat = null;
        $scope.currentCampaign;
        $scope.charsleft = 200;
        $scope.campaignTitle = "";
        $scope.campaignPushMessage = "";
        $scope.campaignContent = "";

        $scope.changeState = function (step) {
            if ($scope.currentCampaign.Key == 0) {
                $location.path("");
            }
            else {
                $location.path("campaign/" + $scope.currentCampaign.Key + "/step/" + step);
            }
        }

        // Init campaign collection
        var postData = new Object();
        postData.currentWimUser = $scope.currentWimUser;



        $scope.campaignChanged = function () {
            console.log($scope.currentCampaign);
            if ($scope.currentCampaign.Key > 0) {
                if ($scope.currentCampaign.CanSend)
                    $scope.changeState(2);
                else
                    $scope.changeState(5);
            }
            else
                $scope.changeState(1);

        };
        $scope.ReloadFromCampagneAdd = function (campaignID) {
            $location.path("campaign/" + campaignID + "/step/" + 2);
        }

        $scope.$on(
               "$routeChangeSuccess",
               function ($currentRoute, $previousRoute) {

                   $timeout.cancel($scope.heartBeat);
                   $http.post('services/DashBoardService.svc/GetCampaigns', postData).success(function (data) {

                       var set = [{ Value: '', Key: 0 }];
                       set = set.concat(data.d.Campaigns);
                       $scope.campaigns = set;
                       if ($scope.currentCampaign == null) {
                           $scope.step = 1;
                           $scope.currentCampaign = $scope.campaigns[0];
                       }

                       if ($routeParams.step == null)
                           $scope.step = 1;
                       else
                           $scope.step = $routeParams.step;

                       for (var i in $scope.campaigns) {
                           var item = $scope.campaigns[i];
                           if (item.Key == $routeParams.campaign)
                               $scope.currentCampaign = item;
                       }
                       if ($scope.step == 1) {
                           $scope.geoFenceContainer.destroy();
                           $scope.deviceContainer.destroy();

                       }
                       if ($scope.step == 2) {
                           $scope.geoFenceContainer.destroy();
                           $scope.deviceContainer.destroy();
                           var postData = new Object();
                           postData.currentWimUser = $scope.currentWimUser;
                           postData.currentCampaignID = $scope.currentCampaign.Key;
                           $http.post('services/DashBoardService.svc/GetCampaign', postData).success(function (data) {
                               $scope.campaign = data.d;
                               $scope.campaignTitle = data.d.Title;
                               $scope.campaignPushMessage = data.d.PushMessage;
                               $scope.campaignContent = data.d.Content;

                               $scope.titleNotfilledError = false;
                               $scope.pushMessageNotfilledError = false;

                               if (!isEmpty($scope.campaign.Title) && !isEmpty($scope.campaign.PushMessage)) {
                                   $scope.step1Class = 'checkmark-1';
                               }
                               else {
                                   $scope.step1Class = '';
                               }
                               if ($scope.campaign.SelectedFences != null && $scope.campaign.SelectedFences.length > 0)
                                   $scope.step2Class = 'checkmark-1';
                               else
                                   $scope.step2Class = '';
                           });
                       }
                       if ($scope.step >= 3) {
                           $scope.geoFenceContainer.destroy();
                           $scope.deviceContainer.destroy();
                           var postData = new Object();
                           postData.currentWimUser = $scope.currentWimUser;
                           postData.currentCampaignID = $scope.currentCampaign.Key;
                           $http.post('services/DashBoardService.svc/GetCampaign', postData).success(function (data) {
                               $scope.campaign = data.d;
                               $scope.geoFenceSelected = data.d.GeoFences[0];
                               $scope.interestSelected = data.d.Interest[0];
                               $scope.deviceTypeSelected = data.d.DeviceTypes[0];
                               $scope.identitySelected = data.d.IdentityProviders[0];

                               $scope.geoFencesNotfilledError = false;
                               // Show population
                               $scope.DrawPopulation(data.d.DevicePopulation);
                               // Create geofences
                               $scope.RedrawAllFences();
                               if ($scope.campaign != null) {
                                   if (!isEmpty($scope.campaign.Title) && !isEmpty($scope.campaign.PushMessage)) {
                                       $scope.step1Class = 'checkmark-1';
                                   }

                                   if ($scope.campaign.SelectedFences != null && $scope.campaign.SelectedFences.length > 0) {
                                       $scope.step2Class = 'checkmark-1';
                                   }
                                   else
                                       $scope.step2Class = '';
                               }

                           });
                       }


                       if ($scope.step > 1 && $scope.step < 5) {
                           $scope.populationDisplay(true);
                       }
                   });
               });

        $scope.populationDisplay = function (doHeartbeat) {

            if ($scope.campaign == null || $scope.step == 1 || $scope.step == 5) {
                $scope.heartBeat = $timeout(function () { $scope.populationDisplay(true) }, 5000);
                return;
            }

            var postData = new Object();
            postData.currentWimUser = $scope.currentWimUser;
            postData.currentCampaignID = $scope.currentCampaign.Key;
            postData.selectedFences = $scope.campaign.SelectedFences;
            postData.selectedInteresses = $scope.campaign.SelectedInteresses;
            postData.selectedOS = $scope.campaign.SelectedDeviceTypes;
            postData.selectIdentityProviders = $scope.campaign.SelectIdentityProviders;
            postData.maxSpeed = $scope.campaign.MaximumSpeed;
            postData.maxAge = $scope.campaign.MaximumAgePosition;

            $http.post('services/DashBoardService.svc/GetPopulation', postData).success(function (data) {
                if (data != null && data.d != null) {
                    if ($scope.campaign.GeoFences.length != data.d.GeoFences.length) {
                        $scope.campaign.GeoFences = data.d.GeoFences;
                        $scope.geoFenceSelected = data.d.GeoFences[0];

                    }
                    if ($scope.campaign.Interest.length != data.d.Interest.length) {
                        $scope.campaign.Interest = data.d.Interest;
                        $scope.interestSelected = data.d.Interest[0];
                    }

                    $scope.DrawPopulation(data.d.DevicePopulation);
                }
                if (doHeartbeat)
                    $scope.heartBeat = $timeout(function () { $scope.populationDisplay(true) }, 5000);
            });

        }
        $scope.drawAGeoFence = function (geoFence) {
            var coords = [];
            for (var i in geoFence.Coordinates) {
                var item = geoFence.Coordinates[i];
                coords.push(new nokia.maps.geo.Coordinate(item.Longitude, item.Latitude));
            }
            var markerPolygon = new nokia.maps.map.Polygon(coords,
	            {
	                pen: { strokeColor: "#00F8", lineWidth: 1 }
	            }
            );

            markerPolygon.geoFenceID = geoFence.Key;
            $scope.geoFenceContainer.objects.add(markerPolygon);
            $scope.map.objects.add($scope.geoFenceContainer);

        }
        $scope.RedrawAllFences = function () {
            // reCreate geofences
            $scope.geoFenceContainer.destroy();
            $scope.geoFenceContainer = new nokia.maps.map.Container();
            if ($scope.campaign.SelectedFences != null && $scope.campaign.SelectedFences.length > 0) {
                for (var i in $scope.campaign.SelectedFences) {
                    for (var j in $scope.campaign.GeoFences) {
                        var selectedFence = $scope.campaign.SelectedFences[i];
                        var aGeoFence = $scope.campaign.GeoFences[j];
                        if (selectedFence.Key == aGeoFence.Key) {
                            $scope.drawAGeoFence(aGeoFence);
                        }
                    }
                }
                $scope.map.zoomTo($scope.geoFenceContainer.getBoundingBox(), false, "default");
            }
        }
        $scope.addToGeoFenceCollection = function (selected, collection) {
            $scope.addToCollection(selected, collection);
            $scope.drawAGeoFence(selected);
            $scope.map.zoomTo($scope.geoFenceContainer.getBoundingBox(), false, "default");
        }
        $scope.removeFromGeoFenceCollection = function (selected, collection) {
            $scope.removeFromCollection(selected, collection);
            $scope.RedrawAllFences();
        }

        $scope.addToCollection = function (selected, collection) {
            if (collection == null) return;
            for (var i in collection) {
                var item = collection[i];
                if (item.Key == selected.Key)
                    return;
            }
            collection.push(selected);
            $scope.geoFencesNotfilledError = false;
            $scope.populationDisplay(false);
        }
        $scope.removeFromCollection = function (selected, collection) {
            if (collection == null) return;
            for (var i in collection) {
                var item = collection[i];
                if (item.Key == selected)
                    collection = collection.splice(i, 1);
            }
            $scope.populationDisplay(false);
        }
        $scope.RedrawPopulation = function () {
            var postData = new Object();
            postData.currentWimUser = $scope.currentWimUser;
            postData.currentCampaignID = $scope.currentCampaign.Key;
            postData.selectedFences = $scope.campaign.SelectedFences;
            postData.selectedInteresses = $scope.campaign.SelectedInteresses;
            postData.selectedOS = $scope.campaign.SelectedDeviceTypes;
            postData.selectIdentityProviders = $scope.campaign.SelectIdentityProviders;
            postData.maxSpeed = $scope.campaign.MaximumSpeed;
            postData.maxAge = $scope.campaign.MaximumAgePosition;

            $http.post('services/DashBoardService.svc/SaveRecipientData', postData).success(function (data) {

                var postData = new Object();
                postData.currentWimUser = $scope.currentWimUser;
                postData.currentCampaignID = $scope.currentCampaign.Key;
                $http.post('services/DashBoardService.svc/GetCampaign', postData).success(function (data) {
                    $scope.campaign = data.d;
                    $scope.geoFenceSelected = data.d.GeoFences[0];
                    $scope.interestSelected = data.d.Interest[0];
                    $scope.deviceTypeSelected = data.d.DeviceTypes[0];
                    $scope.identitySelected = data.d.IdentityProviders[0];
                    $scope.DrawPopulation(data.d.DevicePopulation);
                });
            });

        }
        $scope.DrawPopulation = function (devicePopulation) {
            // reCreate deviceContainer
            $scope.deviceContainer.destroy();
            $scope.deviceContainer = new nokia.maps.map.Container();
            for (var i in devicePopulation) {
                var item = devicePopulation[i];
                var coord = { latitude: item.Latitude, longitude: item.Longitude }
                marker = new nokia.maps.map.Marker(coord, {
                    icon: item.Icon,
                    anchor: new nokia.maps.util.Point(10, 10)
                });
                $scope.deviceContainer.objects.add(marker);

            }

            $scope.map.objects.add($scope.deviceContainer);
        }
        $scope.degradeChars = function ($event) {
            if ($scope.campaignContent != null) {
                $scope.charsleft = 200 - $scope.campaignContent.length;
                if ($scope.charsleft <= 0)
                    event.preventDefault();
            }
        };

        $scope.saveStep2 = function () {
            var error = 0;
            if (isEmpty($scope.campaignTitle)) {
                error++;
                $scope.titleNotfilledError = true;
            }
            else
                $scope.titleNotfilledError = false;
            if (isEmpty($scope.campaignPushMessage)) {
                error++;
                $scope.pushMessageNotfilledError = true;
            }
            else
                $scope.pushMessageNotfilledError = false;

            if (error == 0) {
                var postData = new Object();
                postData.currentWimUser = $scope.currentWimUser;
                postData.currentCampaignID = $scope.currentCampaign.Key;
                postData.title = $scope.campaignTitle;
                postData.pushMessage = $scope.campaignPushMessage;
                postData.content = $scope.campaignContent;

                $http.post('services/DashBoardService.svc/SaveMessageData', postData).success(function (data) {
                    $scope.changeState(3);
                });


            }
        }
        $scope.saveStep3 = function () {
            var error = 0;
            if ($scope.campaign.SelectedFences == null || $scope.campaign.SelectedFences.length < 1) {
                error++;
                $scope.geoFencesNotfilledError = true;
            }
            else
                $scope.geoFencesNotfilledError = false;


            if (error == 0) {
                var postData = new Object();
                postData.currentWimUser = $scope.currentWimUser;
                postData.currentCampaignID = $scope.currentCampaign.Key;
                postData.selectedFences = $scope.campaign.SelectedFences;
                postData.selectedInteresses = $scope.campaign.SelectedInteresses;
                postData.selectedOS = $scope.campaign.SelectedDeviceTypes;
                postData.selectIdentityProviders = $scope.campaign.SelectIdentityProviders;
                postData.maxSpeed = $scope.campaign.MaximumSpeed;
                postData.maxAge = $scope.campaign.MaximumAgePosition;

                $http.post('services/DashBoardService.svc/SaveRecipientData', postData).success(function (data) {
                    $scope.changeState(4);
                });
            }
        }
        $scope.saveStep4 = function () {
            if (!confirm("Are you sure you want to send the campaign message to " + $scope.campaign.TotalReach + " persons?")) return;

            var error = 0;
            var startDate = null;
            var endDate = null;
            if (isEmpty($scope.campaign.ExcutionType)) {
                error++;
                $scope.excutionTypeNotfilledError = true;
            }
            else {
                $scope.excutionTypeNotfilledError = false;
                if ($scope.campaign.ExcutionType == "scheduled") {
                    startDate = createDate($scope.campaign.StartDate, $scope.campaign.StartTime);

                }
                if ($scope.campaign.ExcutionType == "timerange") {
                    startDate = createDate($scope.campaign.StartDate, $scope.campaign.StartTime);
                    endDate = createDate($scope.campaign.EndDate, $scope.campaign.EndTime);
                }

            }

            if (error == 0) {
                var postData = new Object();
                postData.currentWimUser = $scope.currentWimUser;
                postData.currentCampaignID = $scope.currentCampaign.Key;
                postData.executeType = $scope.campaign.ExcutionType;
                postData.startDate = dateToWcf(startDate);
                postData.endDate = dateToWcf(endDate);

                $http.post('services/DashBoardService.svc/SendCampaign', postData).success(function (data) {
                    if (data.d == true) {
                        $scope.changeState(5);
                        $scope.ErrorProcessing = false;
                    }
                    else
                        $scope.ErrorProcessing = true;

                });
            }
        }
    });

// global functions
function isEmpty(str) {
    return str == null || str.trim() == '';
}

function createDate(date, time) {
    var dparts = date.split('-');
    var tparts = time.split(':');
    var m = parseInt(dparts[1], 10) - 1;

    var a = new Date(parseInt(dparts[2], 10), m, parseInt(dparts[0], 10), parseInt(tparts[0], 10), parseInt(tparts[1], 10), 0, 0);
    console.log(a);
    return a;
}

function dateToWcf(input) {
    var d = new Date(input);
    if (isNaN(d)) return null;
    return '\/Date(' + d.getTime() + '-0000)\/';
}