function getQueryStrings() {
    var assoc = {};
    var decode = function (s) { return decodeURIComponent(s.replace(/\+/g, " ")); };
    var queryString = location.search.substring(1);
    var keyValues = queryString.split('&');

    for (var i in keyValues) {
        var key = keyValues[i].split('=');
        if (key.length > 1) {
            assoc[decode(key[0])] = decode(key[1]);
        }
    }
    return assoc;
}

var workerId = 0
var hitId = 0
var assignmentId = 0


function ReadParams() {
    params = getQueryStrings();

    workerId = params["workerId"]
    hitId = params["hitId"]
    assignmentId = params["assId"]
}


