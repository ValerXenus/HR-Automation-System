using HR_Automation_System.AdditionalWindows;
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
        private byte[] _imageBytes; // Байты фотографии сотрудника
        private string _imagePath;
        private string _imageExtension;
        
        // Конструктор без параметра Employee (добавление нового сотрудника)
        public EmployeeProfileWindow()
        {
            InitializeComponent();
            LoadFormData(false);
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
            int contractId = (int)ContractsComboBox.SelectedValue;

            int gender = 0;
            if (FemaleRadioButton.IsChecked == true)
                gender = 1;

            SaveNewEmployee(gender, documentType, contractId, familyStatusId, departmentId);
        }

        // Добавление нового трудового договора
        private void AddNewContract_Click(object sender, RoutedEventArgs e)
        {
            var newContractWindow = new AddNewContract();
            newContractWindow.ShowDialog();

            ContractsComboBox.ItemsSource = GlobalStaticParameters.Database.GetContractsList();
            ContractsComboBox.SelectedIndex = 0; // Выбираем первый элемент из списка
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

        #endregion

        private void SaveNewEmployee(int gender, int documentType, int contractId, int familyStatusId, int departmentId)
        {
            string currecntDirectory = Directory.GetCurrentDirectory();

            if (!Directory.Exists(Path.Combine(currecntDirectory, "Images"))) // Проверяем существование папки Contracts
            {
                // Если ее нет, то создаем
                Directory.CreateDirectory(Path.Combine(currecntDirectory, "Images"));
            }

            var guid = Guid.NewGuid().ToString() + _imageExtension; // Создаем новый уникальный идентификатор для имени файла

            var result = GlobalStaticParameters.Database.AddNewEmployee(NameTextBox.Text, gender, BirthDatePicker.DisplayDate, InnTextBox.Text,
                SnilsTextBox.Text, documentType, DocumentNumberTextBox.Text, AddressTextBox.Text, PhoneTextBox.Text, EmailTextBox.Text,
                EmplHistoryTextBox.Text, familyStatusId, contractId, departmentId, PositionTextBox.Text, double.Parse(SalaryTextBox.Text), guid);
            if (!result) // Если запрос не выполнился, то не сохраняем картинку
                return;

            File.Copy(_imagePath, Path.Combine(currecntDirectory, "Images", guid));
            this.Close();
        }

        // Метод загрузки всех данных на форме
        private void LoadFormData(bool isNewEmployee)
        {
            if (isNewEmployee)
            {
                LoadDataFromDatabase();
            }

            LoadComboBoxData();           

            BirthDatePicker.Text = DateTime.Now.ToString();
        }

        // Заполнение ComboBox
        private void LoadComboBoxData()
        {
            FamilyStatusesComboBox.ItemsSource = GlobalStaticParameters.Database.GetFamilyStatuses();
            DocumentTypesComboBox.ItemsSource = GlobalStaticParameters.Database.GetDocumentTypes();
            DepartmentsComboBox.ItemsSource = GlobalStaticParameters.Database.GetDepartmentsList();
            ContractsComboBox.ItemsSource = GlobalStaticParameters.Database.GetContractsList();
        }

        // Автозаполнение данных сотрудника из базы
        private void LoadDataFromDatabase()
        {
            VacationsPanel.Visibility = Visibility.Visible; // Отображаем панель отпусков/больничных
        }

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

            if (string.IsNullOrEmpty(SalaryTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Оклад\"\n";
            }
            else
            {
                if (!Regex.IsMatch(SnilsTextBox.Text, @"[+-]?([0-9]*[.])?[0-9]+"))
                {
                    errorText += "- Поле \"Оклад\" может содержать только числовое значение\n";
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

            if (ContractsComboBox.SelectedIndex == -1)
            {
                errorText += "- Не выбран трудовой договор";
            }

            if (string.IsNullOrEmpty(_imageExtension))
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
    }
}
