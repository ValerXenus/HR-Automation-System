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
        private OleDbCommand _command;

        public DatabaseUtility()
        {
            Settings settings = new Settings(); // Создаем экземпляр настроек
            _connection = new OleDbConnection();
            _connection.ConnectionString = settings.databaseConnectionString; // Получаем строку подключения

            try
            {
                _connection.Open(); // Подключение к БД
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к базе данных");
            }

            _command = new OleDbCommand(); // Инициализация экземпляра для команд
            _command.Connection = _connection;
        }

        public void Disconnect()
        {
            _connection.Close(); // Завершение подключения с БД
        }

        public void AddFamilyStatus(string status)
        {
            int idx = getLastIndex("family_statuses", "status_id"); // Получаем индекс последней записи в таблице и прибавляем 1

            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO family_statuses VALUES ([Status_Index], [Status_Name])";
            _command.Parameters.AddWithValue("@Status_Index", idx);
            _command.Parameters.AddWithValue("@Status_Name", status);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
            }            
        }

        // Получить индекс последней добавленной записи в таблице
        private int getLastIndex(string tableName, string fieldName)
        {
            _command.CommandText = string.Format("SELECT LAST({0}) AS return_value FROM {1}", fieldName, tableName);
            OleDbDataReader dr = _command.ExecuteReader();

            int idx = 0;

            try
            {
                while (dr.Read())
                {
                    idx = int.Parse(dr["return_value"].ToString());
                }
            }
            catch
            {
                // Если записей нет в таблице, то вернется 0
            }

            return idx;
        }
    }
}
