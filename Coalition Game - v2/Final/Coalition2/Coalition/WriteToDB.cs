using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class WriteToDB
    {
        public static void WriteProposalToDB(int gameIndex, int n, int proposerIndex, int[] w, int[] shares, String[] decisions, int[] timeouts)
        {
            String path = @"C:\Users\User\Desktop\";
            string filePath2 = path+"\\test2.csv";
            string filePath3 = path + "\\test3.csv";
            string filePath4 = path + "\\test4.csv";
            string filePath5 = path + "\\test5.csv";

            string delimiter = ",";
            ArrayList list = new ArrayList();
            list.Add(gameIndex);
            list.Add(n);
            list.Add(proposerIndex);
            for (int i=0 ; i<w.Length ; i++)
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

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
                sb.Append(string.Join(delimiter,list[i].ToString()+","));

            TextWriter tsw = null;
            if (n == 2)
                tsw = new StreamWriter(filePath2, true);
            if (n == 3)
                tsw = new StreamWriter(filePath3, true);
            if (n == 4)
                tsw = new StreamWriter(filePath4, true);
            if (n == 5)
                tsw = new StreamWriter(filePath5, true);
            tsw.WriteLine(sb);
            tsw.Close();

        }
    }
}
