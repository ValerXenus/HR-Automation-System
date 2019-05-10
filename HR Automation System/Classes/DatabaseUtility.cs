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
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Ошибка подключения к базе данных.\n{0}", ex.Message));
            }

            _command = new OleDbCommand(); // Инициализация экземпляра для команд
            _command.Connection = _connection;
        }

        #region Запросы на добавление

        // Добавление нового трудового договора
        public bool AddNewContract(string contractNumber, string filename, DateTime date)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO employment_contracts (contract_number, start_date, document_name, end_date, leaving_reason, leaving_order) " +
                "VALUES ([Contract_Number], [Start_Date], [Document_Name], [End_Date], [Leaving_Reason], [LeavingOrder])";
            _command.Parameters.AddWithValue("@Contract_Number", contractNumber);
            _command.Parameters.AddWithValue("@Start_Date", date.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@Document_Name", filename);
            _command.Parameters.AddWithValue("@End_Date", DateTime.MaxValue.ToString("dd/MM/yyyy")); // Т.к. сотрудник не увольняется, ставим максимальную дату
            _command.Parameters.AddWithValue("@Leaving_Reason", "-"); // И не заполняем причину увольнения
            _command.Parameters.AddWithValue("@LeavingOrder", "-"); // приказ увольнения
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

            // Получаем Id добавленного сотрудника
            var employeeId = getEmployeeByContractId(employeeInfo.Contract);

            if (!AddNewEmployeeInDepartment(employeeInfo.Department, employeeId, employeeInfo.Position, employeeInfo.Salary, employeeInfo.Contract))
                return false;

            return true;
        }

        // Создание записи в таблице "Сотрудники в департаментах"
        public bool AddNewEmployeeInDepartment(int departmentId, int employeeId, string position, double salary, int contractId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO [employees_in_departments] ([date], [department_id], [employee_id], [position], [salary], [contract_id])" +
                " VALUES ([HistoryDate], [Department_Id], [Employee_Id], [Position], [Salary], [Contract_Id])";
            _command.Parameters.AddWithValue("@HistoryDate", DateTime.Now.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@Department_Id", departmentId);
            _command.Parameters.AddWithValue("@Employee_Id", employeeId);
            _command.Parameters.AddWithValue("@Position", position);
            _command.Parameters.Add(@"Salary", OleDbType.Double).Value = salary;
            _command.Parameters.AddWithValue("@Contract_Id", contractId);
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

        // Добавление отпуска
        public bool AddNewVacation(string vacationName, DateTime startDate, DateTime endDate, int employeeId)
        {
            // Добавляем отпуск
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO vacation (vacation_name, start_date, end_date, employee_id) " +
                "VALUES ([Vacation_Name], [Start_Date], [End_Date], [Employee_Id])";
            _command.Parameters.AddWithValue("@Vacation_Name", vacationName);
            _command.Parameters.AddWithValue("@Start_Date", startDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@End_Date", endDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@Employee_Id", employeeId);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return false;
            }

            // Добавляем отпуск к сотруднику, если дата соответствует
            setEmployeeVacation(employeeId, "vacation", "vacation_id");
            return true;
        }

        // Добавление больничного
        public bool AddNewSickLeave(DateTime startDate, DateTime endDate, int employeeId)
        {
            // Добавляем отпуск
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO sick_leaves (start_date, end_date, employee_id) " +
                "VALUES ([Start_Date], [End_Date], [Employee_Id])";
            _command.Parameters.AddWithValue("@Start_Date", startDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@End_Date", endDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@Employee_Id", employeeId);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return false;
            }

            // Добавляем отпуск к сотруднику, если дата соответствует
            setEmployeeVacation(employeeId, "sick_leaves", "sl_id");
            return true;
        }

        // Добавление декретного отпуска
        public bool AddNewMaternityLeave(string orderNumber, DateTime startDate, DateTime endDate, int employeeId)
        {
            // Добавляем отпуск
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO maternity_leave (employee_id, start_date, end_date, order_number) " +
                "VALUES ([Employee_Id], [Start_Date], [End_Date], [OrderNumber])";
            _command.Parameters.AddWithValue("@Employee_Id", employeeId);
            _command.Parameters.AddWithValue("@Start_Date", startDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@End_Date", endDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@OrderNumber", orderNumber);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return false;
            }

            // Добавляем отпуск к сотруднику, если дата соответствует
            setEmployeeVacation(employeeId, "maternity_leave", "ml_id");
            return true;
        }

        // Добавление нового документа
        public bool AddNewDocument(string documentName, string filename)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO hr_documents (document_name, document_link) " +
                "VALUES ([DocumentName], [DocumentFilename])";
            _command.Parameters.AddWithValue("@DocumentName", documentName);
            _command.Parameters.AddWithValue("@DocumentFilename", filename);
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

        // Добавление записи об новой аттестации
        public bool AddNewGraduaiotion(int employeeId, string date)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "INSERT INTO certification (cert_date, employee_id, result) " +
                "VALUES ([@CertificationDate], [@EmployeeId], -1)";
            // где -1 - это результат.            
            _command.Parameters.AddWithValue("@CertificationDate", date);
            _command.Parameters.AddWithValue("@EmployeeId", employeeId);            
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
            _command.Parameters.Clear();
            _command.CommandText = "SELECT DISTINCT [employees].[employee_id], [employees].[employee_name], [employees].[birth_date], " +
                "[employees].[email], [employees_in_departments].[position], [employees_in_departments].[department_id], [departments].[department_name], " +
                "[employment_contracts].[start_date], [employment_contracts].[document_name] " +
                "FROM ([employees] INNER JOIN ([departments] INNER JOIN [employees_in_departments] " +
                "ON [departments].[department_id] = [employees_in_departments].[department_id]) " +
                "ON [employees].[employee_id] = [employees_in_departments].[employee_id]) " +
                "INNER JOIN [employment_contracts] ON [employees].[contract_id] = [employment_contracts].[contract_id] " +
                "WHERE ([employment_contracts].[leaving_reason]='-') " +
                "AND [record_id] = (SELECT MAX([record_id]) FROM [employees_in_departments] WHERE [employee_id] = [employees].[employee_id]);";

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
                        BirthDate = DateTime.Parse(_dataReader["birth_date"].ToString()),
                        Email = _dataReader["email"].ToString(),
                        Department = _dataReader["department_name"].ToString(),
                        DepartmentId = int.Parse(_dataReader["department_id"].ToString()),
                        Position = _dataReader["position"].ToString(),
                        ContractDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                        ContractFilename = _dataReader["document_name"].ToString()
                    });
                }
                _dataReader.Close();

                return rows;
            }
            catch
            {
                _dataReader.Close();
                return null; // Если таблица пустая
            }

        }

        // Получение сотрудников для DataGrid на EmployeeListPage
        public ObservableCollection<BookClasses.EmployeeRow> GetEmployeesRowsGraduation()
        {
            _command.Parameters.Clear();
            _command.CommandText = "SELECT [employees].[employee_id], [employees].[employee_name], [employees].[birth_date], [employees].[email], " +
                "[employees_in_departments].[position], [employees_in_departments].[department_id], [departments].[department_name], " +
                "[employment_contracts].[contract_id], [employment_contracts].[start_date], [certification].[cert_id] " +
                "FROM(([employees] INNER JOIN([departments] INNER JOIN[employees_in_departments] " +
                "ON[departments].[department_id] = [employees_in_departments].[department_id]) ON[employees].[employee_id] = [employees_in_departments].[employee_id]) " +
                "INNER JOIN[employment_contracts] ON[employees].[contract_id] = [employment_contracts].[contract_id]) " +
                "INNER JOIN[certification] ON[employees].[employee_id] = [certification].[employee_id] " +
                "WHERE([employment_contracts].[leaving_reason] = '-') AND ([certification].[result] = -1) " +
                "AND [record_id] = (SELECT MAX([record_id]) FROM [employees_in_departments] WHERE [employee_id] = [employees].[employee_id]); ";

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
                        BirthDate = DateTime.Parse(_dataReader["birth_date"].ToString()),
                        Email = _dataReader["email"].ToString(),
                        Department = _dataReader["department_name"].ToString(),
                        DepartmentId = int.Parse(_dataReader["department_id"].ToString()),
                        Position = _dataReader["position"].ToString(),
                        ContractDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                        GraduationId = int.Parse(_dataReader["cert_id"].ToString()),
                        ContractId = int.Parse(_dataReader["contract_id"].ToString())
                    });
                }
                _dataReader.Close();

                return rows;
            }
            catch
            {
                _dataReader.Close();
                return null; // Если таблица пустая
            }

        }

        // Запрос на получени данных об отпуске для таблицы
        public ObservableCollection<VacationData> GetVacationData(int employeeId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [vacation] WHERE [employee_id] = [EmployeeId]";
            _command.Parameters.Add(@"EmployeeId", OleDbType.Integer).Value = employeeId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var vacationData = new ObservableCollection<VacationData>();

                if (_dataReader.HasRows)
                {
                    while (_dataReader.Read())
                    {
                        var vacation = new VacationData
                        {
                            Id = int.Parse(_dataReader["vacation_id"].ToString()),
                            Name = _dataReader["vacation_name"].ToString(),
                            StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                            EndDate = DateTime.Parse(_dataReader["end_date"].ToString()),
                        };
                        vacationData.Add(vacation);
                    }
                }
                _dataReader.Close();
                return vacationData;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        // Запрос на получение списка больничных по текущему сотруднику
        public ObservableCollection<SickLeaveData> GetSickLeaves(int employeeId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [sick_leaves] WHERE [employee_id] = [EmployeeId]";
            _command.Parameters.Add(@"EmployeeId", OleDbType.Integer).Value = employeeId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var vacationData = new ObservableCollection<SickLeaveData>();

                if (_dataReader.HasRows)
                {
                    while (_dataReader.Read())
                    {
                        var vacation = new SickLeaveData
                        {
                            Id = int.Parse(_dataReader["sl_id"].ToString()),
                            StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                            EndDate = DateTime.Parse(_dataReader["end_date"].ToString()),
                        };
                        vacationData.Add(vacation);
                    }
                }
                _dataReader.Close();
                return vacationData;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        // Запрос на получение списка декретных отпусков по текущему сотруднику
        public ObservableCollection<MaternityLeaveData> GetMaternityLeaves(int employeeId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [maternity_leave] WHERE [employee_id] = [EmployeeId]";
            _command.Parameters.Add(@"EmployeeId", OleDbType.Integer).Value = employeeId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var vacationData = new ObservableCollection<MaternityLeaveData>();

                if (_dataReader.HasRows)
                {
                    while (_dataReader.Read())
                    {
                        var vacation = new MaternityLeaveData
                        {
                            Id = int.Parse(_dataReader["ml_id"].ToString()),
                            OrderNumber = _dataReader["order_number"].ToString(),
                            StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                            EndDate = DateTime.Parse(_dataReader["end_date"].ToString()),
                        };
                        vacationData.Add(vacation);
                    }
                }
                _dataReader.Close();
                return vacationData;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        // Запрос на получение списка кадровых документов
        public ObservableCollection<BookClasses.HrDocument> GetHrDocumentsList()
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [hr_documents]";

            try
            {
                _dataReader = _command.ExecuteReader();
                var documentsList = new ObservableCollection<BookClasses.HrDocument>();

                if (_dataReader.HasRows)
                {
                    while (_dataReader.Read())
                    {
                        var document = new BookClasses.HrDocument
                        {
                            Id = int.Parse(_dataReader["document_id"].ToString()),
                            Name = _dataReader["document_name"].ToString(),
                            Filename = _dataReader["document_link"].ToString()
                        };
                        documentsList.Add(document);
                    }
                }
                _dataReader.Close();
                return documentsList;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        // Запрос на получение списка истории карьерного роста сотрудника
        public ObservableCollection<BookClasses.EmployeeHistory> GetEmployeeHistory(int employeeId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT [departments].[department_name], [employees_in_departments].[position], [employees_in_departments].[salary], [employees_in_departments].[date] " +
                "FROM [departments] INNER JOIN [employees_in_departments] ON [departments].[department_id] = [employees_in_departments].[department_id] " +
                "WHERE [employees_in_departments].[employee_id] = [@EmployeeId]";
            _command.Parameters.Add("@EmployeeId", OleDbType.Integer).Value = employeeId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var historyList = new ObservableCollection<BookClasses.EmployeeHistory>();

                if (_dataReader.HasRows)
                {
                    while (_dataReader.Read())
                    {
                        var history = new BookClasses.EmployeeHistory
                        {
                            DepartmentName = _dataReader["department_name"].ToString(),
                            Position = _dataReader["position"].ToString(),
                            Salary = double.Parse(_dataReader["salary"].ToString()),
                            Date = DateTime.Parse(_dataReader["date"].ToString())
                        };
                        historyList.Add(history);
                    }
                }
                _dataReader.Close();
                return historyList;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        #endregion

        #region Запросы на получение

        // Получить данные трудового договора по имени файла
        public BookClasses.Contract GetContractDataByFilename(string filename)
        {
            _command.CommandText = string.Format("SELECT [employment_contracts].[contract_id], [employment_contracts].[contract_number] " +
                "FROM [employment_contracts] " +
                "WHERE [employment_contracts].[document_name] = '{0}'", filename);
            _dataReader = _command.ExecuteReader();

            var contract = new BookClasses.Contract();

            try
            {                
                while (_dataReader.Read())
                {
                    contract = new BookClasses.Contract
                    {
                        ContractId = int.Parse(_dataReader["contract_id"].ToString()),
                        ContractNumber = _dataReader["contract_number"].ToString()
                    };
                }
            }
            catch
            { }
            _dataReader.Close();

            return contract;
        }

        // Получить индекс последней добавленной записи по условию в таблице
        private int getLastIndexWithCondition(string tableName, string fieldName, string conditionField, int value)
        {
            _command.CommandText = string.Format("SELECT LAST({0}) AS return_value FROM {1} WHERE {2} = {3}",
                fieldName, tableName, conditionField, value);
            _dataReader = _command.ExecuteReader();

            int idx = -1;

            try
            {
                while (_dataReader.Read())
                {
                    idx = int.Parse(_dataReader["return_value"].ToString());
                }
                _dataReader.Close();
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

        // Запрос на получение дат последнего отпуска сотрудника
        private BookClasses.VacationDates getVacationDates(string tableName, int employeeId, string primaryKeyColumn)
        {
            _command.CommandText = string.Format("SELECT TOP 1 {2}, start_date, end_date FROM {0} WHERE employee_id = {1} ORDER BY {2} DESC",
                tableName, employeeId, primaryKeyColumn); // Получение последнего отпуска по employeeId
            _dataReader = _command.ExecuteReader();

            var dates = new BookClasses.VacationDates();

            try
            {
                while (_dataReader.Read())
                {
                    dates = new BookClasses.VacationDates
                    {
                        VacationId = int.Parse(_dataReader[primaryKeyColumn].ToString()),
                        StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                        EndDate = DateTime.Parse(_dataReader["end_date"].ToString())
                    };
                }
            }
            catch
            {
                // Если записей нет в таблице, то вернется null
            }
            _dataReader.Close();

            return dates;
        }

        // Получение данных сотрудника по его Id
        public EmployeeInfo GetEmployeeData(int employeeId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT [employees].[employee_name], [employees].[gender], [employees].[birth_date], [employees].[inn], " +
                "[employees].[snils], [employees].[document_type], [employees].[document_number], [employees].[address], [employees].[phone_number], " +
                "[employees].[email], [employees].[photograph_link], [employees].[empl_book_number], [employees].[family_status], " +
                "[employees].[contract_id], [employees_in_departments].[department_id], [employees_in_departments].[position], " +
                "[employees_in_departments].[salary], [employment_contracts].[document_name] " +
                "FROM ([employees] INNER JOIN [employees_in_departments] ON [employees].[employee_id] = [employees_in_departments].[employee_id]) " +
                "INNER JOIN [employment_contracts] ON [employees].[contract_id] = [employment_contracts].[contract_id] " +
                "WHERE employees.employee_id = [EmployeeId];";
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
                        ImageName = _dataReader["photograph_link"].ToString(),
                        ContractFilename = _dataReader["document_name"].ToString()
                    };
                }
                _dataReader.Close();
                return employeeInfo;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        // Запрос на получение данных для статистики
        public BookClasses.Statistics GetStatistics()
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT a.employee_id, " +
                "(SELECT COUNT(*) FROM[employees] INNER JOIN[employment_contracts] ON[employees].[contract_id] = " +
                    "[employment_contracts].[contract_id] WHERE([employment_contracts].[leaving_reason] = '-')) AS[overall], " +
                "(SELECT COUNT(*) FROM[employees] WHERE([vacation_id] = -1) AND([sl_id] = -1) AND([ml_id] = -1)) AS[working], " +
                "(SELECT COUNT(*) FROM[employees] WHERE NOT([vacation_id] = -1)) AS[vacation], " +
                "(SELECT COUNT(*) FROM[employees] WHERE NOT([sl_id] = -1)) AS[sick], " +
                "(SELECT COUNT(*) FROM[employees] WHERE NOT([ml_id] = -1)) AS[maternity], " +
                "(SELECT COUNT(*) FROM[employees] INNER JOIN[employment_contracts] ON[employees].[contract_id] = " +
                    "[employment_contracts].[contract_id] WHERE NOT([employment_contracts].[leaving_reason] = '-')) AS[dismissed] " +
                "FROM(SELECT DISTINCT employee_id FROM[employees]) a; ";
            try
            {
                _dataReader = _command.ExecuteReader();
                var statistics = new BookClasses.Statistics();
                while (_dataReader.Read())
                {
                    statistics = new BookClasses.Statistics
                    {
                        Overall = int.Parse(_dataReader["overall"].ToString()),
                        Working = int.Parse(_dataReader["working"].ToString()),
                        Vacation = int.Parse(_dataReader["vacation"].ToString()),
                        Sick = int.Parse(_dataReader["sick"].ToString()),
                        Maternity = int.Parse(_dataReader["maternity"].ToString()),
                        Dismissed = int.Parse(_dataReader["dismissed"].ToString())
                    };
                }
                _dataReader.Close();
                return statistics;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        // Запрос на получении информации об отпуске
        public BookClasses.VacationDates GetVacationInfo(int employeeId)
        {
            var vacationInfo = new BookClasses.VacationDates();
            vacationInfo.EmployeeId = employeeId;

            int vacationId = -1; // Код обычного отпуска
            int mlId = -1; // Код декретного отпуска
            int slId = -1; // Код больничного

            _command.CommandText = string.Format("SELECT [ml_id], [sl_id], [vacation_id] FROM [employees] WHERE [employee_id] = {0}", employeeId);

            try
            {
                _dataReader = _command.ExecuteReader();
                if (_dataReader.HasRows)
                {
                    while (_dataReader.Read())
                    {
                        mlId = int.Parse(_dataReader["ml_id"].ToString());
                        slId = int.Parse(_dataReader["sl_id"].ToString());
                        vacationId = int.Parse(_dataReader["vacation_id"].ToString());
                    }
                    _dataReader.Close();

                    if (vacationId != -1) // Обычный отпуск
                    {
                        vacationInfo = getVacationDates("vacation", vacationInfo.EmployeeId, "vacation_id");
                        vacationInfo.VacationType = 0;

                        if (!checkVacation(vacationInfo, "vacation_id")) // Если отпуск закончился или еще не наступил
                        {
                            return null;
                        }
                    }
                    else if (slId != -1) // Больничный
                    {
                        vacationInfo = getVacationDates("sick_leaves", vacationInfo.EmployeeId, "sl_id");
                        vacationInfo.VacationType = 1;

                        if (!checkVacation(vacationInfo, "sl_id")) // Если отпуск закончился или еще не наступил
                        {
                            return null;
                        }
                    }
                    else if (mlId != -1) // Декретный отпуск
                    {
                        vacationInfo = getVacationDates("maternity_leave", vacationInfo.EmployeeId, "ml_id");
                        vacationInfo.VacationType = 2;

                        if (!checkVacation(vacationInfo, "ml_id")) // Если отпуск закончился или еще не наступил
                        {
                            return null;
                        }
                    }
                    else
                    {
                        vacationInfo = null;
                    }
                }
            }
            catch
            {
                _dataReader.Close();
                vacationInfo = null;
            }

            return vacationInfo;
        }

        // Получить contractId сотрудника
        private int getEmployeeContractId(int employeeId)
        {
            int idx = -1;

            _command.CommandText = string.Format("SELECT contract_id FROM employees WHERE employee_id = {0}", employeeId);
            _dataReader = _command.ExecuteReader();

            while (_dataReader.Read())
            {
                idx = int.Parse(_dataReader["contract_id"].ToString());
            }
            _dataReader.Close();

            return idx;
        }

        // Получить документ по Id
        public BookClasses.HrDocument GetDocumentData(int documentId)
        {
            _command.Parameters.Clear();
            _command.CommandText = string.Format("SELECT * FROM hr_documents WHERE document_id = {0}", documentId);
            _dataReader = _command.ExecuteReader();

            BookClasses.HrDocument document = null;

            while (_dataReader.Read())
            {
                document = new BookClasses.HrDocument
                {
                    Name = _dataReader["document_name"].ToString(),
                    Filename = _dataReader["document_link"].ToString()
                };
            }
            _dataReader.Close();

            return document;
        }

        #region Запросы на получение данных для редактирования отпусков
        // Получение записи отпуска на редактирование
        public VacationData GetVacationRecord(int vacationId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [vacation] WHERE [vacation_id] = [VacationId]";
            _command.Parameters.Add(@"VacationId", OleDbType.Integer).Value = vacationId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var vacation = new VacationData();
                while (_dataReader.Read())
                {
                    vacation = new VacationData
                    {
                        Name = _dataReader["vacation_name"].ToString(),
                        StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                        EndDate = DateTime.Parse(_dataReader["end_date"].ToString())
                    };
                }
                _dataReader.Close();
                return vacation;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        public SickLeaveData GetSickLeaveRecord(int sickLeaveId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [sick_leaves] WHERE [sl_id] = [SickLeaveId]";
            _command.Parameters.Add(@"SickLeaveId", OleDbType.Integer).Value = sickLeaveId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var vacation = new SickLeaveData();
                while (_dataReader.Read())
                {
                    vacation = new SickLeaveData
                    {
                        StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                        EndDate = DateTime.Parse(_dataReader["end_date"].ToString())
                    };
                }
                _dataReader.Close();
                return vacation;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        public MaternityLeaveData GetMaternityLeaveRecord(int maternityLeaveId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "SELECT * FROM [maternity_leave] WHERE [ml_id] = [MaternityLeaveId]";
            _command.Parameters.Add(@"MaternityLeaveId", OleDbType.Integer).Value = maternityLeaveId;

            try
            {
                _dataReader = _command.ExecuteReader();
                var maternityLeave = new MaternityLeaveData();
                while (_dataReader.Read())
                {
                    maternityLeave = new MaternityLeaveData
                    {
                        StartDate = DateTime.Parse(_dataReader["start_date"].ToString()),
                        EndDate = DateTime.Parse(_dataReader["end_date"].ToString()),
                        OrderNumber = _dataReader["order_number"].ToString()
                    };
                }
                _dataReader.Close();
                return maternityLeave;
            }
            catch (Exception ex)
            {
                _dataReader.Close();
                MessageBox.Show(string.Format("Произошла ошибка при получении данных сотрудника: {0}", ex.Message));
                return null;
            }
        }

        #endregion

        #endregion

        #region Запросы на обновление

        // Сохранение отредактированного отпуска
        public bool SaveVacation(VacationData vacation)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [vacation] SET [vacation_name] = [VacationName], [start_date] = [StartDate], [end_date] = [EndDate] WHERE [vacation_id] = [VacationId]";
            _command.Parameters.AddWithValue("@VacationName", vacation.Name);
            _command.Parameters.AddWithValue("@StartDate", vacation.StartDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@EndDate", vacation.EndDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@VacationId", vacation.Id);
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

        // Сохранение отредактированного больничного
        public bool SaveSickLeave(SickLeaveData sickLeave)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [sick_leaves] SET [start_date] = [StartDate], [end_date] = [EndDate] WHERE [sl_id] = [SickLeaveId]";
            _command.Parameters.AddWithValue("@StartDate", sickLeave.StartDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@EndDate", sickLeave.EndDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@SickLeaveId", sickLeave.Id);
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

        // Сохранение отредактированного декретного отпуска
        public bool SaveMaternityLeave(MaternityLeaveData maternityLeave)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [maternity_leave] SET [start_date] = [StartDate], [end_date] = [EndDate]," +
                " [order_number] = [OrderNumber] WHERE [ml_id] = [MaternityLeaveId]";
            _command.Parameters.AddWithValue("@StartDate", maternityLeave.StartDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@EndDate", maternityLeave.EndDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@OrderNumber", maternityLeave.OrderNumber);
            _command.Parameters.AddWithValue("@MaternityLeaveId", maternityLeave.Id);
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

        // Сохранение отредактированных данных сотрудника
        public void UpdateEmployee(EmployeeInfo employeeInfo, int employeeId)
        {
            // Сперва обновляем таблицу "Сотрудники"
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [employees] SET [employee_name] = [@EmployeeName], [gender] = [@Gender], [birth_date] = [@BirthDate], " +
                "[inn] = [@InnParam], [snils] = [@SnilsParam], [document_type] = [@DocumentType], [document_number] = [@DocumentNumber], " +
                "[address] = [@AddressParam], [phone_number] = [@PhoneNumber], [email] = [@EmailParam], [photograph_link] = [@PhotographLink], " +
                "[empl_book_number] = [@EmplBook], [family_status] = [@FamilyStatus], [contract_id] = [@ContractId] " +
                "WHERE [employee_id] = [@EmployeeId]";
            _command.Parameters.AddWithValue("@EmployeeName", employeeInfo.Name);
            _command.Parameters.AddWithValue("@GenderParam", employeeInfo.Gender);
            _command.Parameters.AddWithValue("@BirthDate", employeeInfo.BirthDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@InnParam", employeeInfo.Inn);
            _command.Parameters.AddWithValue("@SnilsParam", employeeInfo.Snils);
            _command.Parameters.AddWithValue("@DocumentType", employeeInfo.DocumentType);
            _command.Parameters.AddWithValue("@DocumentNumber", employeeInfo.DocumentNumber);
            _command.Parameters.AddWithValue("@AddressParam", employeeInfo.Address);
            _command.Parameters.AddWithValue("@PhoneNumber", employeeInfo.Phone);
            _command.Parameters.AddWithValue("@EmailParam", employeeInfo.Email);
            _command.Parameters.AddWithValue("@PhotographLink", employeeInfo.ImageName);
            _command.Parameters.AddWithValue("@EmplBook", employeeInfo.EmployeeBook);
            _command.Parameters.AddWithValue("@FamilyStatus", employeeInfo.FamilyStatus);
            _command.Parameters.AddWithValue("@ContractId", employeeInfo.Contract);
            _command.Parameters.AddWithValue("@EmployeeId", employeeId);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
            }

            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [employees_in_departments] SET [department_id] = [DepartmentId], " +
                "[position] = [PositionParam], [salary] = [SalaryParam], [contract_id] = [ContractId] " +
                "WHERE [employee_id] = [EmployeeId]";
            _command.Parameters.AddWithValue("@DepartmentId", employeeInfo.Department);
            _command.Parameters.AddWithValue("@PositionParam", employeeInfo.Position);
            _command.Parameters.Add(@"SalaryParam", OleDbType.Double).Value = employeeInfo.Salary;
            _command.Parameters.AddWithValue("@ContractId", employeeInfo.Contract);
            _command.Parameters.AddWithValue("@EmployeeId", employeeId);

            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
            }
        }

        // Метод увольнения сотрудника
        public bool DismissEmployee(int employeeId, string orderNumber, string reason, DateTime leavingDate)
        {
            int contractId = getEmployeeContractId(employeeId);

            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [employment_contracts] SET [end_date] = [EndDate], [leaving_reason] = [LeavingReason]," +
                " [leaving_order] = [OrderNumber] WHERE [contract_id] = [ContractId]";
            _command.Parameters.AddWithValue("@EndDate", leavingDate.ToString("dd/MM/yyyy"));
            _command.Parameters.AddWithValue("@LeavingReason", reason);
            _command.Parameters.AddWithValue("@OrderNumber", orderNumber);
            _command.Parameters.AddWithValue("@ContractId", contractId);
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

        // Метод сохранения отредактированного документа
        public bool UpdateDocument(BookClasses.HrDocument document)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [hr_documents] SET [document_name] = [DocumentName], [document_link] = [DocumentLink]" +
                " WHERE [document_id] = [DocumentId]";
            _command.Parameters.AddWithValue("@DocumentName", document.Name);
            _command.Parameters.AddWithValue("@DocumentLink", document.Filename);
            _command.Parameters.AddWithValue("@DocumentId", document.Id);
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

        // Метод включения/отключения отпусков у сотрудников
        private void setEmployeeVacation(int employeeId, string tableName, string primaryKey)
        {
            int vacationId = getLastIndexWithCondition(tableName, primaryKey, "employee_id", employeeId);

            _command.Parameters.Clear();

            if (vacationId != -1)
            {
                _command.CommandType = CommandType.Text;
                _command.CommandText = string.Format("UPDATE [employees] SET [{0}] = [Vacation] WHERE [employee_id] = [Employee_Id]", primaryKey);
                _command.Parameters.AddWithValue("@Vacation", vacationId);
                _command.Parameters.AddWithValue("@Employee_Id", employeeId);
                try
                {
                    _command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                    return;
                }
            }
        }

        // Завершение отпуска сотрудника
        private void finishEmployeeVacation(BookClasses.VacationDates vacationInfo, string primaryKey)
        {
            _command.Parameters.Clear();

            _command.CommandType = CommandType.Text;
            _command.CommandText = string.Format("UPDATE [employees] SET {0} = [Vacation_Id] WHERE [employee_id] = [Employee_Id]", primaryKey);
            _command.Parameters.AddWithValue("@Vacation_Id", -1);
            _command.Parameters.AddWithValue("@Employee_Id", vacationInfo.VacationId);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
                return;
            }
        }

        // Установить результат аттестации сотрудника
        public bool SetGraduationResult(int graduationId, int resultId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "UPDATE [certification] SET [result] = [@ResultId] " +
                "WHERE [cert_id] = [@GraduationId]";
            _command.Parameters.AddWithValue("@ResultId", resultId);
            _command.Parameters.AddWithValue("@GraduationId", graduationId);
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

        #region Запросы на удаление

        // Метод сохранения отредактированного документа
        public void RemoveDocument(int documentId)
        {
            _command.Parameters.Clear();
            _command.CommandType = CommandType.Text;
            _command.CommandText = "DELETE FROM [hr_documents]" +
                " WHERE [document_id] = [DocumentId]";
            _command.Parameters.AddWithValue("@DocumentId", documentId);
            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выполнить запрос\n" + ex.Message);
            }
        }

        #endregion

        #region Прочие методы

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

        // Проверка дат отпуска
        private bool checkVacation(BookClasses.VacationDates vacationInfo, string primaryKey)
        {
            if (vacationInfo.StartDate > DateTime.Now)
            {
                return false; // Отпуск еще не наступил
            }

            if (DateTime.Now.Date != vacationInfo.EndDate.Date && vacationInfo.EndDate < DateTime.Now)
            {
                finishEmployeeVacation(vacationInfo, primaryKey);
                return false; // Отпуск завершился, обновляем данные сотрудника
            }

            return true;
        }

        // Метод на получение информации об уже существующей записи на аттестацию
        public bool CheckGraduation(int employeeId)
        {
            _command.CommandText = string.Format("SELECT cert_id FROM certification WHERE (employee_id = {0}) AND (result = -1)", employeeId);
            _dataReader = _command.ExecuteReader();

            if (_dataReader.HasRows)
            {
                _dataReader.Close();
                return true;
            }

            _dataReader.Close();
            return false;
        }

        #endregion
    }
}
