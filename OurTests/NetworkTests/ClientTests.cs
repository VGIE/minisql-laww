

using DbManager.Network;

public class ClientTests
{
    
        [Fact]
        public void TestConnectReturnsFalseIfServerNotExists()
        {
            Client client = new Client();
            string ip="127.0.0.1";
            int port=8080;
            bool result = client.Connect(ip, port);
            Assert.False(result);
        }

        [Fact]
        public void TestConnectHandlesReconnection()
        {
            Client client = new Client();
            
            client.Connect("127.0.0.1", 8080);
            bool result = client.Connect("127.0.0.1", 8081);
            Assert.False(result);
        }

        [Fact]
        public void TestSendQueryConnectionFailureErrorMessage()
        {
            Client client=  new Client();
            string result = client.SendQuery("SELECT * FROM table");

            Assert.Equal("An error message from Constants.cs", result);
        }

        [Fact]
        public void TestOpenReturnsFalseWhenResponseIsNull()
        {
            Client client = new Client();
            string database = "testdb";
            string username = "lupita";
            string password = "123";

            bool result = client.Open(database, username, password, out string error);

            Assert.False(result);
            Assert.Equal("An error message from Constants.cs", error);
        }
}   