screens = ["Home", "WaitingRoom", "WaitingForJoinResponse", "Ready", "BanPage","ExitingGame"];


function PlaySound(soundObj) {
    var sound = document.getElementById(soundObj);
    sound.Play();
}


function beep() {
    var snd = new Audio("data:audio/wav;base64,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qJrx4KOQjahZxWbcZgztj2c49nKmkId44S71j0c8eV9yDK6uPRzx5X18eDvjvQ6yKo9ZSS6l//8elePK/Lf//IInrOF/FvDoADYAGBMGb7FtErm5MXMlmPAJQVgWta7Zx2go+8xJ0UiCb8LHHdftWyLJE0QIAIsI+UbXu67dZMjmgDGCGl1H+vpF4NSDckSIkk7Vd+sxEhBQMRU8j/12UIRhzSaUdQ+rQU5kGeFxm+hb1oh6pWWmv3uvmReDl0UnvtapVaIzo1jZbf/pD6ElLqSX+rUmOQNpJFa/r+sa4e/pBlAABoAAAAA3CUgShLdGIxsY7AUABPRrgCABdDuQ5GC7DqPQCgbbJUAoRSUj+NIEig0YfyWUho1VBBBA//uQZB4ABZx5zfMakeAAAAmwAAAAF5F3P0w9GtAAACfAAAAAwLhMDmAYWMgVEG1U0FIGCBgXBXAtfMH10000EEEEEECUBYln03TTTdNBDZopopYvrTTdNa325mImNg3TTPV9q3pmY0xoO6bv3r00y+IDGid/9aaaZTGMuj9mpu9Mpio1dXrr5HERTZSmqU36A3CumzN/9Robv/Xx4v9ijkSRSNLQhAWumap82WRSBUqXStV/YcS+XVLnSS+WLDroqArFkMEsAS+eWmrUzrO0oEmE40RlMZ5+ODIkAyKAGUwZ3mVKmcamcJnMW26MRPgUw6j+LkhyHGVGYjSUUKNpuJUQoOIAyDvEyG8S5yfK6dhZc0Tx1KI/gviKL6qvvFs1+bWtaz58uUNnryq6kt5RzOCkPWlVqVX2a/EEBUdU1KrXLf40GoiiFXK///qpoiDXrOgqDR38JB0bw7SoL+ZB9o1RCkQjQ2CBYZKd/+VJxZRRZlqSkKiws0WFxUyCwsKiMy7hUVFhIaCrNQsKkTIsLivwKKigsj8XYlwt/WKi2N4d//uQRCSAAjURNIHpMZBGYiaQPSYyAAABLAAAAAAAACWAAAAApUF/Mg+0aohSIRobBAsMlO//Kk4soosy1JSFRYWaLC4qZBYWFRGZdwqKiwkNBVmoWFSJkWFxX4FFRQWR+LsS4W/rFRb/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////VEFHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAU291bmRib3kuZGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAwNGh0dHA6Ly93d3cuc291bmRib3kuZGUAAAAAAAAAACU=");
    snd.play();
}



function RegisterNickname() {
    var assId = getUrlVars()["assId"];
    var workerId = getUrlVars()["workerId"];
    var hitId = getUrlVars()["hitId"];
    $.ajax({
        type: "POST",
        url: "Main.aspx/RegisterNickname",
        data: "{'nickname':'" + document.getElementById("Nickname").value + "','assId':'"+assId+"','workerId':'"+workerId+"','hitId':'"+hitId+"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.d!=="") {
                currentHash = response.d;
            } else
                alert('Username is already taken, pick another one.');
           
        },
        failure: function (response) {
            alert(response.d);
        }
    });
}

var currentHash = "";
var refreshingID = null;

function displayHashToScreen() {
    var qs = getQueryStrings();
    document.getElementById("playerHashDisplay").innerHTML = qs["PlayerHash"];

    announceFinish(document.getElementById("playerHashDisplay").innerHTML)
}
function announceFinish(hashPlayer) {
    $.ajax({
        type: "POST",
        url: "FinalPage.aspx/Finish",
        contentType: "application/json; charset=utf-8",
        data: "{'hashPlayer':'" + hashPlayer + "'}",
        dataType: "json"
    });
}

