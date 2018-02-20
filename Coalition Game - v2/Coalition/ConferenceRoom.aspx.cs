using Coalition.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Coalition
{
    public partial class ConferenceRoom : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        [WebMethod]
        public static string GetGameEndStatus(string roomHash)
        {
            CRoom r = CRoom.GetRoom(roomHash);
            if (r == null)
                return null;
            string answer = r.GetGameResponses();
            return answer;
        }

        [WebMethod]
        public static string SetFutureStatus(string playerHash, string div)
        {
            switch (div)
            {
                case "WaitingRoom":
                    Player.SetFutureStatus(playerHash, Player.Status.WaitingRoom);
                    break;
                default:
                    break;
            }
            return "OK";
        }
        
        [WebMethod]
        public static string SetResponse(string playerHash,string roomHash,string response)
        {
            CRoom r = CRoom.GetRoom(roomHash);
            if (r == null)
                return null;
            if (r.UpdatePlayerResponse(roomHash, playerHash, response))
                return "OK";
            else
                return "NotOK";
        }

        [WebMethod]
        public static string SetInitRole(string playerHash, string roomHash)
        {
            CRoom r = CRoom.GetRoom(roomHash);
            if (r == null)
                return null;

            r.UpdatePlayerRoles(playerHash);
            return "OK";
        }


        [WebMethod]
        public static string StillAlivePing(string playerhash,string roomHash, string currentScreen)
        {
            Player.StillAlive(playerhash);
            string answer= Player.GetDestination(playerhash, currentScreen);
            return answer;
        }
        
        [WebMethod]
        public static string GetGameSettings(string roomHash)
        {
            CRoom r = CRoom.GetRoom(roomHash);
            if (r == null)
                return null;

            var json = new JavaScriptSerializer().Serialize(r.GetGameSettings());
            return json;
        }
        
        [WebMethod]
        public static string GetActiveGameUsers(string playerhash, string roomHash)
        {
            CRoom r = CRoom.GetRoom(roomHash);
            if (r == null)
                return null;
            InGamePlayer[] allPlayers = r.GetAllGamesPlayers(playerhash).ToArray();
          

            var json = new JavaScriptSerializer().Serialize(allPlayers);
            return json;
        }     
    }
}