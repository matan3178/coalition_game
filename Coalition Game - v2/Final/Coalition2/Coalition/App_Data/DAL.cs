using System;
using System.Collections.Generic;
using System.IO;
using Coalition.Properties;
using System.Web.Script.Serialization;
using Coalition.App_Data;
using System.Threading;
using System.Web;

namespace Coalition
{
    public class DAL
    {
        public static string _pathRoom = "C:\\Sites\\Coalition\\RoomConf.json";
        public static string _pathGame = "C:\\Sites\\Coalition\\GameConf.json";

        private static Mutex changeLock = new Mutex();

        //static string _connection = "";
        // weight p1, weight p2, AI is p1 distribution to p1, AI is p1 distribution to p2, AI is p2 distribution to p1, AI is p2 distribution to p2
        //,AI is p1 acceptence Threshold, ,AI is p2 acceptence Threshold, proposer timer, acceptance timer, number of rounds.B
        public static Dictionary<int, List<double[]>> _roomConfiguration;

        public static string ReadFileToString(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        public static bool WriteNewConfigurations(string configurations)
        {
            try
            {
                StreamWriter sw = new StreamWriter(_pathRoom,false) ;
                sw.Write(configurations);
                sw.Close();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool AppendConfigurations(string configurations)
        {
            try
            {
                StreamWriter sw = new StreamWriter(_pathRoom, true);
                sw.Write(configurations);
                sw.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        internal static void ReadGameConfigurationFile()
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

            string allText = ReadFileToString(_pathGame);
            var rawConf = new JavaScriptSerializer().DeserializeObject(allText);

            Dictionary<string, Object> gameConfs = (Dictionary<string, Object>)rawConf;

            GameConfiguration cg = new GameConfiguration();
            cg.TimeInvervalSeconds = (int)gameConfs["TimeInvervalSeconds"];
            cg.ScoreIncrement = (int)gameConfs["ScoreIncrement"];
            cg.MaxWaitTime = (int)gameConfs["MaxWaitTime"];
            cg.MaxPlayedGames = (int)gameConfs["MaxPlayedGames"];


            if (HttpContext.Current.Application["GameConfigurations"] == null)
                HttpContext.Current.Application["GameConfigurations"] = cg;
            
            changeLock.ReleaseMutex();
        }


        public static GameConfiguration GetGameSettings()
        {
            if (HttpContext.Current.Application["GameConfigurations"] == null)
            {
                ReadGameConfigurationFile();
            }
            
            return (GameConfiguration)HttpContext.Current.Application["GameConfigurations"];
        }

        public static void ReadRoomConfigurationFile()
        {
            _roomConfiguration = new Dictionary<int, List<double[]>>();
            // Read everything
            string allText = ReadFileToString(_pathRoom);
            Object[] json = (Object[])new JavaScriptSerializer().DeserializeObject(allText);

            // Parse parts and push to configurations

            foreach (object[] item in json)
            {
                int size = (int)item[0];
                if (!_roomConfiguration.ContainsKey(size))
                    _roomConfiguration.Add(size, new List<double[]>());
                double[] values = new double[item.Length - 1];
                for (int i = 1; i < item.Length; i++)
                {
                    double value = double.Parse(item[i].ToString());
                    values[i-1] = value;
                }

                _roomConfiguration[size].Add(values);
            }
        }

        public static double[] GetConfiguration(int RoomSize)
        {
            Random random = new Random();
            int NumOfConfigurations = GetConfiguratinsCount(RoomSize);
            //for the server +1 if th counter start from 1
            int index = random.Next(0, NumOfConfigurations-1);
            return GetConfiguration(RoomSize, index);
        }

        private static double[] GetConfiguration(int RoomSize, int index)
        {
            return _roomConfiguration[RoomSize][index];
        }

        private static int GetConfiguratinsCount(int roomSize)
        {
            if (_roomConfiguration == null)
            {
                ReadRoomConfigurationFile();
            }
           return _roomConfiguration[roomSize].Count;
        }

  
    }
}