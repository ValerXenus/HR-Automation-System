﻿using HR_Automation_System.AdditionalWindows;
using HR_Automation_System.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for EmployeeProfileWindow.xaml
    /// </summary>
    public partial class EmployeeProfileWindow : Window
    {
        private int _currentEmployeeId; // Код получаемого сотрудника
        private byte[] _imageBytes; // Байты фотографии сотрудника
        private string _imagePath; // путь к картинке
        private string _imageExtension; // Расширение изображение
        private string _imageFilename; // Имя файла картинки
        private int _contractId; // Код трудового договора
        private string _contractNumber; // номер трудового договора

        public EmployeeProfileWindow(int employeeId = -1)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId; // Если это режим редактирование сотрудника, то код будет не равен -1
            LoadFormData();
        }

        #region Методы Click
        // Метод загрузки новой картинки
        private void LoadPhotograph_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();
            result.Filter = "Image Files(*.PNG;*.JPG;*.GIF)|*.PNG;*.JPG;*.GIF";
            result.Multiselect = false;
            result.ShowDialog();

            // Если файл каким-либо образом не был выбран, то ничего не делаем
            if (result.FileName == string.Empty && File.Exists(result.FileName))
            {
                return;
            }

            _imagePath = result.FileName;
            _imageExtension = Path.GetExtension(_imagePath); // Получение расширения картинки
            _imageBytes = File.ReadAllBytes(_imagePath); // Считываем данные файла
            LoadProfileImage();
        }

        // Метод сохранения сотрудника
        private void SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) // Если валидация не пройдена
                return;

            int familyStatusId = (int)FamilyStatusesComboBox.SelectedValue;
            int documentType = (int)DocumentTypesComboBox.SelectedValue;
            int departmentId = (int)DepartmentsComboBox.SelectedValue;
            int contractId = _contractId;

            double salary = Convert.ToDouble(SalaryTextBox.Text);

            int gender = 0;
            if (FemaleRadioButton.IsChecked == true)
            {
                gender = 1;
            }

            if (_currentEmployeeId != -1) // Если это режим на сохранение отредактированного сотрудника
            {
                UpdateEmployee(gender, documentType, contractId, familyStatusId, departmentId, salary);
            }
            else // Если это режим на сохранение нового сотрудника
            {
                SaveNewEmployee(gender, documentType, contractId, familyStatusId, departmentId, salary);
            }
        }

        // Добавление нового трудового договора
        private void AddNewContract_Click(object sender, RoutedEventArgs e)
        {
            var newContractWindow = new AddNewContract();
            newContractWindow.ShowDialog();

            if (!(bool)newContractWindow.DialogResult)
                return;

            _contractId = newContractWindow.ContractId;
            _contractNumber = newContractWindow.ContractNumber;

            ContractTextBox.Text = _contractNumber;
        }

        // Кнопка "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите закрыть окно без сохранения?",
                "Внимание",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                this.Close(); // Закрываем текущее окно
            }
        }

        // Добавление нового отпуска
        private void AddNewVacation_Click(object sender, RoutedEventArgs e)
        {
            var vacationWindow = new AddNewVacation(_currentEmployeeId);
            vacationWindow.ShowDialog();

            // Отображаем отпуск в личной карточке, если он действует с текущего дня
            LoadVacationsInfo();
        }

        // Добавление нового больничного
        private void AddNewSickLeave_Click(object sender, RoutedEventArgs e)
        {
            var vacationWindow = new AddNewSickLeave(_currentEmployeeId);
            vacationWindow.ShowDialog();

            LoadVacationsInfo();
        }

        // Добавление нового декретного отпуска
        private void AddNewMaternityLeave_Click(object sender, RoutedEventArgs e)
        {
            var vacationWindow = new AddNewMaternityLeave(_currentEmployeeId);
            vacationWindow.ShowDialog();

            LoadVacationsInfo();
        }

        // Редактирование отпуска
        private void EditVacation_Click(object sender, RoutedEventArgs e)
        {
            int currentId = (VacationsDataGrid.SelectedItem as VacationData).Id; // Получаем код отпуска из выделенной строки
            var vacationWindow = new AddNewVacation(_currentEmployeeId, currentId);
            vacationWindow.ShowDialog();

            LoadVacationsInfo();
        }

        // Редактирование больничного
        private void EditSickLeave_Click(object sender, RoutedEventArgs e)
        {
            int currentId = (SickLeavesDataGrid.SelectedItem as SickLeaveData).Id;
            var vacationWindow = new AddNewSickLeave(_currentEmployeeId, currentId);
            vacationWindow.ShowDialog();

            LoadVacationsInfo();
        }

        // Редактирование декретного отпуска
        private void EditMaternityLeave_Click(object sender, RoutedEventArgs e)
        {
            int currentId = (MaternityLeaveDataGrid.SelectedItem as MaternityLeaveData).Id;
            var vacationWindow = new AddNewMaternityLeave(_currentEmployeeId, currentId);
            vacationWindow.ShowDialog();

            LoadVacationsInfo();
        }

        // Метод увольнения сотрудника
        private void DismissEmployee_Click(object sender, RoutedEventArgs e)
        {
            var window = new DismissEmployeeWindow(_currentEmployeeId);
            window.ShowDialog();
            this.Close();
        }

        #endregion

        private void SaveNewEmployee(int gender, int documentType, int contractId, int familyStatusId, int departmentId, double salary)
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            if (!Directory.Exists(Path.Combine(currentDirectory, "Images"))) // Проверяем существование папки Images
            {
                // Если ее нет, то создаем
                Directory.CreateDirectory(Path.Combine(currentDirectory, "Images"));
            }

            var guid = Guid.NewGuid().ToString() + _imageExtension; // Создаем новый уникальный идентификатор для имени файла

            var employeeInfo = new EmployeeInfo // Создадим объект, чтобы не передавать в запрос кучу параметров
            {
                Name = NameTextBox.Text,
                Gender = gender,
                BirthDate = BirthDatePicker.DisplayDate,
                Inn = InnTextBox.Text,
                Snils = SnilsTextBox.Text,
                DocumentType = documentType,
                DocumentNumber = DocumentNumberTextBox.Text,
                Address = AddressTextBox.Text,
                Phone = PhoneTextBox.Text,
                Email = EmailTextBox.Text,
                EmployeeBook = EmplHistoryTextBox.Text,
                FamilyStatus = familyStatusId,
                Contract = contractId,
                Department = departmentId,
                Position = PositionTextBox.Text,
                Salary = salary,
                ImageName = guid
            };

            var result = GlobalStaticParameters.Database.AddNewEmployee(employeeInfo);
            if (!result) // Если запрос не выполнился, то не сохраняем картинку
                return;

            File.Copy(_imagePath, Path.Combine(currentDirectory, "Images", guid));
            this.Close();
        }

        // Обновление данных сотрудника
        private void UpdateEmployee(int gender, int documentType, int contractId, int familyStatusId, int departmentId, double salary)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string currentImageFilename = Path.GetFileName(_imagePath);
            string guid = Path.GetFileName(_imagePath);

            if (!string.IsNullOrEmpty(_imageFilename) && !_imageFilename.Equals(currentImageFilename)) // Если картинка была изменена, то удаляем старую и загружаем новую
            {
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", _imageFilename);
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }

                if (!Directory.Exists(Path.Combine(currentDirectory, "Images"))) // Проверяем существование папки Images
                {
                    // Если ее нет, то создаем
                    Directory.CreateDirectory(Path.Combine(currentDirectory, "Images"));
                }

                guid = Guid.NewGuid().ToString() + _imageExtension; // Создаем новый уникальный идентификатор для имени файла
                File.Copy(_imagePath, Path.Combine(currentDirectory, "Images", guid));
            }

            var employeeInfo = new EmployeeInfo
            {
                Name = NameTextBox.Text,
                Gender = gender,
                BirthDate = BirthDatePicker.DisplayDate,
                Inn = InnTextBox.Text,
                Snils = SnilsTextBox.Text,
                DocumentType = documentType,
                DocumentNumber = DocumentNumberTextBox.Text,
                Address = AddressTextBox.Text,
                Phone = PhoneTextBox.Text,
                Email = EmailTextBox.Text,
                EmployeeBook = EmplHistoryTextBox.Text,
                FamilyStatus = familyStatusId,
                Contract = contractId,
                Department = departmentId,
                Position = PositionTextBox.Text,
                Salary = salary,
                ImageName = guid
            };

            GlobalStaticParameters.Database.UpdateEmployee(employeeInfo, _currentEmployeeId);
            this.Close();
        }

        #region Загрузки

        // Метод загрузки всех данных на форме
        private void LoadFormData()
        {
            LoadComboBoxData();
            BirthDatePicker.Text = DateTime.Now.ToString();

            if (_currentEmployeeId != -1)
            {
                LoadDataFromDatabase();
            }
        }

        // Заполнение ComboBox
        private void LoadComboBoxData()
        {
            FamilyStatusesComboBox.ItemsSource = GlobalStaticParameters.Database.GetFamilyStatuses();
            DocumentTypesComboBox.ItemsSource = GlobalStaticParameters.Database.GetDocumentTypes();
            DepartmentsComboBox.ItemsSource = GlobalStaticParameters.Database.GetDepartmentsList();
        }

        // Автозаполнение данных сотрудника из базы
        private void LoadDataFromDatabase()
        {
            VacationsPanel.Visibility =
            DismissButton.Visibility = Visibility.Visible; // Отображаем панель отпусков/больничных
            var employeeInfo = GlobalStaticParameters.Database.GetEmployeeData(_currentEmployeeId);

            var contract = GlobalStaticParameters.Database.GetContractDataByFilename(employeeInfo.ContractFilename);
            _contractId = contract.ContractId;
            _contractNumber = contract.ContractNumber;

            // Заполнение всех полей
            NameTextBox.Text = employeeInfo.Name;
            BirthDatePicker.Text = employeeInfo.BirthDate.ToString();
            AddressTextBox.Text = employeeInfo.Address;
            InnTextBox.Text = employeeInfo.Inn;
            SnilsTextBox.Text = employeeInfo.Snils;
            EmplHistoryTextBox.Text = employeeInfo.EmployeeBook;
            PhoneTextBox.Text = employeeInfo.Phone;
            EmailTextBox.Text = employeeInfo.Email;
            DocumentNumberTextBox.Text = employeeInfo.DocumentNumber;
            PositionTextBox.Text = employeeInfo.Position;
            SalaryTextBox.Text = employeeInfo.Salary.ToString();

            if (employeeInfo.Gender == 1)
            {
                FemaleRadioButton.IsChecked = true;
            }

            FamilyStatusesComboBox.SelectedValue = employeeInfo.FamilyStatus;
            DocumentTypesComboBox.SelectedValue = employeeInfo.DocumentType;
            DepartmentsComboBox.SelectedValue = employeeInfo.Department;
            ContractTextBox.Text = _contractNumber;

            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", employeeInfo.ImageName); // Получаем директорию хранения картинки
            _imageFilename = employeeInfo.ImageName;
            _imageBytes = File.ReadAllBytes(_imagePath); // Считываем данные файла
            LoadProfileImage();
            LoadVacationsInfo();
        }

        // Метод загрузки картинки в элемент Image
        private void LoadProfileImage()
        {
            var memoryStream = new MemoryStream(_imageBytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            AvatarImage.Source = bitmap;
        }

        // Метод загрузки данных об отпусках сотрудника
        private void LoadVacationsInfo()
        {
            var vacationInfo = GlobalStaticParameters.Database.GetVacationInfo(_currentEmployeeId);

            if (vacationInfo == null)
            {
                return;
            }

            VacationLabel.Visibility = Visibility.Visible;
            string message = string.Empty;

            switch (vacationInfo.VacationType)
            {
                case 0:
                    message = string.Format("В отпуске с {0} по {1}",
                        vacationInfo.StartDate.ToString("dd.MM.yyyy"),
                        vacationInfo.EndDate.ToString("dd.MM.yyyy"));
                    break;
                case 1:
                    message = string.Format("На больничном с {0} по {1}",
                        vacationInfo.StartDate.ToString("dd.MM.yyyy"),
                        vacationInfo.EndDate.ToString("dd.MM.yyyy"));
                    break;
                case 2:
                    message = string.Format("В декретном отпуске с {0} по {1}",
                        vacationInfo.StartDate.ToString("dd.MM.yyyy"),
                        vacationInfo.EndDate.ToString("dd.MM.yyyy"));
                    break;
            }

            VacationLabel.Content = message;

            LoadVacationTables();
        }

        // Загрузка таблиц с отпусками
        private void LoadVacationTables()
        {
            VacationsDataGrid.ItemsSource = GlobalStaticParameters.Database.GetVacationData(_currentEmployeeId);
            SickLeavesDataGrid.ItemsSource = GlobalStaticParameters.Database.GetSickLeaves(_currentEmployeeId);
            MaternityLeaveDataGrid.ItemsSource = GlobalStaticParameters.Database.GetMaternityLeaves(_currentEmployeeId);
        }

        #endregion     

        #region Валидация
        // Метод для валидации правильности заполнения полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                errorText += "- Не заполнено поле \"ФИО\"\n";
            }

            if (DateTime.Parse(BirthDatePicker.Text).AddYears(18) > DateTime.Today)
            {
                errorText += "- Возраст сотрудника не может быть меньше 18 лет\n";
            }

            if (string.IsNullOrEmpty(AddressTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Адрес\"\n";
            }

            if (string.IsNullOrEmpty(InnTextBox.Text))
            {
                errorText += "- Не заполнено поле \"ИНН\"\n";
            }
            else
            {
                if (Regex.IsMatch(InnTextBox.Text, @"/^\d+$/"))
                {
                    errorText += "- Поле \"ИНН\" может содержать только цифры\n";
                }
            }

            if (string.IsNullOrEmpty(SnilsTextBox.Text))
            {
                errorText += "- Не заполнено поле \"СНИЛС\"\n";
            }
            else
            {
                if (Regex.IsMatch(SnilsTextBox.Text, @"/^\d+$/"))
                {
                    errorText += "- Поле \"СНИЛС\" может содержать только цифры\n";
                }
            }

            if (string.IsNullOrEmpty(EmplHistoryTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Номер трудовой книжки\"\n";
            }

            if (string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Телефон\"\n";
            }

            if (string.IsNullOrEmpty(DocumentNumberTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Номер документа\"\n";
            }

            if (string.IsNullOrEmpty(PositionTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Должность\"\n";
            }

            if (string.IsNullOrEmpty(ContractTextBox.Text))
            {
                errorText += "- Не выбран трудовой договор";
            }

            if (string.IsNullOrEmpty(SalaryTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Оклад\"\n";
            }
            else
            {
                if (!Regex.IsMatch(SalaryTextBox.Text, @"^(0|([1-9][0-9]*))(,[0-9]+)?$"))
                {
                    errorText += "- Поле \"Оклад\" не соответствует десятичному формату числа\n";
                }
            }

            if (FamilyStatusesComboBox.SelectedIndex == -1)
            {
                errorText += "- Не выбрано семейное положение";
            }

            if (DocumentTypesComboBox.SelectedIndex == -1)
            {
                errorText += "- Не выбран тип документа, удостоверяющего личность";
            }

            if (DepartmentsComboBox.SelectedIndex == -1)
            {
                errorText += "- Не выбран отдел";
            }

            if (string.IsNullOrEmpty(_imagePath))
            {
                errorText += "- Не выбрана фотография сотрудника";
            }

            if (!string.IsNullOrEmpty(errorText))
            {
                MessageBox.Show(string.Format("Во время проверки правильности заполнения полей возникли ошибки: \n{0}", errorText),
                    "Ошибка при валидации полей",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        #endregion
    }
}
