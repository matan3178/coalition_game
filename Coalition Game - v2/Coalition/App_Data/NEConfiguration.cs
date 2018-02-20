using Coalition.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Coalition
{
    public class NEConfiguration : Configuration
    {

        //public double[] weights;
        //public double[][] NEDevision;
        ////public double[] AcceptnaceRateAi;
        //public double ProposerTimeout;
        //public double PlayerTimeout;
        ////public int RoundsNumber;
        //public int numOfRounds;

        public NEConfiguration(int numOfPlayers)
        {
            double[] rawData = DAL.GetSingleRoomConfigurationForSize(numOfPlayers);
            weights = new double[numOfPlayers];
            //AcceptnaceRateAi = new double[size];
            numOfRounds = (int)(rawData[0]);
            AiDevision = new double[numOfRounds][];            
            int index = 1;
            for (; index <= numOfPlayers; index++)
                weights[index-1] = rawData[index];

            for (int i = 0; i < numOfRounds; i++)
            {
                AiDevision[i] = new double[numOfPlayers];
                for (int j = 0; j < numOfPlayers; j++)
                {
                    AiDevision[i][j] = rawData[index];
                    index++;
                }
                //for (; index < (i + 2) * numOfPlayers; index++)
                //    AiDevision[i][index % numOfPlayers] = rawData[index];
            }
            //for (; index < rawData.Length - 3; index++)
            //    AcceptnaceRateAi[index % numOfPlayers] = rawData[index];

            ProposerTimeout = rawData[index++];
            PlayerTimeout = rawData[index];
        }

        //public double[] GetAiSplit(int index)
        //{
        //    return NEDevision[index];
        //}

        //public bool AcceptOffer(int index, double[] offer)
        //{
        //    if( offer[index] < AcceptnaceRateAi[index])
        //        return false;
        //    return true;
        //}

    }
}