using Coalition.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Coalition
{
    public partial class ManagementPanel : System.Web.UI.Page
    {
        static Mutex retPlayers = new Mutex();
        static Mutex autoOperRooms = new Mutex();
        private static bool open2PlayersRoom = false;
        private static bool open3PlayersRoom = false;
        private static bool open4PlayersRoom = false;
        private static bool open5PlayersRoom = false;
        private static bool open6PlayersRoom = false;
        private static bool openWithAIroom = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            DAL.ReadRoomConfigurationFile();
            DAL.ReadGameConfigurationFile();
        }

        private static Random rng = new Random();

        public static IList<Player> Shuffle(IList<Player> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Player value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        [WebMethod] 
        public static string OpenNewRoom(int RoomSize,string random,string AI)
        {
            Player[] players = Player.GetAllPlayers();
            List<Player> joiningPlayers = new List<Player>();
            IList<Player> sortedPlayers = players.OrderBy(si => si.EntranceTime).ToList();
            if (random == "True")
            {
                sortedPlayers = Shuffle(sortedPlayers);
            }
            int i = 0;

            try
            {
                retPlayers.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                retPlayers.ReleaseMutex();
                retPlayers.WaitOne();
            }

            foreach (var player in sortedPlayers)
            {
                if (player.conStat == Player.Connection.Connected && player.status == Player.Status.WaitingRoom &&
                    player.futureStatus == player.status)
                {
                    joiningPlayers.Add(player);
                    i++;
                    if (i == RoomSize)
                        break;
                }
            }
            retPlayers.ReleaseMutex();

            if (i < RoomSize)
            {
                return "Not enough players to open such room";
            } else
            {
                List<string> playersHash = new List<string>();
                foreach (var item in joiningPlayers)
                {
                    playersHash.Add(item.HashPlayer);
                }
                //Room newRoom = new Room(RoomSize, playersHash.ToArray());
                CRoom cRoom;
                if (AI=="false")
                    cRoom = new CRoom(playersHash.ToArray(), false);
               else
                    cRoom = new CRoom(playersHash.ToArray(), true);

                foreach (var item in joiningPlayers)
                {
                    //item.AddPlayerToRoom(newRoom._hashRoom);
                    item.AddPlayerToRoom(cRoom.roomHash);
                    item.SetFutureStatus(Player.Status.WaitingForJoinResponse, false);
                }
                return "Join request sent to all players.";
            }
        }
        [WebMethod]
        public static string sendToFinalPage(string PlayerNick)
        {
            Player.setNickNameFutureStatus(PlayerNick, Player.Status.ExitingGame);
            return "OK";
        }
        [WebMethod]
        public static string RemovePlayer(string PlayerNick)
        {
            Player.RemovePlayer(PlayerNick);
            return "OK";
        }

        [WebMethod]
        public static string GetActiveUsers(string dt)
        {

            AutoOpenRooms();

            Player[] allPlayers = Player.GetAllPlayers();

            List<Dictionary<string, string>> listOfPlayers = new List<Dictionary<string, string>>();

            foreach (var player in allPlayers)
            {
                GameConfiguration gc = DAL.GetGameSettings();

                if (player.NumberOfGames >= gc.MaxPlayedGames ||
                    player.TimeWaitedSeconds / 1000 >= gc.MaxWaitTime)
                        player.SetFutureStatus(Player.Status.ExitingGame, false);


                if ((DateTime.Now - player.LastSeen).Seconds >= 3)
                    player.conStat = Player.Connection.Disconneced;
                else
                    player.conStat = Player.Connection.Connected;

                if ((DateTime.Now - player.LastSeen).Seconds >= 15)
                {
                    player.RemovePlayer();           
                }

                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("Nickname", player.Nickname);
                values.Add("EntranceTime", player.UserNameTimeCreated.ToShortTimeString());
                values.Add("Status", player.status.ToString());
                int totalScore = player.PlayScore + player.WaitScore;
                values.Add("Score", totalScore.ToString());
                values.Add("NumberOfGames", player.NumberOfGames.ToString());
                values.Add("conStat", player.conStat.ToString());
                values.Add("LastSeen", player.LastSeen.ToString("hh:mm:ss"));
                listOfPlayers.Add(values);

                if (player.status == Player.Status.WaitingRoom)
                {
                    player.UpdateTimeWaited(int.Parse(dt));
                }

            }
           
            var json = new JavaScriptSerializer().Serialize(listOfPlayers.ToArray());
            return json;
        }

        private static void AutoOpenRooms()
        {

            try
            {
                autoOperRooms.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                autoOperRooms.ReleaseMutex();
                autoOperRooms.WaitOne();
            }

            Player[] players = Player.GetAllPlayers();
            List<Player> joiningPlayers = new List<Player>();
            IList<Player> sortedPlayers = players.OrderBy(si => si.EntranceTime).ToList();

            int i = 0;

            foreach (var player in sortedPlayers)
            {
                if (player.conStat == Player.Connection.Connected && player.status == Player.Status.WaitingRoom &&
                    player.futureStatus == player.status)
                {
                    joiningPlayers.Add(player);
                    i++;
                }
            }
            try
            {

                for (int k = 6; k >= 2; k--)
                {
                    if (k <= i)
                    {

                        switch (k)
                        {
                            case 2:
                                if (open2PlayersRoom && openWithAIroom)
                                    OpenNewRoom(2, "False", "true");
                                else if (open2PlayersRoom)
                                    OpenNewRoom(2, "False", "false");
                                break;
                            case 3:
                                if (open3PlayersRoom && openWithAIroom)
                                    OpenNewRoom(3, "False", "true");
                                else if (open3PlayersRoom)
                                    OpenNewRoom(3, "False", "false");
                                break;
                            case 4:
                                if (open4PlayersRoom && openWithAIroom)
                                    OpenNewRoom(4, "False", "true");
                                else if (open4PlayersRoom)
                                    OpenNewRoom(4, "False", "false");
                                break;
                            case 5:
                                if (open5PlayersRoom && openWithAIroom)
                                    OpenNewRoom(5, "False", "true");
                                else if (open5PlayersRoom)
                                    OpenNewRoom(5, "False", "false");
                                break;
                            case 6:
                                if (open6PlayersRoom && openWithAIroom)
                                    OpenNewRoom(6, "False", "true");
                                else if (open6PlayersRoom)
                                    OpenNewRoom(6, "False", "false");

                                break;
                            default:
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                
            }

            autoOperRooms.ReleaseMutex();
        }

        [WebMethod]
        public static string RoomsUpdate()
        {            
            CRoom.UpdateRooms();
            return "OK";
        }

        protected void cbPlayers_CheckedChanged(object sender, EventArgs e)
        {
            open2PlayersRoom = cb2players.Checked;
            open3PlayersRoom = cb3players.Checked;
            open4PlayersRoom = cb4players.Checked;
            open5PlayersRoom = cb5players.Checked;
            open6PlayersRoom = cb6players.Checked;
            openWithAIroom = cbAutoStartWithAI.Checked;
        }
    }
}