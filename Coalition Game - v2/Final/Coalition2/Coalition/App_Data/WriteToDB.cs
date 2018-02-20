using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coalition.App_Data
{
    class WriteToDB
    {
        static string path = @"C:\Sites\Coalition\";
        private static Mutex m_writemutex = new Mutex();
        public static void WriteProposalToDB(String[] playersHash, int currectRound, string gameIndex, int n, int proposerIndex, int aiIndex, int[] w, int[] shares, String[] decisions, int[] timeouts)
        {
            StringBuilder sb = new StringBuilder();
            
            try {
               
                string delimiter = ",";
                ArrayList list = new ArrayList();
                list.Add(DateTime.Now);
                list.Add(currectRound);
                list.Add(gameIndex);
                list.Add(n);
                list.Add(proposerIndex);
                list.Add(aiIndex);

                for (int i = 0; i < playersHash.Length; i++)
                {
                    list.Add(playersHash[i].ToString());
                }

                for (int i = 0; i < w.Length; i++)
                {
                    list.Add(w[i].ToString());
                }
                for (int i = 0; i < shares.Length; i++)
                {
                    list.Add(shares[i].ToString());
                }
                for (int i = 0; i < decisions.Length; i++)
                {
                    list.Add(decisions[i]);
                }
                for (int i = 0; i < timeouts.Length; i++)
                {
                    list.Add(timeouts[i].ToString());
                }

                
                for (int i = 0; i < list.Count; i++)
                    sb.Append(string.Join(delimiter, list[i].ToString() + ","));

            } catch (Exception ex) {
                 // Prevents the server from exception and mutex lock
            }

            try
            {
                m_writemutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                m_writemutex.ReleaseMutex();
                m_writemutex.WaitOne();
            }

            try {

                if (sb.Length<=0) {
                    throw new Exception("Empty output string");
                }

                string filename = "test" + n + ".csv";
                StreamWriter tsw = new StreamWriter(path + filename, true);               
                tsw.WriteLine(sb);
                tsw.Close();

            } catch (Exception ex)
            {
                string fileName = "ErrorLogRooms_"+ DateTime.Now.ToString("yyyy_MM_dd_mm_ss")+".txt";
                StreamWriter sw = new StreamWriter(path + fileName);
                sw.Write(ex.Message);
                sw.Close();
            }

            m_writemutex.ReleaseMutex();
        }
        public static void WriteFinishedPlayer(Player player)
        {

            player.UserNameTimeErased = DateTime.Now;
            player.SecondsInGame = (DateTime.Now - player.UserNameTimeCreated).TotalSeconds;

            try
            {
                m_writemutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                m_writemutex.ReleaseMutex();
                m_writemutex.WaitOne();
            }
            try {

                StreamWriter sw = new StreamWriter(path + "PlayersFinished.csv", true);

                sw.WriteLine(player.HashPlayer + "," + player.workerID + "," + player.assID + "," 
                    + player.hitID + "," + (player.PlayScore+player.WaitScore) + "," + 
                    player.SecondsInGame + ","+ player.UserNameTimeCreated+","+player.UserNameTimeErased+"," 
                    +player.NumberOfGames);

                sw.Close();
            }
            catch (Exception ex)
            {
                string fileName = "ErrorLogPlayers_" + DateTime.Now.ToString("yyyy_MM_dd_mm_ss") + ".txt";
                StreamWriter sw = new StreamWriter(path + fileName);
                sw.Write(ex.Message);
                sw.Close();
            }


            m_writemutex.ReleaseMutex();
        }
    }
}
