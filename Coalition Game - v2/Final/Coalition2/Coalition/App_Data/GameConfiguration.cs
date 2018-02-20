using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coalition.App_Data
{
    public class GameConfiguration
    {
        public int MaxWaitTime { get; set; }
        public int MaxPlayedGames { get; set; }
        public int TimeInvervalSeconds { get; set; }
        public int ScoreIncrement { get; set; }
    }


}