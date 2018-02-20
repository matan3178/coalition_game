using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace Coalition.App_Data
{
    public class Player
    {
        public enum Status { Home, WaitingRoom, WaitingForJoinResponse, Ready, EnteringRoom, Proposing, FinishedRespondingWaiting, GameFinishedCoalitionNotFormed, GameFinishedCoalitionFormedInc, GameFinishedCoalitionFormedNotInc, EndGameScreen, WaitingNotGivenOffer, ProposalInspection, ProposerWaitingForAnswer, WaitingForProposal, None, ExitingGame }; // divs

        internal static void WriteToFile(string hashPlayer)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            Dictionary<string, Player> onlinePlayers = (Dictionary<string, Player>)online;

            foreach (var item in onlinePlayers)
            {
                if (item.Value.HashPlayer == hashPlayer)
                {
                    WriteToDB.WriteFinishedPlayer(((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key]);
                    break;
                }
            }
            changeLock.ReleaseMutex();
        }

        public enum Connection { Connected, Disconneced }; // status conneced

        private static Mutex changeLock = new Mutex();

        private static int playerCounter = 0;

        public string Nickname;
        public DateTime EntranceTime;
        public string HashPlayer;
        public DateTime UserNameTimeCreated;
        public DateTime UserNameTimeErased;
        public string roomHash;
        public Status status;
        public Connection conStat;
        public DateTime LastSeen;
        public Status futureStatus;
        public double SecondsInGame;
        public string workerID;
        public string assID;
        public string hitID;
        public int PlayScore;
        public int WaitScore;
        public int NumberOfGames;
        public int TimeWaitedSeconds;
        public int MaxWaitingTime = 0;
        public int MaxGamesNumber = 0;
        public int TimeInvervalSecondsScoreIncrement = 0;
        public int ScoreIncrement = 0;

        internal static string GetScore(string hash)
        {
            string answer = "0";
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (!(online == null || !(online is Dictionary<string, Player>)))
            {
                Dictionary<string, Player> players = (Dictionary<string, Player>)online;

                foreach (var player in players)
                {
                    if (player.Value.HashPlayer == hash)
                    {
                        int totalScore = player.Value.PlayScore + player.Value.WaitScore;
                        answer = totalScore.ToString();
                        break;
                    }
                }
            }

            changeLock.ReleaseMutex();
            return answer;
        }

        internal static string GetNumberOfGames(string hash)
        {
            string answer = "0";
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (!(online == null || !(online is Dictionary<string, Player>)))
            {
                Dictionary<string, Player> players = (Dictionary<string, Player>)online;

                foreach (var player in players)
                {
                    if (player.Value.HashPlayer == hash)
                    {
                        answer = player.Value.NumberOfGames.ToString();
                        break;
                    }
                }
            }

            changeLock.ReleaseMutex();
            return answer;
        }



        public Player(string nick)
        {
            playerCounter++;
            using (MD5 md5Hash = MD5.Create())
            {
                HashPlayer = GetMd5Hash(md5Hash, nick + DateTime.Now.ToFileTime());
            }
            Nickname = nick;
            status = Status.WaitingRoom;
            conStat = Connection.Connected;
            EntranceTime = DateTime.Now;
            UserNameTimeCreated = DateTime.Now;
            LastSeen = DateTime.Now;
            AddPlayer(this);
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static void RemovePlayer(string Nickname)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            if (((Dictionary<string, Player>)online).ContainsKey(Nickname))
            {
                ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"]).Remove(Nickname);
            }

            changeLock.ReleaseMutex();
        }

        internal void UpdateTimeWaited(int dt)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            if (((Dictionary<string, Player>)online).ContainsKey(Nickname))
            {
                ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[Nickname].TimeWaitedSeconds += dt;

                ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[Nickname].UpdateWaitedScore();

            }

            changeLock.ReleaseMutex();
        }

        private void UpdateWaitedScore()
        {
            GameConfiguration gc = DAL.GetGameSettings();
            int timeInterval = gc.TimeInvervalSeconds;
            int scoreAddition = gc.ScoreIncrement;
            int waitedInseconds = TimeWaitedSeconds / 1000;
            WaitScore = waitedInseconds / timeInterval * scoreAddition;
        }

        public static bool isEveryonePlayingInRoom(string roomHash,int numberOfPlayers)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = true;
            int counter = 0;
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null || !(online is Dictionary<string, Player>))
                answer = false;
            else
            {
                Dictionary<string, Player> players = (Dictionary<string, Player>)online;
                
                foreach (var player in players)
                {
                    if (player.Value.roomHash == roomHash)
                    {
                        if (player.Value.status == Status.Home ||
                            player.Value.status == Status.WaitingRoom ||
                            player.Value.status == Status.WaitingForJoinResponse ||
                            player.Value.status == Status.Ready ||
                            player.Value.status == Status.EnteringRoom)
                        {
                            answer = false;
                            break;
                        }
                             else
                        {
                            counter++;
                        }
                    }
                }
            }
            if (counter != numberOfPlayers)
                answer = false;

            changeLock.ReleaseMutex();

            return answer;
        }

        public bool SetFutureStatus(Status status,bool fullReset)
        {
            try
            {               
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            if (((Dictionary<string, Player>)online).ContainsKey(Nickname))
            {
                (((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[Nickname]).futureStatus = status;
                if (status == Status.WaitingRoom)
                {
                    (((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[Nickname]).roomHash = "";
                    if (fullReset)
                    {
                        (((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[Nickname]).EntranceTime = DateTime.Now;
                    }
                }
                answer = true;
            }
            changeLock.ReleaseMutex();
            return answer;
        }

        public static Player GetPlayer(string nickname)
        {
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
                return null;
            var onlinePlayers = (Dictionary<string, Player>)online;
            if (onlinePlayers.ContainsKey(nickname))
                return onlinePlayers[nickname];
            return null;
        }

        public static bool isPlayerConnected(string hash)
        {
            if (hash == "AI")
                return true;
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }
            var onlinePlayers = (Dictionary<string, Player>)online;
            foreach (var item in onlinePlayers)
            {
                if (item.Value.HashPlayer == hash)
                    answer = true;
            }
            changeLock.ReleaseMutex();
            return answer;
        }

        public bool AddPlayer(Player player)
        {
            try {
                changeLock.WaitOne();
            } catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = true;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            var onlinePlayers = (Dictionary<string, Player>)online;

            if (!onlinePlayers.ContainsKey(player.Nickname))
            {
                onlinePlayers.Add(player.Nickname, player);
                HttpContext.Current.Application["OnlinePlayers"] = onlinePlayers;
            }
            else
                answer = false;
            changeLock.ReleaseMutex();
            return answer;
        }
        public static string GetDestination(string hash,string currentScreen)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            string answer = "NotConnected";

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null || !(online is Dictionary<string, Player>))
                answer = "OK";
            else
            {
                Dictionary<string, Player> players = (Dictionary<string, Player>)online;

                foreach (var player in players)
                {
                    if (player.Value.HashPlayer == hash)
                    {
                        player.Value.status = (Status)Enum.Parse(typeof(Status), currentScreen);
                        if (player.Value.futureStatus != player.Value.status)
                            answer = player.Value.futureStatus.ToString();
                        else
                            answer = "OK";
                        break;
                    }
                }
            }

            changeLock.ReleaseMutex();

            return answer;
        }
        public static string GetRoomHash(string playerHash)
        {
            string answer = "";
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null || !(online is Dictionary<string, Player>))
                answer = "";
            else
            {
                 Dictionary<string, Player> players = (Dictionary<string, Player>)online;
                 Player[] playersValues = new Player[players.Count];
                 players.Values.CopyTo(playersValues, 0);
                 foreach (var player in playersValues)
                  {
                    if (player.HashPlayer == playerHash)
                    {
                        answer =  player.roomHash;
                    }
                  }  
            }
            return answer;
        }
        public static void StillAlive(string hash)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (!(online == null || !(online is Dictionary<string, Player>)))
            {
                Dictionary<string, Player> players = (Dictionary<string, Player>)online;

                foreach (var player in players)
                {
                    if (player.Value.HashPlayer == hash)
                    {
                        player.Value.LastSeen = DateTime.Now;
                        break;
                    }
                }
            }

            changeLock.ReleaseMutex();
        }


        public bool AddPlayerToRoom(string roomHash)
        {
            this.roomHash = roomHash;
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }
            
            if (((Dictionary<string, Player>)online).ContainsKey(Nickname))
            {
                ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[Nickname] = this;
                answer = true;
            }

            changeLock.ReleaseMutex();
            return answer;
        }

        public static Player[] GetAllPlayers()
        {
            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }
            var onlinePlayers = (Dictionary<string, Player>)online;

            Player[] allPlayers = new Player[onlinePlayers.Count];
            onlinePlayers.Values.CopyTo(allPlayers, 0);
            return allPlayers;
        
        }
        public bool RemovePlayer()
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];

            if (online != null)
            {
                var onlinePlayers = (Dictionary<string, Player>)online;
                if (onlinePlayers.ContainsKey(Nickname))
                {
                    ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"]).Remove(Nickname);
                    answer = true;
                }
            }
            changeLock.ReleaseMutex();
            return answer;

        }

        internal static bool SetFutureStatus(string hashPlayer, Status status)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            Dictionary<string, Player> onlinePlayers = (Dictionary<string, Player>)online;

            foreach (var item in onlinePlayers)
            {
                if (item.Value.HashPlayer == hashPlayer)
                {
                    item.Value.futureStatus = status;
                    ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key].futureStatus = status;
                    answer = true;
                    break;
                }
            }

            changeLock.ReleaseMutex();
            return answer;
        }

        internal static bool setNickNameFutureStatus(string nickname, Status status)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }

            Dictionary<string, Player> onlinePlayers = (Dictionary<string, Player>)online;

            foreach (var item in onlinePlayers)
            {
                if (item.Value.Nickname == nickname)
                {
                    item.Value.futureStatus = status;

                    /*if (item.Value.futureStatus == Status.ExitingGame)
                    {
                        ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key].UserNameTimeErased = DateTime.Now;
                        ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key].SecondsInGame =
                            (DateTime.Now -
                            ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key].UserNameTimeCreated).TotalSeconds;
                        WriteToDB.WriteFinishedPlayer(((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key]);
                    }*/
                    ((Dictionary<string, Player>)HttpContext.Current.Application["OnlinePlayers"])[item.Key].futureStatus = status;
                    answer = true;
                    break;
                }
            }

            changeLock.ReleaseMutex();
            return answer;
        }

        internal static void AddScore(string key, double playerOffer)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }
            Dictionary<string, Player> onlinePlayers = (Dictionary<string, Player>)online;
            foreach (var item in onlinePlayers)
            {
                if (item.Value.HashPlayer == key)
                {
                    onlinePlayers[item.Key].PlayScore += (int)playerOffer;
                }
            }
            HttpContext.Current.Application["OnlinePlayers"] = onlinePlayers;
            changeLock.ReleaseMutex();
        }

        internal static void ResetEntranceTime(string key)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }
            Dictionary<string, Player> onlinePlayers = (Dictionary<string, Player>)online;
            foreach (var item in onlinePlayers)
            {
                if (item.Value.HashPlayer == key)
                {
                    onlinePlayers[item.Key].EntranceTime = DateTime.Now;
                }
            }
            HttpContext.Current.Application["OnlinePlayers"] = onlinePlayers;
            changeLock.ReleaseMutex();
        }

        internal static void AddGame(string key, double playerOffer)
        {
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            bool answer = false;

            var online = HttpContext.Current.Application["OnlinePlayers"];
            if (online == null)
            {
                online = new Dictionary<string, Player>();
            }
            Dictionary<string, Player> onlinePlayers = (Dictionary<string, Player>)online;
            foreach (var item in onlinePlayers)
            {
                if (item.Value.HashPlayer == key)
                {
                    onlinePlayers[item.Key].NumberOfGames++;
                }
            }
            HttpContext.Current.Application["OnlinePlayers"] = onlinePlayers;
            changeLock.ReleaseMutex();

        }
    }
}