using Coalition.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Coalition
{
    public abstract class Configuration
    {

        public double[] weights;
        public double[][] AiDevision;
        public double[] AcceptnaceRateAi;
        public double ProposerTimeout;
        public double PlayerTimeout;
        public int numOfRounds;

    }
}