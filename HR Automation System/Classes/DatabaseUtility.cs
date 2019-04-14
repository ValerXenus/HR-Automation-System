using HR_Automation_System.Properties;
using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;

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

            try
            {
                _connection.Open();
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к базе данных");
            }
        }

        public void Disconnect()
        {
            _connection.Close();
        }

        public void AddFamilyStatus(string status)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = _connection;

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO family_statuses VALUES ([Status_Index], [Status_Name])";
            //cmd.CommandText = "INSERT INTO family_statuses VALUES ([Status_Name])";
            cmd.Parameters.AddWithValue("@Status_Index", 1);
            cmd.Parameters.AddWithValue("@Status_Name", status);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось создать запрос\n" + ex.Message);
            }

            cmd.CommandText = "SELECT LAST(status_id) FROM family_statuses";            
            OleDbDataReader dr = cmd.ExecuteReader();

            int id = 0;

            while (dr.Read())
            {
                id = int.Parse(dr["Expr1000"].ToString());
            }            
        }
    }
}
