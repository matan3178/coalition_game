using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coalition.App_Data
{
    public enum PlayerResponse {None, Accepted, Refused}

    public class PlayerRoundInfo
    {
        public PlayerResponse playerResponse;
        public double playerOffer;
        public int responseTime;
        public bool isAI;
        public bool isProposer;
        public double weight;

        public PlayerRoundInfo(bool isProposer,bool isAI,double weight)
        {
            this.isAI = isAI;
            this.isProposer = isProposer;
            this.weight = weight;
        }
    }
}