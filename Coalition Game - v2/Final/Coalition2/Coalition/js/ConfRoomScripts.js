screens = ["None", "Proposing", "WaitingForProposal", "ProposalInspection","GameFinishedCoalitionFormedInc","GameFinishedCoalitionFormedNotInc","GameFinishedCoalitionNotFormed","FinishedRespondingWaiting", "ProposerWaitingForAnswer", "WaitingNotGivenOffer" , "WaitingRoom"];
var playerHash;
var roomHash;
var currentDiv = "None";
var timerID;
var response = "";
var numberOfConnectedUsers;

var decisionTimeOut;
var proposalTimeOut;
var numberOfRemainingRounds;
var timeCounter = 0;
var currentRound = 0;
var playerID = 0;
var isProposer = false;
var confirmedPays;



function startRefresh() {

    var qs = getQueryStrings();
    playerHash = qs["PlayerHash"];
    roomHash = qs["RoomHash"];
    
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/SetInitRole",
        contentType: "application/json; charset=utf-8",
        data: "{'playerHash':'" + playerHash + "','roomHash':'" + roomHash + "'}",
        dataType: "json",
        success: function (response) {
            SwitchTo(currentDiv);
            return ""
        },
        failure: function (response) {
            return "";
        }
    });
    

    gameSettingsRef = setInterval(GetAndSetGameSettings,100);
    refreshingID = setInterval(updateStatus, 100);

}
function GotoHome(div) {
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/SetFutureStatus",
        contentType: "application/json; charset=utf-8",
        data: "{'playerHash':'" + playerHash + "','div':'"+div+"'}",
        dataType: "json",
        success: function (response) {
            return "";
        },
        failure: function (response) {
            return "";
        }
    });
}
function GetAndSetGameSettings() {
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/GetGameSettings",
        data: "{'roomHash':'" + roomHash +"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var properties = JSON.parse(response.d);
            decisionTimeOut = properties[0];
            proposalTimeOut = properties[1];
            numberOfRemainingRounds = properties[2];
            PrintTimer();
        },
        failure: function (response) {
            //alert(response.d);
        }
    });
}

function PrintTimer() {
    var totalremaining = 0;
    switch (currentDiv) {
        case "ProposalInspection":
            totalremaining = decisionTimeOut;
            document.getElementById("newRound").style.visibility = "hidden";
            break
        case "WaitingForProposal":
            totalremaining = proposalTimeOut;
            document.getElementById("newRound").style.visibility = "visible";
            break;
        case "Proposing":
            totalremaining = proposalTimeOut;
            document.getElementById("newRound").style.visibility = "visible";
            break;
        case "FinishedRespondingWaiting":
            totalremaining = decisionTimeOut;
            break
        case "ProposerWaitingForAnswer":
            totalremaining = decisionTimeOut;
            document.getElementById("newRound").style.visibility = "hidden";
            break
        case "WaitingNotGivenOffer":
            totalremaining = decisionTimeOut;
            document.getElementById("newRound").style.visibility = "hidden";
            break
        case "FinishedRespondingWaiting":
            totalremaining = decisionTimeOut;
        default:
            totalremaining = 0;
            break;
    }
    var seconds = totalremaining % 60;
    var minutes = parseInt(totalremaining / 60);
    document.getElementById("timeleft").innerHTML = pad(minutes) + ":" + pad(seconds);
}
function updateStatus() {
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/StillAlivePing",
        contentType: "application/json; charset=utf-8",
        data: "{'playerhash':'" + playerHash + "','roomHash':'" + roomHash + "','currentScreen':'" + currentDiv + "'}",
        dataType: "json",
        success: function (response) {
            if (response.d != "OK") {
                SwitchTo(response.d)
            }
        },
        failure: function (response) {
            return null;
        }
    });
}

function updatePlayersTable() {
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/GetActiveGameUsers",
        contentType: "application/json; charset=utf-8",
        data: "{'playerhash':'" + playerHash + "','roomHash':'" + roomHash +  "'}",
        dataType: "json",
        success: function (response) {
            DisplayPlayers(response.d);
            updateTotal();
        },
        failure: function (response) {
            return null;
        }
    });
}

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

    //timeCounter = 0;

    //UpdateTimer();
    //GetAndSetGameSettings();
    currentDiv = divName;

    updatePlayersTable();
    
    switch(currentDiv) {
        case "GameFinishedCoalitionNotFormed":
            var isGameFinished = GetGameEndStatus();
            break;
        case "GameFinishedCoalitionFormedNotInc":
            var isGameFinished = GetGameEndStatus();
            break;
        case "GameFinishedCoalitionFormedInc":
            var isGameFinished = GetGameEndStatus();
            break;

        case "WaitingRoom":

            setTimeout(function () {
                var assId = getUrlVars()["assignmentId"];
                var workerId = getUrlVars()["workerId"];
                var hitId = getUrlVars()["hitId"];
                window.location.replace("Main.aspx?PlayerHash=" + playerHash + "&assignmentId=" + assId + "&workerId=" + workerId + "&hitId=" + hitId);
            }, 2000);      
            break;
    }

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

