using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coalition.App_Data
{
    public class RoundInfo
    {
        public Dictionary<string, PlayerRoundInfo> playersRoundInfo = new Dictionary<string, PlayerRoundInfo>();
        public static Random randGen = new Random();

        public RoundInfo(double[] weights, string[] playersHash, int proposerID)
        {
            for (int i = 0; i < playersHash.Length; i++)
            {
                if (playersHash[i] == "AI")
                    playersRoundInfo.Add(playersHash[i], new PlayerRoundInfo(i == proposerID, true, weights[i]));
                else
                    playersRoundInfo.Add(playersHash[i], new PlayerRoundInfo(i == proposerID, false, weights[i]));
            }
        }

        public void SetOffers(Dictionary<string, double> offers)
        {
            foreach (var offer in offers)
            {
                if (playersRoundInfo.ContainsKey(offer.Key))
                    playersRoundInfo[offer.Key].playerOffer = offer.Value;
            }
        }

        public void SetResponse(string hashPlayer, PlayerResponse response)
        {
            if (playersRoundInfo.ContainsKey(hashPlayer))
                playersRoundInfo[hashPlayer].playerResponse = response;
        }

        internal List<InGamePlayer> GetPlayers(string requesterPlayerHash)
        {
            List<InGamePlayer> players = new List<InGamePlayer>();
            foreach (var playerInfo in playersRoundInfo)
            {
                InGamePlayer igp = new InGamePlayer();
                if (requesterPlayerHash == playerInfo.Key)
                {
                    igp.nick = "You";
                } else
                {
                    igp.nick = "Anonymos";
                }
                igp.hash = playerInfo.Key;
                igp.offer = playerInfo.Value.playerOffer.ToString();
                if (playerInfo.Value.isProposer == true)
                {
                    igp.role = "Proposer";
                }
                igp.weight = playerInfo.Value.weight.ToString();
                players.Add(igp);
            }
            return players;
        }
    }
}