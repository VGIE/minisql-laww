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

                //Console.WriteLine("Server running and listening on port 1200");

                Socket socket = server.AcceptSocket();

                //Console.WriteLine("Connection accepted from " + socket.RemoteEndPoint);

                Database activeDB= null;
                bool running = true;

                while(running==true)
                {
                    byte[] buffer= new byte[4096];
                    int bytesRead = socket.Receive(buffer);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    string response= "";

                    if (XmlDeserializer.ParseOpen(clientMessage, out string database, out string username, out string password))
                    {
                        activeDB= Database.Load(database, username, password);

                        if (activeDB!=null)
                        {
                            response= XmlSerializer.OpenCreateSuccess;
                        }
                        else
                        {
                            response= XmlSerializer.OpenCreateError("Error opening database");
                        }
                    }
                    else if (XmlDeserializer.ParseCreate(clientMessage, out database, out username, out password))
                    {
                        try
                        {
                            activeDB = new Database(username, password);
                            if (activeDB.Save(database))
                             {
                                response = XmlSerializer.OpenCreateSuccess;
                             }
                             else
                              {
                                response = XmlSerializer.CreateError("Error saving database");
                              }
                            {
                                
                            }
                        }
                        catch (Exception e)
                        {
                            response = XmlSerializer.OpenCreateError(e.Message);
                        }  
                       
                    }
                    else if (XmlDeserializer.ParseQuery(clientMessage, out string query))
                    {
                        if (activeDB != null)
                        {
                            string queryResult = activeDB.ExecuteMiniSQLQuery(query);
                            response = XmlSerializer.SucessfulAnswer(queryResult);
                        }
                        else
                        {
                            response = XmlSerializer.ErrorAnswer("No database is open");
                        }
                    }
                    else if (XmlDeserializer.IsCloseCommand(clientMessage))
                    {
                        running = false;
                        break;
                    }
                    else
                    {
                        response = XmlSerializer.ErrorAnswer("Error: Unrecognized command");
                        
                    }
                    

                    if (!string.IsNullOrEmpty(response))
                    {
                       byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                       socket.Send(responseBytes);
                        
                    }
                }


                Task.Delay(2000).Wait();

                socket.Close();
                server.Stop();

            }
            catch (Exception e)
            {
            //Console.WriteLine("Unhandled error: " + e);

            }


        }  
    }
}
