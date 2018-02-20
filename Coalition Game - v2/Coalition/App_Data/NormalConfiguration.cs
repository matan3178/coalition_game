using Coalition.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Coalition
{
    public class NormalConfiguration : Configuration
    {

        //public double[] weights;
        //public double[][] AiDevision;
        //public double[] AcceptnaceRateAi;
        //public double ProposerTimeout;
        //public double PlayerTimeout;
        //public int RoundsNumber;

        public NormalConfiguration(int size)
        {
            //double[] configuration=   DAL.GetConfiguration(size);
            double[] configuration = DAL.GetSingleRoomConfigurationForSize(size);
            weights = new double[size];
            AcceptnaceRateAi = new double[size];
            AiDevision = new double[size][];

            int index = 0;
            for (; index < size; index++)
                weights[index] = configuration[index];


            for (int i = 0; i < size; i++)
            {
                AiDevision[i] = new double[size];
                for (; index < (i + 2) * size; index++)
                    AiDevision[i][index % size] = configuration[index];
            }
            for (; index < configuration.Length - 3; index++)
                AcceptnaceRateAi[index % size] = configuration[index];

            ProposerTimeout = configuration[configuration.Length - 3];
            PlayerTimeout = configuration[configuration.Length - 2];
            numOfRounds = int.Parse(configuration[configuration.Length - 1].ToString());
        }


    }
}