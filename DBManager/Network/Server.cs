using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DbManager;
using System.IO.Pipes;
using System.Xml;

namespace DbManager.Network
{
    public class Server
    {
        public void Listen(int port)
        {
            //DEADLINE 6: Implement the server as specified (eGela)
            //Have a look at the project ServerConsole to see how a TcpListener is used
            //Use XmlSerializer to create Xml commands

            try
            {
                //Crear server

                // llamar a listen (puerto) en server


                DbManager.Database serverDatabase = new Database("admin", "adminPassword");
                //Listen on port 1200. Accept connections from any IP address

                TcpListener server = new TcpListener(IPAddress.Parse("0.0.0.0"), port);

                server.Start();

                Console.WriteLine("Server running and listening on port 1200");

                Socket socket = server.AcceptSocket();

                Console.WriteLine("Connection accepted from " + socket.RemoteEndPoint);

                bool trueFalse = true;
                while (trueFalse == true)
                {
                    byte[] buffer = new byte[100];
                    int bytesRead = socket.Receive(buffer);
                    buffer[bytesRead] = 0;
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string clientMessage = encoding.GetString(buffer).Substring(0, bytesRead);
                    Console.WriteLine("Message received from client: " + clientMessage);
                    if (clientMessage == "Exit")
                    {
                        trueFalse = false;
                    }
                    else
                    {
                        string clientResult = serverDatabase.ExecuteMiniSQLQuery(clientMessage);
                        socket.Send(encoding.GetBytes(clientResult));
                    }
                }

                Task.Delay(2000).Wait();

                socket.Close();
                server.Stop();

            }
            catch (Exception e)
            {
            Console.WriteLine("Unhandled error: " + e);
;
            }
            

        }  
    }
}