function GetGameEndStatus() {
    var answer;
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/GetGameEndStatus",
        contentType: "application/json; charset=utf-8",
        data: "{'roomHash':'" + roomHash + "'}",
        dataType: "json",
        success: function (response) {
            var answer = false;
            var usersOffers = JSON.parse(response.d);
            for (var i = 0; i < numberOfConnectedUsers ; i++) {
                var pay = usersOffers[i].Item1;
                var currHashKey = usersOffers[i].Item3;

                for (var j = 0; j < numberOfConnectedUsers ; j++) {
                    if (document.getElementById("playerHash" + j).innerHTML == currHashKey)
                        break;
                }

                var responded = usersOffers[j].Item2;
                var proposer = usersOffers[j].Item4;


                var payLabel = document.getElementById("payLabel" + j);

                var responseLabel = document.getElementById("responseLabel" + j);

                if (pay > 0 && !proposer)
                {
                    payLabel.innerHTML = pay;
                    responseLabel.innerHTML = responded;
                }
                else if (proposer && pay > 0)
                {
                    payLabel.innerHTML = pay;
                    responseLabel.innerHTML = "";
                }
                else
                {
                    payLabel.innerHTML = "";
                    responseLabel.innerHTML = "";
                }
            }
        },
        failure: function (response) {
            answer = false;
        }
    });
    return answer;
}

