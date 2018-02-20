var refreshingID = null;
var roomsUpdate = null;

function startRefreshing() {
    refreshingID = setInterval(function() {
        refreshUsersTable(500);
    }, 500);

    if (roomsUpdate != null)
        clearInterval(roomsUpdate);
    roomsUpdate = setInterval(function () {
        roomsStatusUpdate();
    }, 100);
}
function roomsStatusUpdate() {
    $.ajax({
        type: "POST",
        url: "ManagementPanel.aspx/RoomsUpdate",
        contentType: "application/json; charset=utf-8",
        data: "{}",
        dataType: "json",
        success: function (response) {
        },
        failure: function (response) {
        }
    });
}
function refreshUsersTable(dt) {
    $.ajax({
        type: "POST",
        url: "ManagementPanel.aspx/GetActiveUsers",
        contentType: "application/json; charset=utf-8",
        data: "{'dt':" + dt + "}",
        dataType: "json",
        success: function (response) {
            buildHtmlTable(response.d);
        },
        failure: function (response) {
            return null;
        }
    });
}

function openNewRoom(size,ai) {
    $.ajax({
        type: "POST",
        url: "ManagementPanel.aspx/OpenNewRoom",
        contentType: "application/json; charset=utf-8",
        data: "{'RoomSize':" + size + ",'random':'"+document.getElementById("pickRandomly").isChecked+"','AI':'"+ai+"'}",
        dataType: "json",
        success: function (response) {
            document.getElementById("OpenRoomResponse").innerHTML = response.d;
        },
        failure: function (response) {
            return false;
        }
    });
}

function start2PlayersGame() {
    openNewRoom(2,false);
}
function start3PlayersGame() {
    openNewRoom(3, false);
}
function start4PlayersGame() {
    openNewRoom(4, false);
}
function start5PlayersGame() {
    openNewRoom(5, false);
}
function start6PlayersGame() {
    openNewRoom(6, false);
}

function start7PlayersGame() {
    openNewRoom(7, false);
}

function start2PlayersAIGame() {
    openNewRoom(2,true);
}
function start3PlayersAIGame() {
    openNewRoom(3, true);
}
function start4PlayersAIGame() {
    openNewRoom(4, true);
}
function start5PlayersAIGame() {
    openNewRoom(5, true);
}
function start6PlayersAIGame() {
    openNewRoom(6, true);
}
function start7PlayersAIGame() {
    openNewRoom(7, true);
}
function RemovePlayer(playerNickname) {
    $.ajax({
        type: "POST",
        url: "ManagementPanel.aspx/RemovePlayer",
        contentType: "application/json; charset=utf-8",
        data: "{'PlayerNick':'" + playerNickname + "'}",
        dataType: "json",
        success: function (response) {
            return true;
        },
        failure: function (response) {
            return false;
        }
    }); 
}
function sendToFinalPage(playerNickname) {
    $.ajax({
        type: "POST",
        url: "ManagementPanel.aspx/sendToFinalPage",
        contentType: "application/json; charset=utf-8",
        data: "{'PlayerNick':'" + playerNickname + "'}",
        dataType: "json",
        success: function (response) {
            return true;
        },
        failure: function (response) {
            return false;
        }
    });
}


function buildHtmlTable(data) {
    var users = JSON.parse(data);
    var numberOfConnectedUsers = users.length;
    var table = document.getElementById('usersTable');
    table.innerHTML = "<tr><td>Nickname</td><td>Time Entered</td><td>Status</td><td>Score</td><td>NumberOfGames</td><td>Status</td><td>Remove</td><td>Finish</td></tr>";
    
    for (var i = 0; i < users.length; i++) {
        var newString = "<tr>";
        var user = users[i];      
        newString += "<td>" + user['Nickname'] + "</td>";
        newString += "<td>" + user['EntranceTime'] + "</td>";
        newString += "<td>" + user['Status'] + "</td>";
        newString += "<td>" + user['Score'] + "</td>";
        newString += "<td>" + user['NumberOfGames'] + "</td>";
        if (user['conStat'] == 'Connected') {
            newString += "<td>Connected</td>";
        } else {
            newString += "<td>Disconnected, Last seen: " + user['LastSeen'] + "</td>";
        }
        //button id="btnStartGame4AI" onclick="start4PlayersAIGame()">4 Players with AI</button>
        newString += "<td><button onclick=RemovePlayer('" + user['Nickname'] + "')>Kick</button></td>";
        newString += "<td><button onclick=sendToFinalPage('" + user['Nickname'] + "')>finish</button></td>";
        newString += "</tr>";
        table.innerHTML += newString;
    }
}