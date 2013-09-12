using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.BulkMessage.Scheduler
{
    public static class Logger
    {
        private static object syncRoot = new Object();

        public static void Write(string message)
        {
            lock (syncRoot)
            {
                Console.WriteLine(DateTime.Now.ToString() + "   -   " + message);
                try
                {
                    string path = Path.Combine(ConfigManager.GetConfiguration("logpath"), "log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                    System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
                    file.WriteLine(DateTime.Now.ToString() + "   -   " + message);
                    file.Close();
                }
                catch (Exception ex)
                {

                }
                
            }

        }

    }
}