function OnClickAccept(flag) {
    SetResponse(flag);
}
function DisplayPlayers(result) {

    var users = JSON.parse(result);
    numberOfConnectedUsers = users.length;

    confirmedPays = []

    playersDiv = document.getElementById("players-table");
    playersDiv.innerHTML = "";
    for (var i = 0; i < numberOfConnectedUsers; i++) {

        user = users[i];

        var playerNameLabel = document.createElement("h3");

        if (user["nick"] != "Anonymos")
        {
            playerID = i;
            if (user["role"] == "Proposer")
                playerNameLabel.innerHTML = "Player " + (i + 1) + " - " + user["nick"] + "<br/>(proposer)";

            else
                playerNameLabel.innerHTML = "Player " + (i+1) + " - " + user["nick"];
        }
        else
        {
            if (user["role"] == "Proposer")
                playerNameLabel.innerHTML = "Player " + (i + 1) + "<br/>(proposer)";
            else
                playerNameLabel.innerHTML = "Player " + (i+1);
        }

        var playerWeightLabel = document.createElement("label");
        var playerWeightValue = document.createElement("label");
        playerWeightLabel.innerHTML = "Weight: ";
        playerWeightValue.innerHTML = user["weight"];
        playerWeightValue.id = "weight" + i;
        var playerBox = document.createElement("div");
        playerBox.className = "plan";

        var imgSpan = document.createElement("span");

        if (user["nick"] == "Anonymos")
            imgSpan.innerHTML = "<img src='css//player.png' style=' border-radius: 50%!important; width: inherit; '>";
        else 
            imgSpan.innerHTML = "<img src='css//playerMe.png' style=' border-radius: 50%!important; width: inherit; '>";

        playerNameLabel.appendChild(imgSpan);
        playerBox.appendChild(playerNameLabel);
        playerBox.appendChild(playerWeightLabel)

        playerBox.appendChild(playerWeightValue);
        playerBox.appendChild(document.createElement("br"));
        if (user["role"] == "Proposer") {
            playerBox.id = "most-popular";
            var playerProposerLabel = document.createElement("b");
            //playerProposerLabel.innerHTML = "Proposer";
            playerBox.appendChild(playerProposerLabel);
        }
        playerBox.appendChild(document.createElement("br"));
        playerBox.appendChild(document.createElement("br"));
        if (currentDiv == "Proposing") {

            isProposer = true;
            var playerAmountLabel = document.createElement("input");

            playerBox.appendChild(document.createElement("br"));

            var paylabel = document.createElement("b");
            paylabel.innerHTML = "Pay: ";
            playerBox.appendChild(paylabel);
            playerAmountLabel.type = "number";
            playerAmountLabel.id = "labelPay" + i;
            playerAmountLabel.width = 50;
            playerAmountLabel.value = "";
            playerAmountLabel.max = 100;
            playerAmountLabel.min = 0;

            var span1 = document.createElement('span');
            span1.innerHTML = '<button id="addBtn' + i + '" type="submit" onclick="confirmPay(' + i + ',true)">Add</button>';
            var span2 = document.createElement('span');
            span2.innerHTML = '<button id="rmvBtn' + i + '" type="submit" onclick="confirmPay(' + i + ',false)" disabled>Remove</button>';


            playerBox.appendChild(playerAmountLabel);
            playerBox.appendChild(document.createElement("br"));
            playerBox.appendChild(span1);
            playerBox.appendChild(document.createElement("br"));
            playerBox.appendChild(span2);
        }

        var playerHashLabelSym = document.createElement("label");
        playerHashLabelSym.innerHTML = user["hash"];
        playerHashLabelSym.hidden = "true";
        playerHashLabelSym.id = "playerHash" + i;
        playerBox.appendChild(playerHashLabelSym);

        var outcomeDiv = document.createElement("div");
        outcomeDiv.id = "OutcomeDiv" + i;
        
        var payLabel = document.createElement("label");
        payLabel.id = "payLabel" + i;
        var offer = user["offer"];
        if (offer > 0) {
            payLabel.innerHTML = offer;
            if (user["nick"] == "You") {
                document.getElementById("givenShareToPlayer").innerHTML = offer;
            }
        }
        var responseLabel = document.createElement("label");
        responseLabel.id = "responseLabel" + i;

        outcomeDiv.appendChild(payLabel);
        outcomeDiv.appendChild(document.createElement("br"));
        outcomeDiv.appendChild(responseLabel);

        playerBox.appendChild(outcomeDiv);
        playersDiv.appendChild(playerBox);
    }
}
function confirmPay(payID, flag) {

    var myPay = document.getElementById("labelPay" + payID).value;
    if (myPay <= 0 || myPay>100) {
        alert("You must offer between 1 and 100!");
        return;
    }


    confirmedPays[payID] = flag;
    if (flag) {
        document.getElementById("labelPay" + payID).disabled = "true";
        document.getElementById("rmvBtn" + payID).removeAttribute('disabled');
        document.getElementById("addBtn" + payID).disabled = "true";
    }
    else {
        document.getElementById("labelPay" + payID).removeAttribute('disabled');
        document.getElementById("rmvBtn" + payID).disabled = "true";
        document.getElementById("addBtn" + payID).removeAttribute('disabled');
    }
    updateTotal();

}
function updateTotal(value, id) {
    var total = 0;
    var totalWeight = 0;
    for (var i = 0; i < numberOfConnectedUsers; i++) {
        var element = document.getElementById("labelPay" + i);
        var weightElem = document.getElementById("weight" + i);
        if (confirmedPays[i]) {
            total += Number(element.value);
            totalWeight += Number(weightElem.innerHTML);
        }
    }

    document.getElementById("totalWeight").innerText = totalWeight + " / 10";

    document.getElementById("totalPay").innerText = total + " / 100";
    if (total == 100 && totalWeight>=10) {
        document.getElementById("sendProposals").disabled = false;
    } else {
        document.getElementById("sendProposals").disabled = true;
    }
}

function SendProposals() {
    var paysArr = {};
    var proposerIsOffered = false;
    for (var i = 0; i < numberOfConnectedUsers; i++) {
        
        var value;
        var hashLabel = document.getElementById("playerHash" + i).innerHTML;
        if (confirmedPays[i])
            value = Number(document.getElementById('labelPay' + i).value);
        else
            value = 0;
        if (value > 0 && playerHash == hashLabel)
            proposerIsOffered = true;
        paysArr[hashLabel] = value;
    }
    if (!proposerIsOffered)
    {
        alert("You must include yourself in the proposals!");
        return;
    }
    var str = JSON.stringify(paysArr);
    //alert(str);
    SetResponse(str);
}

function SetResponse(response) {
    $.ajax({
        type: "POST",
        url: "ConferenceRoom.aspx/SetResponse",
        data: "{'playerHash':'" + playerHash + "','roomHash':'" + roomHash + "','response':'" + response + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
        },
        failure: function (response) {
            alert(response.d);
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

function pad(val) {
    var valString = val + "";
    if (valString.length < 2) {
        return "0" + valString;
    }
    else {
        return valString;
    }
}