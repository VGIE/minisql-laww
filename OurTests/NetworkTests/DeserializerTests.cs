using DbManager.Network;
using DbManager.Security;

namespace OurTests.NetworkTests
{
    public class DeserializerTests
    {
        [Fact]
        public void ParseOpen()
        {
            string command = "<Open Database=\"db\" User=\"user\" Password=\"1234\"/>";

            bool result = XmlDeserializer.ParseOpen(command, out string db, out string user, out string pass);

            Assert.True(result);
            Assert.Equal("db", db);
            Assert.Equal("user", user);
            Assert.Equal("1234", pass);
        }

        [Fact]
        public void ParseOpenNull()
        {
            string db, user, pass;
            bool result = XmlDeserializer.ParseOpen(null, out db, out user, out pass);

            Assert.False(result);
        }

        [Fact]
        public void ParseOpenEmptyCommand()
        {
            string db, user, pass;
            bool result = XmlDeserializer.ParseOpen("", out db, out user, out pass);

            Assert.False(result);
        }

        [Fact]
        public void ParseOpenInvalidInputs()
        {
            string db, user, pass;

            //error en command
            string command1 = "<OpenX Database=\"db\" User=\"user\" Password=\"1234\"/>";
            bool result1 = XmlDeserializer.ParseOpen(command1, out db, out user, out pass);

            Assert.False(result1);

            //faltan atributos
            string command2 = "<Open Database=\"db\" User=\"user\"/>";
            bool result2 = XmlDeserializer.ParseOpen(command2, out db, out user, out pass);

            Assert.False(result2);

            //orden incorrecto
            string command3 = "<Open User=\"user\" Database=\"db\" Password=\"1234\"/>";
            bool result3 = XmlDeserializer.ParseOpen(command3, out db, out user, out pass);

            Assert.False(result3);

            //espacios en command
            string command4 = "<Open Database=\"db\" User=\"user\" Password= \"1234\"/>";
            bool result4 = XmlDeserializer.ParseOpen(command4, out db, out user, out pass);

            Assert.False(result4);

            string command5 = "<Open Database=\"db\" User=\"user\" Password=\"1234\" />";
            bool result5 = XmlDeserializer.ParseOpen(command5, out db, out user, out pass);

            Assert.False(result5);

            //valores vacios
            string command6 = "<Open Database=\"\" User=\"\" Password=\"\"/>";
            bool result6 = XmlDeserializer.ParseOpen(command6, out db, out user, out pass);

            Assert.False(result6);

            //command sin comillas
            string command7 = "<Open Database=db User=user Password=1234/>";
            bool result7 = XmlDeserializer.ParseOpen(command7, out db, out user, out pass);

            Assert.False(result7);
        }

        [Fact]
        public void ParseCreate()
        {
            string command = "<Create Database=\"db\" User=\"user\" Password=\"1234\"/>";

            bool result = XmlDeserializer.ParseCreate(command, out string db, out string user, out string pass);

            Assert.True(result);
            Assert.Equal("db", db);
            Assert.Equal("user", user);
            Assert.Equal("1234", pass);
        }

        [Fact]
        public void ParseCreateNull()
        {
            string db, user, pass;

            bool result = XmlDeserializer.ParseCreate(null, out db, out user, out pass);

            Assert.False(result);
        }

        [Fact]
        public void ParseCreateEmptyCommand()
        {
            string db, user, pass;

            bool result = XmlDeserializer.ParseCreate("", out db, out user, out pass);

            Assert.False(result);
        }

        [Fact]
        public void ParseCreateInvalidInputs()
        {
            string db, user, pass;

            // error en command
            string command1 = "<CreateX Database=\"db\" User=\"user\" Password=\"1234\"/>";
            bool result1 = XmlDeserializer.ParseCreate(command1, out db, out user, out pass);

            Assert.False(result1);

            // faltan atributos
            string command2 = "<Create Database=\"db\" User=\"user\"/>";
            bool result2 = XmlDeserializer.ParseCreate(command2, out db, out user, out pass);

            Assert.False(result2);

            // orden incorrecto
            string command3 = "<Create User=\"user\" Database=\"db\" Password=\"1234\"/>";
            bool result3 = XmlDeserializer.ParseCreate(command3, out db, out user, out pass);

            Assert.False(result3);

            // espacios extra
            string command4 = "<Create Database= \"db\" User=\"user\" Password=\"1234\"/>";
            bool result4 = XmlDeserializer.ParseCreate(command4, out db, out user, out pass);

            Assert.False(result4);

            string command5 = "<Create Database=\"db\" User=\"user\" Password=\"1234\" />";
            bool result5 = XmlDeserializer.ParseCreate(command5, out db, out user, out pass);

            Assert.False(result5);

            // valores vacíos
            string command6 = "<Create Database=\"\" User=\"\" Password=\"\"/>";
            bool result6 = XmlDeserializer.ParseCreate(command6, out db, out user, out pass);

            Assert.False(result6);

            // sin comillas
            string command7 = "<Create Database=db User=user Password=1234/>";
            bool result7 = XmlDeserializer.ParseCreate(command7, out db, out user, out pass);

            Assert.False(result7);
        }
    }
}
