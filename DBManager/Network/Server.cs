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
                TcpListener server = new TcpListener(IPAddress.Parse("0.0.0.0"), port);
                server.Start();

                Socket socket = server.AcceptSocket();

                Database activeDB = null;
                bool running = true;

                while (running)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = socket.Receive(buffer);

                    if (bytesRead <= 0) break;

                    string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string response = "";

                    if (clientMessage.StartsWith("<Open"))
                    {
                        if (XmlDeserializer.ParseOpen(clientMessage, out string database, out string username, out string password))
                        {
                            activeDB = Database.Load(database, username, password);
                            if (activeDB != null)
                            {
                                response = XmlSerializer.OpenCreateSuccess;
                            }
                        }
                        else
                        {
                            response = XmlSerializer.OpenCreateError(Constants.IncorrectLogin);
                        }
                    }

                    else if (clientMessage.StartsWith("<Create"))
                    {
                        try
                        {
                            if (XmlDeserializer.ParseCreate(clientMessage, out string database, out string username, out string password))
                            {
                                activeDB = new Database(username, password);
                                if (activeDB.Save(database))
                                {
                                    response = XmlSerializer.OpenCreateSuccess;
                                }
                                else
                                {
                                    response = XmlSerializer.CreateError(Constants.CouldNotCreateDatabase);
                                }
                            }
                            else
                            {
                                response = XmlSerializer.CreateError(Constants.CouldNotCreateDatabase);
                            }
                        }
                        catch (Exception e)
                        {
                            response = XmlSerializer.CreateError(e.Message);
                        }
                    }

                    else if (clientMessage.StartsWith("<Query"))
                    {
                        if (XmlDeserializer.ParseQuery(clientMessage, out string query))
                        {
                            if (activeDB != null)
                            {
                                try
                                {
                                    string queryResult = activeDB.ExecuteMiniSQLQuery(query);
                                    response = XmlSerializer.SucessfulAnswer(queryResult);
                                }
                                catch (Exception e)
                                {
                                    response = XmlSerializer.ErrorAnswer(e.Message);
                                }

                            }

                            else
                            {
                                response = XmlSerializer.ErrorAnswer(Constants.NoDatabaseOpen);
                            }
                        }
                        else
                        {
                            response = XmlSerializer.ErrorAnswer(Constants.NoDatabaseOpen);
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
            }
        }
    }
}