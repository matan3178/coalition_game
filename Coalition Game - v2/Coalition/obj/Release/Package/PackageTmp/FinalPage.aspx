<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinalPage.aspx.cs" Inherits="Coalition.FinalPage" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
	<meta charset="utf-8" />
    <script src="js/ParticipantScripts.js"></script>
    <link href="css/style.css" rel="stylesheet" />
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
</head>
<body onload="displayHashToScreen()">
    <div id="wrap">
        <div id="regbar">
            <div id="navthing">
                <div class="Bigheader">
                    <label>Thank you for your participating in our experiment</label>
                </div>
            </div>
        </div>
    </div>
    <div class="center">
        <div class="formholder">
            <div class="randompad">
                <label>For Amazon Turk participants:</label><br />
                <label>Your survey code is:</label><br /><br />
                <b><label id="playerHashDisplay"></label></b><br />

                <br />

                <label>Go back to mechanical turk and paste it there.</label>
            </div>
        </div>
    </div>
</body>
</html>