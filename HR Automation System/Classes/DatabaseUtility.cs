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
        private OleDbDataReader _dataReader;

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
            _dataReader.Close();
            _connection.Close(); // Завершение подключения с БД            
        }

        // Проверка логина и пароля пользователя
        // Если вернется -1, то авторизация пройдена
        public int CheckUserAuth(string loginString, string passwordString)
        {
            _command.CommandText = string.Format("SELECT user_id FROM users WHERE login = '{0}' AND pass = '{1}'", loginString, passwordString);
            _dataReader = _command.ExecuteReader();

            int idx = -1;
            if (_dataReader.HasRows) // Если результат запроса пуст
            {
                while (_dataReader.Read())
                {
                    idx = int.Parse(_dataReader["user_id"].ToString());
                }
            }
            _dataReader.Close();
            return idx;
        }

        public void AddFamilyStatus(string status)
        {
            int idx = getLastIndex("family_statuses", "status_id") + 1; // Получаем индекс последней записи в таблице и прибавляем 1

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

        public void GetFamilyStatusName(int idx)
        {
            string temp = string.Empty;

            _command.CommandText = string.Format("SELECT status_name FROM family_statuses WHERE status_id = {0}", idx);
            _dataReader = _command.ExecuteReader();

            while (_dataReader.Read())
            {
                temp = _dataReader["status_name"].ToString();
            }
            _dataReader.Close();
        }

        // Получить индекс последней добавленной записи в таблице
        private int getLastIndex(string tableName, string fieldName)
        {
            _command.CommandText = string.Format("SELECT LAST({0}) AS return_value FROM {1}", fieldName, tableName);
            _dataReader = _command.ExecuteReader();

            int idx = -1;

            try
            {
                while (_dataReader.Read())
                {
                    idx = int.Parse(_dataReader["return_value"].ToString());
                }
            }
            catch
            {
                // Если записей нет в таблице, то вернется -1
            }
            _dataReader.Close();

            return idx;
        }
    }
}
