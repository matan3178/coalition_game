<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagementPanel.aspx.cs" Inherits="Coalition.ManagementPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <script type="text/javascript" src="js/ManagerScripts.js"></script>
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="all"/>
</head>
<body onload="startRefreshing()">
             <form id="form1" runat="server">
             <div id="Home" class="formholderAdmin">
            <div class="randompad">
        <table id="usersTable" border="1" style="margin-left:auto; margin-right:auto; width:auto"></table>
        <label>
           
        </label>
                <br />
    <table border="1" style="margin-left:auto; margin-right:auto; width:auto">
        <tr>
            <td>
                 Open new room
            </td>
            <td>
                <span>Pick players randomly</span>
                 <input type="checkbox" id="pickRandomly"/>
            </td>
            <td>
                <span>Add AI Player</span>
            </td>
            <td>
                <span>Auto start when ready (+ 1 AI Player<asp:CheckBox ID="cbAutoStartWithAI" runat="server" OnCheckedChanged="cbPlayers_CheckedChanged"  AutoPostBack="True" />)</span>
            </td>
        </tr>
        <tr>
            <td>
                2 Players
            </td>
            <td>
                <button id="btnStartGame2" onclick="start2PlayersGame()">Start</button>
            </td>
            <td>
                <button id="btnStartGame2AI" onclick="start2PlayersAIGame()">2 Players + 1 AI ( 3 players )</button>
            </td>
            <td>
                <asp:CheckBox ID="cb2players" runat="server" OnCheckedChanged="cbPlayers_CheckedChanged"  AutoPostBack="True" />
            </td>
        </tr>
                <tr>
            <td>
                3 Players
            </td>
            <td>
                <button id="btnStartGame3" onclick="start3PlayersGame()">Start</button>
            </td>
            <td>
                <button id="btnStartGame3AI" onclick="start3PlayersAIGame()">3 Players + 1 AI ( 4 players )</button>
            </td>
             <td>
                 <asp:CheckBox ID="cb3players" runat="server" AutoPostBack="True" OnCheckedChanged="cbPlayers_CheckedChanged"  />
             </td>
        </tr>
                <tr>
            <td>
                4 Players
            </td>
            <td>
                <button id="btnStartGame4" onclick="start4PlayersGame()">Start</button>
            </td>
            <td>
                <button id="btnStartGame4AI" onclick="start4PlayersAIGame()">4 Players + 1 AI ( 5 players )</button>
            </td>
            <td>
                 <asp:CheckBox ID="cb4players" runat="server" AutoPostBack="True" OnCheckedChanged="cbPlayers_CheckedChanged"  />
             </td>
        </tr>
                <tr>
            <td>
                5 Players
            </td>
            <td>
                <button id="btnStartGame5" onclick="start5PlayersGame()">Start</button>
            </td>
            <td>
                <button id="btnStartGame5AI" onclick="start5PlayersAIGame()">5 Players + 1 AI ( 6 players )</button>
            </td>
            <td>
                 <asp:CheckBox ID="cb5players" runat="server" AutoPostBack="True" OnCheckedChanged="cbPlayers_CheckedChanged"  />
             </td>
        </tr>
                <tr>
            <td>
                6 Players
            </td>
            <td>
                <button id="btnStartGame6" onclick="start6PlayersGame()">Start</button>
            </td>
            <td>
                <button id="btnStartGame6AI" onclick="start6PlayersAIGame()">6 Players + 1 AI ( 7 players )</button>
            </td>
            <td>
                 <asp:CheckBox ID="cb6players" runat="server" AutoPostBack="True" OnCheckedChanged="cbPlayers_CheckedChanged" />
             </td>
        </tr>
         <tr>
            <td>
                7 Players
            </td>
            <td>
                <button id="btnStartGame7" onclick="start7PlayersGame()">Start</button>
            </td>
            <td>
                <button id="btnStartGame7AI" onclick="start7PlayersAIGame()">7 Players + 1 AI ( 8 players )</button>
            </td>
            <td>
                 <asp:CheckBox ID="cb7players" runat="server" AutoPostBack="True" OnCheckedChanged="cbPlayers_CheckedChanged" />
             </td>
        </tr>
    </table>
    <label id="OpenRoomResponse"></label>    
    </div>
  </div>
             </form>
</body>
</html>
