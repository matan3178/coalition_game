using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Coalition
{
    public class DAL
    {
        static string _path = Path.Combine(@"C:\Users\dshemary\Downloads\kobi\Coalition\Configurations.csv");

        //static string _connection = "";
        // weight p1, weight p2, AI is p1 distribution to p1, AI is p1 distribution to p2, AI is p2 distribution to p1, AI is p2 distribution to p2
        //,AI is p1 acceptence Threshold, ,AI is p2 acceptence Threshold, proposer timer, acceptance timer, number of rounds.B
        public static Dictionary<int, List<double[]>> _configuration;

        public static string ReadFileToString()
        {
            StreamReader sr = new StreamReader(_path);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        public static bool WriteNewConfigurations(string configurations)
        {
            try
            {
                StreamWriter sw = new StreamWriter(_path, false);
                sw.Write(configurations);
                sw.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool AppendConfigurations(string configurations)
        {
            try
            {
                StreamWriter sw = new StreamWriter(_path, true);
                sw.Write(configurations);
                sw.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static void ReadFile()
        {
            _configuration = new Dictionary<int, List<double[]>>();
            StreamReader sr = new StreamReader(_path);
            var lines = new List<double[]>();
            int Row = 0;
            char[] n = { '\\', '\"', ',' };
            int indexRow = 0;
            while (!sr.EndOfStream)
            {
                indexRow++;
                string str = sr.ReadLine();
                str = str.Trim(n);
                if (str != "")
                {
                    string[] Line = str.Split(',');
                    double[] doubleLine = new double[Line.Length];
                    for (int i = 0; i < Line.Length; i++)
                    {
                        double d = Double.Parse(Line[i]);
                        doubleLine[i] = d;
                    }
                    int ConfigutaionSize = 0;
                    for (int i = 1; i < 10; i++)
                    {
                        if (i + i * i + i + 3 == doubleLine.Length)
                            ConfigutaionSize = i;
                    }
                    if (ConfigutaionSize == 0)
                        throw new Exception("Configuration format invalid in row" + indexRow);
                    if (!_configuration.ContainsKey(ConfigutaionSize))
                        _configuration.Add(ConfigutaionSize, new List<double[]>());
                    _configuration[ConfigutaionSize].Add(doubleLine);

                    Row++;
                }
            }
            sr.Close();
        }
        public static double[] GetConfiguration(int RoomSize)
        {
            Random random = new Random();
            int NumOfConfigurations = GetConfiguratinsCount(RoomSize);
            //for the server +1 if th counter start from 1
            int index = random.Next(1, NumOfConfigurations);
            return GetConfiguration(RoomSize, index);
        }

        private static double[] GetConfiguration(int RoomSize, int index)
        {
            return _configuration[RoomSize][index];
        }


        private static int GetConfiguratinsCount(int roomSize)
        {
            return _configuration[roomSize].Count;
        }
    }
}