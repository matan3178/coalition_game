<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Coalition.Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="js/ParticipantScripts.js"></script>
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="all"/>
    <script>
        $(document).ready(function () {
            SwitchTo('Home');
        });
    </script>
</head>
<body onload="startRefresh()">
    <div id="wrap">
        <div id="regbar">
            <div id="navthing">
                <div  class="Bigheader">
                    <label>
                        &nbsp;</label><label>Current Score:</label>
                    <label id="score"></label>
                </div>
            </div>
        </div>
    </div>
    <div class="center">
        <div id="Home" class="formholder" style="display:none">
            <div class="randompad">
                <label>Enter your Nickname:</label>
                <input type="text" id="Nickname" />
                <input type="submit"  onclick="RegisterNickname()" value="Begin"/> 
            </div>
        </div>
        <div id="WaitingRoom" class="formholder" style="display:none">
            <div class="randompad">
                <label><i>Please hold on, soon you will be grouped with other people</i></label><br/><br/>  
                <div class="center">
                    <label >Waited so far: </label><label id="timer">00:00</label>
                </div>
                <br />
                <label>Please click on the exit button only if you wish to terminate the experiment.</label>
                <br />
                <div class="negativeResponse">
                    <input type="submit" onclick="SwitchTo('ExitingGame')" value="Exit"/>
                </div>
            </div>
        </div>
        <div id="WaitingForJoinResponse" class="formholder" style="display:none">
            <div class="randompad">
                <label>Group found, are you ready to start?</label>
                <input type="submit" onclick="OnClickReady(true)" value="Ready"/><br/>
                <div class="negativeResponse">
                    <input type="submit" onclick="OnClickReady(false)" value="Not Ready"/>
                </div>
            </div>
        </div>
        <div id="Ready" class="formholder" style="display:none">
            <div class="randompad">
                The game will begin momentarily.
            </div>
        </div>
        <div id="BanPage" class="formholder" style="display:none">
            <div class="randompad">
                <label>BanPage</label>
                <label>:'(</label>
            </div>
        </div>
         <div id="ExitingGame" class="formholder" style="display:none">
            <div class="randompad">
                <label>One moment...</label>
            </div>
        </div>
    </div>
</body>
</html>