function startRefresh() {
    var qs = getQueryStrings();
    playerHash = qs["PlayerHash"];
    
    if (playerHash != "") {
        currentHash = playerHash;

    }

    if (refreshingID!=null)
        clearInterval(refreshingID);
    refreshingID = setInterval(updateStatus, 100);
}

function updateStatus() {
    $.ajax({
        type: "POST",
        url: "Main.aspx/StillAlivePing",
        contentType: "application/json; charset=utf-8",
        data: "{'hash':'"+currentHash+"','currentScreen':'"+currentDiv+"'}",
        dataType: "json",
        success: function (response) {
            if (response.d == "NotConnected")
            {
                if (currentDiv != "Home") {
                    currentDiv = "Home";
                    window.location = "Main.aspx";
                }
                    
            }
            else if (response.d != "OK") {
                SwitchTo(response.d);
            }
            if (currentDiv == "EnteringRoom") {
                var roomHash = GetRoomHash();
                
            }
        },
        failure: function (response) {
            return null;
        }
    });
    $.ajax({
        type: "POST",
        url: "Main.aspx/GetScore",
        contentType: "application/json; charset=utf-8",
        data: "{'hash':'" + currentHash + "'}",
        dataType: "json",
        success: function (response) {
            document.getElementById("score").innerHTML = response.d;
        },
        failure: function (response) {
            return null;
        }
    });
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function GetRoomHash() {
    $.ajax({
        type: "POST",
        url: "Main.aspx/GetRoomHash",
        contentType: "application/json; charset=utf-8",
        data: "{'hash':'" + currentHash + "'}",
        dataType: "json",
        success: function (response) {

            var assId = getUrlVars()["assId"];
            var workerId = getUrlVars()["workerId"];
            var hitId = getUrlVars()["hitId"];
            //alert("ok");
            //alert(hitId);
            if (response.d != "")
               window.location.replace("ConferenceRoom.aspx?RoomHash=" + response.d + "&PlayerHash=" + currentHash + "&assId=" + assId + "&workerId=" + workerId + "&hitId=" + hitId);
        },
        failure: function (response) {
            return "";
        }
    });
}

function ResetGame(div) {
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/SetFutureStatus",
        contentType: "application/json; charset=utf-8",
        data: "{'playerHash':'" + playerHash + "','div':'" + div + "'}",
        dataType: "json",
        success: function (response) {
            return "";
        },
        failure: function (response) {
            return "";
        }
    });
}

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

function OnClickReady(flag) {
    $.ajax({
        type: "POST",
        url: "Main.aspx/AnswerJoinRequest",
        data: "{'hash':'" + currentHash + "','answer':'"+flag+"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
        },
        failure: function (response) {
            alert(response.d);
        }
    });
}


currentDiv = "Home";
waitingRoomEntranceTime = 0;
waitingRoomClock = null;
function SwitchTo(divName) {
    for (var i = 0; i < screens.length; i++) {
        var e = document.getElementById(screens[i]);
        if (divName == screens[i]) {
            e.style.display = 'block';
        }
        else {
            e.style.display = 'none';
        }
    }

    currentDiv = divName;

    if (divName == "WaitingForJoinResponse") {
        var snd = new Audio("audio/bell-ringing.wav"); 
        snd.play();
        //beep();
    }

    if (divName == "WaitingRoom") {
        waitingRoomEntranceTime = 0;
        if (waitingRoomClock != null)
            clearInterval(waitingRoomClock);
        waitingRoomClock = setInterval(timerFunction, 1000);

    }
    if (divName == "ExitingGame") {
        window.location.replace("FinalPage.aspx?PlayerHash=" + currentHash);
    }
}

function timerFunction() {
    waitingRoomEntranceTime++;

    document.getElementById("timer").innerHTML = pad(parseInt(waitingRoomEntranceTime / 60)) + ":" +
        pad(waitingRoomEntranceTime % 60);
}

function pad(val) {
    var valString = val + "";
    if (valString.length < 2) {
        return "0" + valString;
    }
    else {
        return valString;
    }
}