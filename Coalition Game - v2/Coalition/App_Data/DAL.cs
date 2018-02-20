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
        public static Dictionary<int, List<double[]>> _roomConfigurationsDictionary;

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
            _roomConfigurationsDictionary = new Dictionary<int, List<double[]>>();
            // Read everything
            string allText = ReadFileToString(_pathRoom);
            string type = allText.Substring(0,allText.IndexOf('\n'));
            string data = allText.Substring(allText.IndexOf('\n')+1);
            ConfigurationType.getInstance().setTypeFromString(type);
            Object[] json = (Object[])new JavaScriptSerializer().DeserializeObject(data);

            // Parse parts and push to configurations

            foreach (object[] item in json)
            {
                int size = (int)item[0];
                if (!_roomConfigurationsDictionary.ContainsKey(size))
                    _roomConfigurationsDictionary.Add(size, new List<double[]>());
                double[] values = new double[item.Length - 1];
                for (int i = 1; i < item.Length; i++)
                {
                    double value = double.Parse(item[i].ToString());
                    values[i-1] = value;
                }

                _roomConfigurationsDictionary[size].Add(values);
            }
        }

        public static double[] GetSingleRoomConfigurationForSize(int RoomSize)
        {
            Random random = new Random();
            int NumOfConfigurations = GetConfiguratinsCount(RoomSize);
            //for the server +1 if th counter start from 1
            int index = random.Next(0, NumOfConfigurations-1);
            return GetConfiguration(RoomSize, index);
        }

        private static double[] GetConfiguration(int RoomSize, int index)
        {
            return _roomConfigurationsDictionary[RoomSize][index];
        }

        private static int GetConfiguratinsCount(int roomSize)
        {
            if (_roomConfigurationsDictionary == null)
            {
                ReadRoomConfigurationFile();
            }
           return _roomConfigurationsDictionary[roomSize].Count;
        }

  
    }
}