using HR_Automation_System.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // Отключение от БД
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

        #region Запросы на добавление

        // Добавление нового трудового договора
        public bool AddNewContract(string contractNumber, string filename, DateTime date)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO employment_contracts (contract_number, start_date, document_name, end_date, leaving_reason) " +
                "VALUES ([Contract_Number], [Start_Date], [Document_Name], [End_Date], [Leaving_Reason])";
            _command.Parameters.AddWithValue("@Contract_Number", contractNumber);
            _command.Parameters.AddWithValue("@Start_Date", date.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@Document_Name", filename);
            _command.Parameters.AddWithValue("@End_Date", DateTime.MaxValue.ToString("dd/MM/yyyy")); // Т.к. сотрудник не увольняется, ставим максимальную дату
            _command.Parameters.AddWithValue("@Leaving_Reason", "-"); // И не заполняем причину увольнения
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return false;
            }

            return true;
        }

        // Добавление нового сотрудника
        public bool AddNewEmployee(EmployeeInfo employeeInfo)
        {
            // Сперва добавляем сотрудника в таблицу "Сотрудники"
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO [employees] ([employee_name], [gender], [birth_date], [inn], [snils]," +
                " [document_type], [document_number], [address], [phone_number], [email], [photograph_link]," +
                " [empl_book_number], [family_status], [contract_id], [ml_id], [sl_id], [vacation_id])" +
                " VALUES ([Employee_Name], [Gender], [Birth_Date], [Inn], [Snils]," +
                " [Document_Type], [Document_Number], [Address], [Phone_Number], [Email], [Photograph_Link]," +
                " [Empl_Book], [Family_Status], [Contract_Id], [Ml_Id], [Sl_Id], [Vacation_Id]);";
            _command.Parameters.AddWithValue("@Employee_Name", employeeInfo.Name);
            _command.Parameters.AddWithValue("@Gender", employeeInfo.Gender);
            _command.Parameters.AddWithValue("@Birth_Date", employeeInfo.BirthDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@Inn", employeeInfo.Inn);
            _command.Parameters.AddWithValue("@Snils", employeeInfo.Snils);
            _command.Parameters.AddWithValue("@Document_Type", employeeInfo.DocumentType);
            _command.Parameters.AddWithValue("@Document_Number", employeeInfo.DocumentNumber);
            _command.Parameters.AddWithValue("@Address", employeeInfo.Address);
            _command.Parameters.AddWithValue("@Phone_Number", employeeInfo.Phone);
            _command.Parameters.AddWithValue("@Email", employeeInfo.Email);
            _command.Parameters.AddWithValue("@Photograph_Link", employeeInfo.ImageName);
            _command.Parameters.AddWithValue("@Empl_Book", employeeInfo.EmployeeBook);
            _command.Parameters.AddWithValue("@Family_Status", employeeInfo.FamilyStatus);
            _command.Parameters.AddWithValue("@Contract_Id", employeeInfo.Contract);
            _command.Parameters.AddWithValue("@Ml_Id", -1); // Отпусков пока нет, их не заполняем
            _command.Parameters.AddWithValue("@Sl_Id", -1);
            _command.Parameters.AddWithValue("@Vacation_Id", -1);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return false;
            }

            // Затем получаем Id добавленного сотрудника,
            // и создаем запись в таблице "Сотрудники в департаментах"
            var employeeId = getEmployeeByContractId(employeeInfo.Contract);

            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO [employees_in_departments] ([department_id], [employee_id], [position], [salary], [contract_id])" +
                " VALUES ([Department_Id], [Employee_Id], [Position], [Salary], [Contract_Id])";
            _command.Parameters.AddWithValue("@Department_Id", employeeInfo.Department);
            _command.Parameters.AddWithValue("@Employee_Id", employeeId);
            _command.Parameters.AddWithValue("@Position", employeeInfo.Position);
            _command.Parameters.Add(@"Salary", OleDbType.Double).Value = employeeInfo.Salary;
            //_command.Parameters.AddWithValue("@Salary", salary);
            _command.Parameters.AddWithValue("@Contract_Id", employeeInfo.Contract);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return false;
            }

            return true;
        }

        #endregion        

        #region Запросы на заполнение ComboBox
        // Получение списка семеных положений
        public List<BookClasses.FamilyStatuses> GetFamilyStatuses()
        {
            _command.CommandText = "SELECT * FROM family_statuses";
            _dataReader = _command.ExecuteReader();

            try
            {
                var falimyStatuses = new List<BookClasses.FamilyStatuses>();
                while (_dataReader.Read())
                {
                    falimyStatuses.Add(new BookClasses.FamilyStatuses
                    {
                        StatusId = int.Parse(_dataReader["status_id"].ToString()),
                        StatusName = _dataReader["status_name"].ToString()
                    });
                }
                _dataReader.Close();

                return falimyStatuses;
            }
            catch
            {
                MessageBox.Show("Справочник \"Семейные положения\" пуст");
                _dataReader.Close();
                return null;
            }
        }

        // Получение типов удостоверений личности
        public List<BookClasses.DocumentType> GetDocumentTypes()
        {
            _command.CommandText = "SELECT * FROM document_types";
            _dataReader = _command.ExecuteReader();

            try
            {
                var documentTypes = new List<BookClasses.DocumentType>();
                while (_dataReader.Read())
                {
                    documentTypes.Add(new BookClasses.DocumentType
                    {
                        DocumentId = int.Parse(_dataReader["document_type"].ToString()),
                        DocumentName = _dataReader["document_name"].ToString()
                    });
                }
                _dataReader.Close();

                return documentTypes;
            }
            catch
            {
                MessageBox.Show("Справочник \"Типы документов\" пуст");
                _dataReader.Close();
                return null;
            }
        }

        // Получение списка отделов
        public List<BookClasses.Department> GetDepartmentsList()
        {
            _command.CommandText = "SELECT * FROM departments";
            _dataReader = _command.ExecuteReader();

            try
            {
                var departments = new List<BookClasses.Department>();
                while (_dataReader.Read())
                {
                    departments.Add(new BookClasses.Department
                    {
                        DepartmentId = int.Parse(_dataReader["department_id"].ToString()),
                        DepartmentName = _dataReader["department_name"].ToString()
                    });
                }
                _dataReader.Close();

                return departments;
            }
            catch
            {
                MessageBox.Show("Справочник \"Отделы\" пуст");
                _dataReader.Close();
                return null;
            }
        }

        // Получение списка трудовых договоров
        public List<BookClasses.Contract> GetContractsList()
        {
            _command.CommandText = "SELECT * FROM employment_contracts";
            _dataReader = _command.ExecuteReader();

            try
            {
                var contracts = new List<BookClasses.Contract>();
                while (_dataReader.Read())
                {
                    contracts.Add(new BookClasses.Contract
                    {
                        ContractId = int.Parse(_dataReader["contract_id"].ToString()),
                        ContractNumber = _dataReader["contract_number"].ToString()
                    });
                }
                _dataReader.Close();

                return contracts;
            }
            catch
            {
                MessageBox.Show("Справочник \"Трудовые договора\" пуст");
                _dataReader.Close();
                return null;
            }
        }
        #endregion

        #region Запросы на заполнение DataGrid

        // Получение сотрудников для DataGrid на EmployeeListPage
        public ObservableCollection<BookClasses.EmployeeRow> GetEmployeesRows()
        {
            _command.CommandText = "SELECT [employees].[employee_id], [employees].[employee_name], [employees_in_departments].[position], [departments].[department_name]" +
                " FROM [departments] INNER JOIN ([employees] INNER JOIN [employees_in_departments] ON [employees].[employee_id] = [employees_in_departments].[employee_id])" +
                " ON [departments].[department_id] = [employees_in_departments].[department_id]";

            try
            {
                _dataReader = _command.ExecuteReader();
                var rows = new ObservableCollection<BookClasses.EmployeeRow>();
                while (_dataReader.Read())
                {
                    rows.Add(new BookClasses.EmployeeRow
                    {
                        EmployeeId = int.Parse(_dataReader["employee_id"].ToString()),
                        EmployeeName = _dataReader["employee_name"].ToString(),
                        Department = _dataReader["department_name"].ToString(),
                        Position = _dataReader["position"].ToString()
                    });
                }
                _dataReader.Close();

                return rows;
            }
            catch
            {
                return null; // Если таблица пустая
            }

        }

        #endregion

        #region Запросы на получение

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

        // Получить сотрудника по Id трудового договора
        private int getEmployeeByContractId(int contractId)
        {
            int idx = -1;

            _command.CommandText = string.Format("SELECT employee_id FROM employees WHERE contract_id = {0}", contractId);
            _dataReader = _command.ExecuteReader();

            while (_dataReader.Read())
            {
                idx = int.Parse(_dataReader["employee_id"].ToString());
            }
            _dataReader.Close();

            return idx;
        }

        // Получение данных сотрудника по его Id
        public EmployeeInfo GetEmployeeData(int employeeId)
        {
            _command.CommandText = "SELECT [employees].[employee_name], [employees].[gender]," +
                " [employees].[birth_date], [employees].[inn], [employees].[snils]," +
                " [employees].[document_type], [employees].[document_number], [employees].[address]," +
                " [employees].[phone_number], [employees].[email], [employees].[photograph_link]," +
                " [employees].[empl_book_number], [employees].[family_status]," +
                " [employees].[contract_id], [employees_in_departments].[department_id]," +
                " [employees_in_departments].[position], [employees_in_departments].[salary]" +
                " FROM [employees]" +
                " INNER JOIN [employees_in_departments] ON [employees].[employee_id] = [employees_in_departments].[employee_id]" +
                " WHERE [employees].[employee_id] = [EmployeeId]";
            _command.Parameters.Add(@"EmployeeId", OleDbType.Integer).Value = employeeId;

            try
            {
                _dataReader = _command.ExecuteReader();
                EmployeeInfo employeeInfo = new EmployeeInfo();
                while (_dataReader.Read())
                {
                    employeeInfo = new EmployeeInfo
                    {
                        Name = _dataReader["employee_name"].ToString(),
                        Gender = int.Parse(_dataReader["gender"].ToString()),
                        BirthDate = DateTime.Parse(_dataReader["birth_date"].ToString()),
                        Inn = _dataReader["inn"].ToString(),
                        Snils = _dataReader["snils"].ToString(),
                        DocumentType = int.Parse(_dataReader["document_type"].ToString()),
                        DocumentNumber = _dataReader["document_number"].ToString(),
                        Address = _dataReader["address"].ToString(),
                        Phone = _dataReader["phone_number"].ToString(),
                        Email = _dataReader["email"].ToString(),
                        EmployeeBook = _dataReader["empl_book_number"].ToString(),
                        FamilyStatus = int.Parse(_dataReader["family_status"].ToString()),
                        Contract = int.Parse(_dataReader["contract_id"].ToString()),
                        Department = int.Parse(_dataReader["department_id"].ToString()),
                        Position = _dataReader["position"].ToString(),
                        Salary = double.Parse(_dataReader["salary"].ToString()),
                        ImageName = _dataReader["photograph_link"].ToString()
                    };
                }
                _dataReader.Close();

                return employeeInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        #endregion
    }
}
