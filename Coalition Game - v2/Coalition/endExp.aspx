<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EndExp.aspx.cs" Inherits="Coalition.endExp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="js/ParticipantScripts.js"></script>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="all"/>
</head>


<body onload="displayHashToScreen()">
    <div id="wrap">
        <div id="regbar">
            <div id="navthing">
                <div  class="Bigheader">
                    <label>Thank you for your participating in our experiment</label>
                </div>
            </div>
        </div>
    </div>
    <div class="center">
        <div class="formholder">
            <div class="randompad">
                <label>Your survey code is:</label>
                <label id="playerHashDisplay"></label> 
            </div>
        </div>
    </div>
</body>
</html>
