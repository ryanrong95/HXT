using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ConsoleApp.vTaskers.Services;

namespace HKExitTask
{
    class Program
    {
        static void Main(string[] args)
        {
            HKExitTaskers.Current.Start()
        }


        private static void Current_SqlError(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }
    }
}
