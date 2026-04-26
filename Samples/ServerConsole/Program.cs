using DbManager;
using DbManager.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Listen(1200);
     

                Console.WriteLine("Server closed. Press any key to finish...");
                Console.ReadKey();
            
        }
    }
}
