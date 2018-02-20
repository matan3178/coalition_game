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
    public enum Type { NORMAL, NE };
    public class ConfigurationType
    {
        public Type configType { get; set; }
        private static ConfigurationType instance;

        public static ConfigurationType getInstance()
        {
            if (instance == null)
                instance = new ConfigurationType();
            return instance;
        }
        private ConfigurationType()
        {

        }
        public Configuration createConfiguration(int numOfPlayers)
        {
            if (configType == Type.NE)
                return new NEConfiguration(numOfPlayers);
            else if (configType == Type.NORMAL)
                return new NormalConfiguration(numOfPlayers);
            throw new Exception("Undefined configuration type!");
        }

        internal void setTypeFromString(string type)
        {
            if (type.Contains("NE"))
                configType = Type.NE;
            else if (type.Contains("NORMAL"))
                configType = Type.NORMAL;
        }
    }
}