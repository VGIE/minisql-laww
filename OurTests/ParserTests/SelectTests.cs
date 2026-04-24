using DbManager;

namespace OurTests
{
    public class SelectTests
    {
       [Fact]
       public void TestSelectConstructor()
        {
            string table= "Datos";
            List<string> columns= new List<string>{"Nombre", "Edad"};
            Condition condicion= new Condition("Edad", ">", "20");

            Select selectQ= new Select(table, columns, condicion);

            Assert.Equal("Datos", selectQ.Table);
            Assert.Equal(2, selectQ.Columns.Count);
            Assert.Equal("Nombre", selectQ.Columns[0]);
            Assert.Equal(condicion, selectQ.Where);
        } 

       [Fact]
       public void TestSelectExecuteSuccess()
        {
            Database db= Database.CreateTestDatabase();
            List<string> columns= new List<string>{Table.TestColumn1Name};
            Select selectQ= new Select(Table.TestTableName, columns, null);
            string result= selectQ.Execute(db);

            Assert.NotNull(result);
            Assert.Contains(Table.TestColumn1Row1, result);
            Assert.DoesNotContain(Constants.UpdateSuccess, result);
            
        }

        [Fact]
        public void TestSelectWithCondition()
        {
            Database db= Database.CreateTestDatabase();
            List<string> columns= new List<string>{Table.TestColumn1Name};
            Condition condicion= new Condition(Table.TestColumn1Name, "=", Table.TestColumn1Row1);

            Select selectQ= new Select(Table.TestTableName, columns, condicion);
            string result= selectQ.Execute(db);

            Assert.Contains(Table.TestColumn1Row1, result);
            Assert.DoesNotContain(Table.TestColumn1Row2, result);
            
        }

        [Fact]

        public void TestSelectWithSpacesInLiteralWithoutQuotesNull()
        {
            string query= "SELECT Nombre FROM Usuarios WHERE Ciudad = Lu ";

            var result= MiniSQLParser.Parse(query);
            Assert.Null(result);

        }


        [Fact]
        public void TestSelectWhereValueWIthSpacesWithQuotesNotNull()
        {
            string query= "SELECT Nombre FROM Personas WHERE Name='Lupe'";
            Select result = MiniSQLParser.Parse(query) as Select;

            Assert.NotNull(result);
            Assert.Equal("Lupe", result.Where.LiteralValue);
        }


        [Fact]
        public void TestSelectNonExistentColumn()
        {
            Database db = Database.CreateTestDatabase();
            List<string> columns = new List<string> { "ColumnaInexistente" };
            Condition condition = null;

            Select selectQuery = new Select(Table.TestTableName, columns, condition);
            string result = selectQuery.Execute(db);

            Assert.Equal(Constants.ColumnDoesNotExistError, result);
        }

        [Fact]
        public void TestSyntaxErrorUpdatesLastErrorMessage()
        {
            Database db = Database.CreateTestDatabase();

            string result = db.ExecuteMiniSQLQuery("SELECT FROM"); 

            Assert.Equal(Constants.SyntaxError, result);
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage); 
        }

        [Fact]
        public void TestSyntaxErrorDoesNotPersist()
        {
            Database db = Database.CreateTestDatabase();

            db.ExecuteMiniSQLQuery("SELECT FROM");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT ColumnaInexistente FROM " + Table.TestTableName);
            Assert.Equal(Constants.ColumnDoesNotExistError, db.LastErrorMessage);
        }

        [Fact]
        public void TestSelectSyntaxErrors()
        {
            Database db = Database.CreateTestDatabase();

            db.ExecuteMiniSQLQuery("SELECT dni, nombre FROM Tabla");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni,  FROM Tabla");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni,,nombre FROM Tabla");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni Tabla");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad 20");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad<");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE nombre='Juan");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad!=20");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE nombre=Juan Perez");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery(null);
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);
        }

        [Fact]
        public void TestSelectValidQueries()
        {
            Database db = Database.CreateTestDatabase();

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni,nombre FROM Tabla");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT * FROM Tabla");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad=20");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad=-20");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE nombre='Juan'");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad>18");
            Assert.Equal(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE edad<'6.5'");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT dni FROM Tabla WHERE nombre='Juan Perez'");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT col1,col2 FROM Tabla1");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);

            db.ExecuteMiniSQLQuery("SELECT    dni    FROM    Tabla");
            Assert.NotEqual(Constants.SyntaxError, db.LastErrorMessage);
        }
        [Fact]
        public void TestCommasInWhereCondition()
        {

           string query1= "SELECT Nombre FROM Personas WHERE Name=Lupe";
           Select result1 = MiniSQLParser.Parse(query1) as Select;

            string query= "SELECT Nombre FROM Personas WHERE Name=2";
            Select result = MiniSQLParser.Parse(query) as Select;
            Assert.Null(result1);
            Assert.Null(result);
            
        }

    }
    
}
