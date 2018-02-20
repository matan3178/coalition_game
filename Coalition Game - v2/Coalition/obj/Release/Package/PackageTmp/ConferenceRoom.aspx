<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConferenceRoom.aspx.cs" Inherits="Coalition.ConferenceRoom" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>   
     <script type="text/javascript" src="js/ConfRoomScripts.js"></script>
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
    <link rel="stylesheet" href="css/style.css" type="text/css" media="all" />
     <link rel="stylesheet" href="css/proposalCss.css" type="text/css" media="all" />
</head>
<body onload="startRefresh()"">
        <div id="wrap">
        <div id="regbar">
            <div id="navthing">  
                <div  class="Bigheader">
                    <label>
                        Time Left:</label>
                    <label id="timeleft"></label>
                </div>
                 <div id="newRound" visible="false" class="Bigheader">
                    <label>
                        New Round</label>
                    <label id="newRoundLabel"></label>
                </div>
            </div>
        </div>
    </div>
    <div id="players-table" class="clear">

    </div>
    <div id="Game">
        <table id="GameTable">
           
        </table>
    </div>
    <div id="InspectingProposal" style="display:none">

    </div>
    <div id="Proposing" style="display:none">
         <div id="Home" class="formholder">
            <div class="randompad">
                <label><b>Total Weight:</b></label>    
                 <label id="totalWeight">0</label> <br />
                 <label><b>Total Pay:</b></label>    
                 <label id="totalPay">0</label> <br />
                 
                <input id="sendProposals" type="submit" onclick="SendProposals()" value="Propose" disabled="disabled"/><br/>
            </div>       
        </div>           
    </div>
    <div id="ProposalInspection" class="formholder"  style="display:none">
        <div class="randompad">
            <label><b>You were given: </b></label>
            <label id ="givenShareToPlayer"></label><br />
            <input type="submit" onclick="OnClickAccept('Accept')" value="Accept"/><br/>
            <div class="negativeResponse">
                <input type="submit" class="badPerson" onclick="OnClickAccept('Refuse')" value="Refuse" />
            </div>
        </div>
    </div>
    <div id="WaitingForProposal" class="formholder" style="display:none">
       <div class="randompad">
            <label><b>Waiting for proposal..</b></label>
        </div>
    </div>
    <div id="None" style="display:none">

    </div>
    <div id="WaitingRoom" class="formholder" style="display:none">
         <div class="randompad">
            <label><b>You will be redirected to the waiting room momentarily.</b></label>
         </div>
    </div>
   
    <div id="ProposerWaitingForAnswer" class="formholder" style="display:none">
       <div class="randompad">
            <label><b>Waiting for feedback..</b></label>
        </div>
    </div>
    <div id="FinishedRespondingWaiting" class="formholder" style="display:none">
         <div class="randompad">
            <label><b>Thank you for your response. Waiting for others..</b></label>
         </div>
    </div>
    <div id="GameFinishedCoalitionFormedNotInc" class="formholder" style="display:none">
         <div class="randompad">
            <label><b>The round has ended. Coalition has been <i>formed.</i> </b></label><br />
            <label>Unfortunately! You are <b>not</b> a member of the coalition.</label>
         </div>
    </div>
    <div id="GameFinishedCoalitionFormedInc" class="formholder" style="display:none">
         <div class="randompad">
            <label><b>The round has ended. Coalition has been <i>formed.</i> </b></label><br />
            <label>Congratulations! <b>You are a member of the coalition.</b></label>
         </div>
    </div>
    <div id="GameFinishedCoalitionNotFormed" class="formholder" style="display:none">
         <div class="randompad">
            <label><b>The round has ended.</b></label>
             <label><b>Coalition has <i>not been formed.</i> </b></label>
         </div>
    </div>
    <div id="WaitingNotGivenOffer" class="formholder" style="display:none">
         <div class="randompad">
            <label><b>Unfortunately, you were not given an offer. </b></label>
             <label><b>Please wait..</b></label>
         </div>
    </div>
</body>
</html>
