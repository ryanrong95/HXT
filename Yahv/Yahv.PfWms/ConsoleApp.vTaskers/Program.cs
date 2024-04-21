using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.vTaskers
{
    class Program
    {
        static private void Current_SqlError(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }

        static void Main(string[] args)
        {

            //Services.Taskers.Current.TaskOrder("NL02020200429008", "Admin00057");

            Services.Taskers.Current.SqlError += Current_SqlError;
            Services.Taskers.Current.Start();
        }
    }
}
