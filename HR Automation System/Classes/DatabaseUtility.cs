using HR_Automation_System.Properties;
using System.Data.OleDb;

namespace HR_Automation_System.Classes
{
    public class DatabaseUtility
    {
        private OleDbConnection _connection;

        public DatabaseUtility()
        {
            Settings settings = new Settings();
            _connection = new OleDbConnection();
            _connection.ConnectionString = settings.databaseConnectionString;
            _connection.Open();
        }

        public void Disconnect()
        {
            _connection.Close();
        }
    }
}
