using DbManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbManager.Network
{
    public static class XmlDeserializer
    {
        public static bool ParseOpen(string command, out string database, out string username, out string password)
        {
            //TODO DEADLINE 6: Try to parse the xml command using the specified xml format (eGela)
            //Return true if 'command' is an Open statement, false otherwise. If true, set the value of database, username and password

            database = null;
            username = null;
            password = null;

            if (string.IsNullOrEmpty(command))
                return false;

            if (!command.StartsWith("<Open") || !command.EndsWith("/>"))
                return false;


            const string openPattern = @"^<Open\sDatabase=""([^""]+)""\sUser=""([^""]+)""\sPassword=""([^""]+)""/>$";
            Match match = Regex.Match(command, openPattern);

            if (!match.Success)
                return false;

            database = match.Groups[1].Value;
            username = match.Groups[2].Value;
            password = match.Groups[3].Value;

            return true;
        }

        public static bool ParseOpenCreateAnswer(string answer, out string error)
        {
            //TODO DEADLINE 6: Try to parse the answer to an Open/Create command.
            //Return true if 'command' is equal to XmlSerializer.OpenCreateSuccess
            //If it is an error (<Error>...</Error>), return false and set 'error' with the error message

            error = null;
            return false;
        }

        public static bool ParseCreate(string command, out string database, out string username, out string password)
        {
            //TODO DEADLINE 6: Try to parse a Create xml command using the specified xml format (eGela)
            //Return true if 'command' is a Create statement, false otherwise. If true, set the value of database, username and password

            database = null;
            username = null;
            password = null;

            if (string.IsNullOrEmpty(command))
                return false;

            if (!command.StartsWith("<Create") || !command.EndsWith("/>"))
                return false;


            const string createPattern = @"^<Create\sDatabase=""([^""]+)""\sUser=""([^""]+)""\sPassword=""([^""]+)""/>$";
            Match match = Regex.Match(command, createPattern);

            if (!match.Success)
                return false;


            database = match.Groups[1].Value;
            username = match.Groups[2].Value;
            password = match.Groups[3].Value;

            return true;

        }



        public static bool ParseQuery(string answer, out string query)
        {
            //TODO DEADLINE 6: Try to parse a Query xml command using the specified xml format (eGela)
            //Return true if 'command' is a Query statement, false otherwise. If true, set the value of query with the content of the command

            query = null;
            return false;
        }

        public static bool ParseQueryAnswer(string answer, out string answerContent)
        {
            //TODO DEADLINE 6: Try to parse the answer to a Query command.
            //Return true if 'command' does not contain an error inside (<Error>...</Error>)
            //If it is an error (<Error>...</Error>), return false and set 'answerContent' with the error message

            answerContent = null;
            return false;
        }

        public static bool IsCloseCommand(string command)
        {
            return command == XmlSerializer.CloseConnection;
        }
    }
}
