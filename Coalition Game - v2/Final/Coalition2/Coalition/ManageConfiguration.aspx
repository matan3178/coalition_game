<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageConfiguration.aspx.cs" Inherits="Coalition.ManageConfiguration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <script type="text/javascript" src="js/ManagerScripts.js"></script>
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="all"/>
</head>
<body style="height: 661px">
    <form runat="server">
    <div id="wrap">
        <div id="regbar">
            <div id="navthing">
                <h2>
                    Coalition Game - Configurations
                </h2>
            </div>
        </div>
    </div>

    <div  style="width: 95%; height: 641px; margin-left:50px">
        <div id="Home" class="formholder" style="width: 90%; height: 641px;">
            <div class="randompad">
                 <asp:Button runat="server" Text="Load Existing Configurations" OnClick="Unnamed_Click" Width="300px"> </asp:Button>
                <label>   Add Configuration:</label>
                <br />
                <asp:TextBox  ID="txt_configurations" runat="server" Height="500px"  TextMode = "MultiLine" Width="1047px"></asp:TextBox>
                <div style="float:left;width:330px">
                    <asp:Button runat="server" Text="Save Configuration" OnClick="Unnamed3_Click" Width="300px"> </asp:Button>
                </div>
         </div>

        </div>
        </div>
        </form>
</body>
</html>
