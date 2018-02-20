using Coalition.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Coalition
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string returningPlayer = Request.QueryString["PlayerHash"];
        }

        [WebMethod]
        public static string GetRoomHash(string hash)
        {
            return Player.GetRoomHash(hash);
        }

        [WebMethod]
        public static string StillAlivePing(string hash,string currentScreen)
        { 
            Player.StillAlive(hash);
            return Player.GetDestination(hash, currentScreen);
        }

        [WebMethod]
        public static string GetScore(string hash)
        {
            return Player.GetScore(hash);
        }
        [WebMethod]
        public static string RegisterNickname(string nickname,string assId,string workerId,string hitId)
        {
            if (Player.GetPlayer(nickname) == null)
            {
                Player p = new Player(nickname);
                p.workerID = workerId;
                p.assID = assId;
                p.hitID = hitId;
                p.futureStatus = Player.Status.WaitingRoom;
                return p.HashPlayer;
            }
            else
            {
                return "";
            }
        }
        [WebMethod]
        public static bool AnswerJoinRequest(string hash, bool answer)
        {
            Dictionary<string, Player> OnlinePlayers = (Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"];
            if (OnlinePlayers!=null)
            if (!answer)
            {
                
                // if this player is not ready, return all players to the queue
                foreach (var item in OnlinePlayers)
                {
                    if (item.Value.HashPlayer == hash)
                    {
                        foreach (var otherPlayers in OnlinePlayers)
                        {
                                if (item.Key == otherPlayers.Key)
                                {
                                    CRoom.CloseRoom(item.Value.roomHash);
                                    continue;
                                }
                            if (otherPlayers.Value.roomHash == item.Value.roomHash)
                            {
                                otherPlayers.Value.SetFutureStatus(Player.Status.WaitingRoom,false);
                            }
                        }
                        item.Value.SetFutureStatus(Player.Status.WaitingRoom, true);
                    }
                }
                
            }
            else {
                
                foreach (var item in OnlinePlayers)
                {
                    if (item.Value.HashPlayer != hash)
                    {
                        continue;
                    }

                    item.Value.SetFutureStatus(Player.Status.Ready,false);
                    
                    bool allready = true;
                    foreach (var otherPlayer in OnlinePlayers)
                    {

                        // bugfix: if the any of the players from the room has state different from ready
                        if (otherPlayer.Key != item.Key &&
                            otherPlayer.Value.roomHash == item.Value.roomHash && 
                            otherPlayer.Value.status != Player.Status.Ready &&
                            otherPlayer.Value.futureStatus != Player.Status.Ready)
                        {
                            allready = false;
                            break;
                        }
                    }
                    
                    if (allready)
                    {
                        foreach (var otherPlayer in OnlinePlayers)
                        {
                            if (otherPlayer.Value.roomHash == item.Value.roomHash)
                            {
                                otherPlayer.Value.futureStatus = Player.Status.EnteringRoom;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}