var extractApp = angular.module('ExtractApp', []);

extractApp.controller('ExtractController', ['$scope', '$http', function ExtractController($scope, $http) {

	$scope.database = '';

	$scope.exportCsv = function () {
		$http.get('api/export', { params: { 'database': $scope.database , 'type' : 'csv'} })
		.then(
		function (success) {
			console.log(success);
			window.open(success.data, '_blank', '');
		})
	};

	$scope.exportXml = function () {
		$http.get('api/export', { params: { 'database': $scope.database, 'type': 'xml' } })
		.then(
		function (success) {
			console.log(success);
			window.open(success.data, '_blank', '');
		})
	};

	$scope.exportSQL = function () {
		$http.get('api/export', { params: { 'database': $scope.database, 'type': 'sql' } })
		.then(
		function (success) {
			console.log(success);
			window.open(success.data, '_blank', '');
		})
	};

	$scope.upload = function () {
		var file = document.getElementById('file').files[0];
		var fd = new FormData();
		fd.append('file', file);

		$http.post('api/upload', fd, { headers: { 'Content-Type' : undefined }, params : { 'database' : $scope.database } })
		.then(
			function (success) {
				console.log(success.data);
				$scope.database = success.data.database;

				$http.get('api/metadata', { params: { 'database': success.data.database } })
				.then(
					function (success) {
						console.log(success.data);
						$scope.Myvar = success.data;
					}, 
					function (error) {
						console.log(error);
						$scope.Myvar = error;
					}
				)
			},
			function (error) {
				console.log(error);
			}
		)
	}
}]);