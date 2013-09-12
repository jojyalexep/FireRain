using EP.BulkMessage.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireRainConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Bulk Messaging Console App";

            TimerService.Run();
            Console.Read();
        }
    }
}
