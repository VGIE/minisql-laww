using DbManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DbManager.Network
{
    public class Client
    {
        TcpClient m_tcpClient;
        public Client()
        {
            m_tcpClient = new TcpClient();
        }
        public bool Connect(string ipAddress, int port)
        {
            //DEADLINE 6: Connect the tcp client to the given ip/port
            //Return false if something goes wrong, true otherwise (try/catch)

            try
            {
                if (m_tcpClient!=null)
                {
                    m_tcpClient.Close(); 
                }
                TcpClient newClient = new TcpClient();
                m_tcpClient.Connect(ipAddress, port);
                return true;
                
            }catch
            {
                
                return false;
            
            }
        }

        private string SendString(string message)
        {
            //DEADLINE 6: Send a string to the server, read the answer and return it.
            //Here, we do not do any Xml formatting, we just send the string as it comes and return the string as it comes
            //This private method should be used from Open/SendQuery/Close
            //Have a look at the project ClientConsole to see how we can use the TcpClient class

            string messageToSend= message;
                try
                {
                    NetworkStream stream = m_tcpClient.GetStream();
                    //Pasamos el mensaje string a bytes para mandar (con UTF8 se transforma)
                    byte[] dataToSend = Encoding.UTF8.GetBytes(messageToSend);
                    //Mandamos al servidor el mensaje en bytes donde 0 es offset
                    stream.Write(dataToSend, 0, dataToSend.Length);
    
                    //EL buffer lee la respuesta del servido. EL tamaño es 4096 pero se pyede cambiar dependiendo de la respuesta esperada 
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if(bytesRead==0)
                    {
                        return null;
                    }   
                    //De vuelta a string la respuesta dle servidor
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    return response;
                }
                catch
                {
                    return null;
                }            
        }

        public bool Open(string database, string username, string password, out string error)
        {
            //DEADLINE 6: Send an Open command to the server using SendString

            error = null;
            string request= XmlSerializer.OpenDatabase(database, username, password);
            string response = SendString(request);

            if (response==null)
            {
                error="An error message from Constants.cs";
                return false;
            }

            bool success= XmlDeserializer.ParseOpenCreateAnswer(response, out error);
           
            return success;
            
        }

        public bool Create(string database, string username, string password, out string error)
        {
            //DEADLINE 6: Send a Create command to the server using SendString
            
            error = null;
            string request= XmlSerializer.CreateDatabase(database, username, password);
            string response = SendString(request);
            if(response==null)
            {
                error="An error message from Constants.cs";
                return false;
            }
            bool success= XmlDeserializer.ParseOpenCreateAnswer(response, out error);

            return success;
            
        }

        public string SendQuery(string query)
        {
         
            //DEADLINE 6: Send a Query command to the server using SendString
            string queryRequest=XmlSerializer.Query(query);
            string response = SendString(queryRequest);
            if (response==null)
            {
                return "An error message from Constants.cs";
            }

            if(XmlDeserializer.ParseQueryAnswer(response, out string answerContent))
             {
                return answerContent;
             }
             else
              {
                return answerContent;
              }
            
        }

        public void Close()
        {
            //DEADLINE 6: Send a Close command to the server using SendString and close the connection to the server
            SendString(XmlSerializer.CloseConnection);
            m_tcpClient.Close(); 
            
        }
    }
}
