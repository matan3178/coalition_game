using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace Coalition.App_Data
{
    // possible states of a room
    enum RoomState {Init, ProposalMaking, OffersInspection, Summary }

    public class CRoom
    {
        static Random randGenerator = new Random();
        RoomState roomState = RoomState.Init;
        Configuration configuration;
        DateTime? NextRoundTimeOut = null;
        DateTime creationTime = DateTime.Now;
        DateTime stateChangeTime = DateTime.Now;
        Dictionary<int,RoundInfo> roundsInfo = new Dictionary<int, RoundInfo>();
        int numberOfPlayers;
        bool AIparticipating = false;
        public string roomHash;
        int currentRound = 0;
        private static Mutex changeLock = new Mutex();

        public CRoom(string[] hashPlayers,bool participatingAI) 
        {
            if (participatingAI)
            {
                numberOfPlayers = hashPlayers.Length + 1;
            } else
            {
                numberOfPlayers = hashPlayers.Length;
            }

            using (MD5 md5Hash = MD5.Create())
            {
                roomHash = GetMd5Hash(md5Hash, numberOfPlayers + creationTime.ToFileTime() + "");
            }

            configuration = new Configuration(numberOfPlayers);


            string[] scrumbledPlayers = ScrumblePlayers(hashPlayers);
            if (configuration.weights.Length == hashPlayers.Length + 1)
            {
                scrumbledPlayers = AddAIToPlayers(scrumbledPlayers);
                AIparticipating = true;
            }

            for (int i = 0; i < configuration.RoundsNumber; i++)
            {
                int proposerID = randGenerator.Next(numberOfPlayers);
                RoundInfo ri = new RoundInfo(configuration.weights, scrumbledPlayers, proposerID);
                roundsInfo.Add(i,ri);
            }

            AddRoom(this);
        }

        private string[] AddAIToPlayers(string[] scrumbledPlayers)
        {
            string[] newPlayers = new string[scrumbledPlayers.Length + 1];
            int AIIndex = randGenerator.Next(scrumbledPlayers.Length + 1);
            int playersIndex = 0;
            for (int i = 0; i <= scrumbledPlayers.Length; i++)
            {
                if (i == AIIndex)
                {
                    newPlayers[i] = "AI";
                }
                else
                {
                    newPlayers[i] = scrumbledPlayers[playersIndex];
                    playersIndex++;
                }
            }
            return newPlayers;
        }

        internal static void CloseRoom(string roomHash)
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
            var online = HttpContext.Current.Application["OnlineRooms"];
            if (online == null)
            {
                online = new Dictionary<string, CRoom>();
            }

            var OnlineRooms = (Dictionary<string, CRoom>)online;

            if (OnlineRooms.ContainsKey(roomHash))
            {
                OnlineRooms.Remove(roomHash);
                HttpContext.Current.Application["OnlineRooms"] = OnlineRooms;
            }

            changeLock.ReleaseMutex();

        }
    

        public bool UpdatePlayerResponse(string roomHash, string playerHash, string response)
        {
            bool answer = false;
            try
            {
                changeLock.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                changeLock.ReleaseMutex();
                changeLock.WaitOne();
            }
            var rooms = HttpContext.Current.Application["OnlineRooms"];
            if (rooms != null)
            {
                var onlineRooms = (Dictionary<string, CRoom>)rooms;
                onlineRooms[roomHash].roundsInfo[currentRound].playersRoundInfo[playerHash].responseTime = (int)(DateTime.Now - onlineRooms[roomHash].stateChangeTime).TotalSeconds;
                switch (response)
                {
                    case "Accept":
                        onlineRooms[roomHash].roundsInfo[currentRound].playersRoundInfo[playerHash].playerResponse = PlayerResponse.Accepted;
                        answer = true;
                        break;
                    case "Refuse":
                        answer = true;
                        onlineRooms[roomHash].roundsInfo[currentRound].playersRoundInfo[playerHash].playerResponse = PlayerResponse.Refused;
                        break;
                    default:
                        Dictionary<string, object> x = (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(response);
                        foreach (var item in onlineRooms[roomHash].roundsInfo[currentRound].playersRoundInfo)
                        {
                            item.Value.playerOffer = double.Parse(x[item.Key].ToString());
                            item.Value.playerResponse = PlayerResponse.None;
                        }
                        onlineRooms[roomHash].roundsInfo[currentRound].playersRoundInfo[playerHash].playerResponse = PlayerResponse.Accepted;
                        answer = true;
                        break;
                }
                HttpContext.Current.Application["OnlineRooms"] = onlineRooms;
            }

            changeLock.ReleaseMutex();
            return answer;
        }

        public string GetGameResponses()
        {
            List<Tuple<double, string, string, bool>> responses = new List<Tuple<double, string, string, bool>>();

            foreach (var item in roundsInfo[currentRound].playersRoundInfo)
            {
                responses.Add(new Tuple<double, string, string, bool>(item.Value.playerOffer,
                    item.Value.playerResponse.ToString(), item.Key, item.Value.isProposer));
            }

            var json = new JavaScriptSerializer().Serialize(responses.ToArray());
            return json;
        }

        public void UpdatePlayerRoles(string hashPlayer)
        {
            var player = roundsInfo[currentRound].playersRoundInfo[hashPlayer];
            if (player.isProposer)
                Player.SetFutureStatus(hashPlayer, Player.Status.Proposing);
            else
                Player.SetFutureStatus(hashPlayer, Player.Status.WaitingForProposal);
        }

        public void UpdateAllPlayersRoles()
        {
            foreach (var item in roundsInfo[currentRound].playersRoundInfo)
            {
                UpdatePlayerRoles(item.Key);
            }
        }

        public void UpdatePlayerRolesToSummery(string hashPlayer,bool formed,bool included)
        {
            if (formed) {
                if (included)
                    Player.SetFutureStatus(hashPlayer, Player.Status.GameFinishedCoalitionFormedInc);
                else
                    Player.SetFutureStatus(hashPlayer, Player.Status.GameFinishedCoalitionFormedNotInc);
            }
            else
                Player.SetFutureStatus(hashPlayer, Player.Status.GameFinishedCoalitionNotFormed);


        }

        public void UpdateAllPlayersToSummary(bool formed)
        {
            foreach (var item in roundsInfo[currentRound].playersRoundInfo)
            {
                if (item.Value.playerOffer > 0)
                    UpdatePlayerRolesToSummery(item.Key, formed, true);
                else
                    UpdatePlayerRolesToSummery(item.Key, formed, false);
            }
        }
        public bool AddRoom(CRoom room)
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

            var online = HttpContext.Current.Application["OnlineRooms"];
            if (online == null)
            {
                online = new Dictionary<string, CRoom>();
            }

            var OnlineRooms = (Dictionary<string, CRoom>)online;

            if (!OnlineRooms.ContainsKey(roomHash))
            {
                OnlineRooms.Add(roomHash, room);
                HttpContext.Current.Application["OnlineRooms"] = OnlineRooms;
            }
            else
                answer = false;
            changeLock.ReleaseMutex();
            return answer;
        }


        public double[] GetGameSettings()
        {
            double[] answer = new double[3];
            int passedTime = (int)(DateTime.Now - stateChangeTime).TotalSeconds;
            answer[0] = configuration.PlayerTimeout - passedTime;
            answer[1] = configuration.ProposerTimeout - passedTime;
            answer[2] = configuration.RoundsNumber - currentRound;
            return answer;
        }
        

        public void SetOffers(Dictionary<string,double> offers)
        {
            roundsInfo[currentRound].SetOffers(offers);
        }

        public void SetResponse(string hashPlayer,PlayerResponse response)
        {
            roundsInfo[currentRound].SetResponse(hashPlayer, response);
        }

        public List<InGamePlayer> GetAllGamesPlayers(string playerhash)
        {
            List<InGamePlayer> players = new List<InGamePlayer>();
            RoundInfo ri = roundsInfo[currentRound];
            List<InGamePlayer> roundPlayers = ri.GetPlayers(playerhash);
            return roundPlayers;
        }

        public static CRoom GetRoom(string hashRoom)
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
            CRoom answer = null;
            var rooms = HttpContext.Current.Application["OnlineRooms"];
            if (rooms != null)
            {
                var onlineRooms = (Dictionary<string, CRoom>)rooms;
                if (onlineRooms.ContainsKey(hashRoom))
                    answer = onlineRooms[hashRoom];
            }
            changeLock.ReleaseMutex();
            return answer;
        }

       
        public static void UpdateRooms()
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

            var rooms = HttpContext.Current.Application["OnlineRooms"];
            if (rooms != null)
            {
                var onlineRooms = (Dictionary<string, CRoom>)rooms;
                List<string> roomsToBeRemoved = new List<string>();
                int timeInTheCurrentState;
                bool coalitionHasBeenFormed = false;

                RoundInfo ri;
                foreach (var room in onlineRooms)
                {
                    if (room.Value.NextRoundTimeOut != null)
                    {
                        if (room.Value.NextRoundTimeOut < DateTime.Now)
                        {
                            if (!room.Value.NextRound())
                                roomsToBeRemoved.Add(room.Key);
                            room.Value.NextRoundTimeOut = null;
                        }
                    }
                    else {
                        switch (room.Value.roomState)
                        {
                            case RoomState.Init:
                                ri = room.Value.roundsInfo[room.Value.currentRound];
                                int realNumberOfPlayers = room.Value.AIparticipating ? room.Value.numberOfPlayers - 1 : room.Value.numberOfPlayers;
                                bool everyoneIsInGame = Player.isEveryonePlayingInRoom(room.Key, realNumberOfPlayers);
                                if (everyoneIsInGame)
                                {
                                    room.Value.stateChangeTime = DateTime.Now;
                                    room.Value.roomState = RoomState.ProposalMaking;
                                }
                                break;
                            case RoomState.ProposalMaking:
                                TimeSpan ts = DateTime.Now - room.Value.stateChangeTime;
                                timeInTheCurrentState = (int)ts.TotalSeconds;
                                if ((room.Value.configuration.ProposerTimeout - timeInTheCurrentState) <= 0)
                                {
                                    room.Value.UpdateAllPlayersToSummary(false);
                                    room.Value.NextRoundTimeOut = DateTime.Now.AddSeconds(5);
                                }

                                // play the AI
                                ri = room.Value.roundsInfo[room.Value.currentRound];
                                int aiIndex = 0;
                                foreach (var item in ri.playersRoundInfo)
                                {
                                    if (item.Value.isAI && item.Value.isProposer && timeInTheCurrentState>=10)
                                    {
                                        double[] offers = room.Value.configuration.AiDevision[aiIndex];
                                        int playersindex = 0;
                                        foreach (var item2 in ri.playersRoundInfo)
                                        {
                                            item2.Value.playerOffer = offers[playersindex];
                                            item2.Value.playerResponse = PlayerResponse.None;
                                            playersindex++;
                                        }
                                        break;
                                    }
                                    aiIndex++;
                                }

                                if (room.Value.OffersWereGiven())
                                {
                                    room.Value.stateChangeTime = DateTime.Now;
                                    room.Value.roomState = RoomState.OffersInspection;

                                    ri = room.Value.roundsInfo[room.Value.currentRound];
                                    foreach (var item in ri.playersRoundInfo)
                                    {
                                        if (item.Value.isProposer)
                                            Player.SetFutureStatus(item.Key, Player.Status.ProposerWaitingForAnswer);
                                        else if (item.Value.playerOffer > 0)
                                            Player.SetFutureStatus(item.Key, Player.Status.ProposalInspection);
                                        else
                                            Player.SetFutureStatus(item.Key, Player.Status.WaitingNotGivenOffer);
                                    }
                                }
                                break;
                            case RoomState.OffersInspection:
                                timeInTheCurrentState = (int)(DateTime.Now - room.Value.stateChangeTime).TotalSeconds;
                                if ((room.Value.configuration.PlayerTimeout - timeInTheCurrentState) <= 0)
                                {
                                    room.Value.UpdateAllPlayersToSummary(false);
                                    room.Value.NextRoundTimeOut = DateTime.Now.AddSeconds(5);
                                }

                                // play as the AI
                                ri = room.Value.roundsInfo[room.Value.currentRound];
                                aiIndex = 0;
                                foreach (var item in ri.playersRoundInfo)
                                {
                                    if (item.Value.isAI && item.Value.playerOffer>0 &&item.Value.playerResponse == PlayerResponse.None && timeInTheCurrentState >= 5)
                                    {
                                        double acceptance = room.Value.configuration.AcceptnaceRateAi[aiIndex];
                                        if (item.Value.playerOffer > acceptance)
                                        {
                                            item.Value.playerResponse = PlayerResponse.Accepted;
                                        } else
                                        {
                                            item.Value.playerResponse = PlayerResponse.Refused;
                                        }
                                        break;
                                    }
                                    aiIndex++;
                                }



                                bool everyOneResponded = true;
                                ri = room.Value.roundsInfo[room.Value.currentRound];
                                foreach (var item in ri.playersRoundInfo)
                                {
                                    if (item.Value.playerResponse == PlayerResponse.None)
                                        if (item.Value.playerOffer > 0)
                                            everyOneResponded = false;
                                        else
                                            Player.SetFutureStatus(item.Key, Player.Status.WaitingNotGivenOffer);
                                    else
                                    {
                                        if (!item.Value.isProposer)
                                        {
                                            //item.Value.responseTime = timeInTheCurrentState;
                                            Player.SetFutureStatus(item.Key, Player.Status.FinishedRespondingWaiting);
                                        }
                                    }
                                }
                                if (everyOneResponded)
                                {
                                    room.Value.roomState = RoomState.Summary;
                                    coalitionHasBeenFormed = room.Value.isCoalitionFormed();
                                    room.Value.UpdateAllPlayersToSummary(coalitionHasBeenFormed);
                                    room.Value.NextRoundTimeOut = DateTime.Now.AddSeconds(5);
                                }
                                break;
                            case RoomState.Summary:

                                break;
                            default:
                                break;
                        }
                    }
                }
                foreach (var item in roomsToBeRemoved)
                {
                    onlineRooms.Remove(item);
                }
                HttpContext.Current.Application["OnlineRooms"] = onlineRooms;
            }
            changeLock.ReleaseMutex();
        }


        private bool OffersWereGiven()
        {
            RoundInfo ri = roundsInfo[currentRound];
            bool offersGiven = false;
            foreach (var item in ri.playersRoundInfo)
            {
                if (item.Value.playerOffer > 0)
                    offersGiven = true;
            }
            return offersGiven;
        }
        private bool isCoalitionFormed()
        {
            bool coalitionHasBeenFormed = true;
            RoundInfo ri = roundsInfo[currentRound];
            int weightSum = 0;
            foreach (var item in ri.playersRoundInfo)
            {
                if (item.Value.playerOffer > 0)
                    if (item.Value.playerResponse != PlayerResponse.Accepted)
                        coalitionHasBeenFormed = false;
                    else
                        weightSum += (int)item.Value.weight;
            }
            if (weightSum < 10)
            {
                coalitionHasBeenFormed = false;
            }

            return coalitionHasBeenFormed;
        }
        private bool NextRound()
        {
            WriteLog();

            bool coalitionHasBeenFormed = isCoalitionFormed();
            
            if (coalitionHasBeenFormed)
            {
                foreach (var item in roundsInfo[currentRound].playersRoundInfo)
                {
                    Player.AddScore(item.Key, item.Value.playerOffer);
                    Player.AddGame(item.Key, item.Value.playerOffer);
                    Player.SetFutureStatus(item.Key, Player.Status.WaitingRoom);
                    Player.ResetEntranceTime(item.Key);
                }
                return false;
            } else
            {
                if (currentRound >= configuration.RoundsNumber - 1)
                { 
                    foreach (var item in roundsInfo[currentRound].playersRoundInfo)
                    {
                        Player.SetFutureStatus(item.Key, Player.Status.WaitingRoom);
                        Player.ResetEntranceTime(item.Key);
                    }
                    return false;
                }
            }

            RoundInfo ri = roundsInfo[currentRound];
            foreach (var item in ri.playersRoundInfo)
            {
                if (!Player.isPlayerConnected(item.Key))
                {
                    foreach (var item2 in roundsInfo[currentRound].playersRoundInfo)
                    {                        
                        Player.SetFutureStatus(item2.Key, Player.Status.WaitingRoom);
                        Player.ResetEntranceTime(item2.Key);
                    }
                    return false;
                }               
            }

            currentRound++;
            stateChangeTime = DateTime.Now;
            roomState = RoomState.ProposalMaking;
            UpdateAllPlayersRoles();
            return true;
        }

        private void WriteLog()
        {

            int proposerIndex = 0;
            int aiIndex = -1;
            PlayerRoundInfo[] priPlayers = roundsInfo[currentRound].playersRoundInfo.Values.ToArray();
            String[] playersHash = roundsInfo[currentRound].playersRoundInfo.Keys.ToArray();

            int[] weights = new int[priPlayers.Length];
            int[] shares = new int[priPlayers.Length];
            string[] decisions = new string[priPlayers.Length];
            int[] timeouts = new int[priPlayers.Length];
            for (int i = 0; i < priPlayers.Length; i++)
            {
                if (priPlayers[i].isAI) aiIndex = i;
                if (priPlayers[i].isProposer) proposerIndex = i;
                weights[i] = (int)priPlayers[i].weight;
                shares[i] = (int)priPlayers[i].playerOffer;
                decisions[i] = priPlayers[i].playerResponse.ToString();
                timeouts[i] = priPlayers[i].responseTime;
            }

            WriteToDB.WriteProposalToDB(playersHash, currentRound, roomHash, numberOfPlayers, proposerIndex, aiIndex, weights, shares, decisions, timeouts);

        }

        private static string[] ScrumblePlayers(string[] hashPlayers)
        {
             return hashPlayers.OrderBy(x => randGenerator.Next()).ToArray();
        }
        private static string GetMd5Hash(MD5 md5Hash, string input)
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


    }
}