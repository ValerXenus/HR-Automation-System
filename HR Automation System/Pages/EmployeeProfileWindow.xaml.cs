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
        
        // Конструктор без параметра Employee (добавление нового сотрудника)
        public EmployeeProfileWindow()
        {
            InitializeComponent();
            LoadFormData(false);
        }        

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

            _imageBytes = File.ReadAllBytes(result.FileName); // Считываем данные файла
            LoadProfileImage();
        }       

        // Метод сохранения сотрудника
        private void SaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) // Если валидация не пройдена
                return;

            int familyStatusId = (int)FamilyStatusesComboBox.SelectedValue;
        }

        // Добавление нового трудового договора
        private void AddNewContract_Click(object sender, RoutedEventArgs e)
        {
            var newContractWindow = new AddNewContract();
            newContractWindow.ShowDialog();

            LoadComboBoxData();
        }

        // Метод загрузки всех данных на форме
        private void LoadFormData(bool isNewEmployee)
        {
            if (isNewEmployee)
            {
                LoadDataFromDatabase();
            }

            LoadComboBoxData();           

            BirthDatePicker.Text =
            HiringDatePicker.Text = DateTime.Now.ToString();
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
            VacationsPanel.Visibility = Visibility.Visible; // Отображаем панель отпусков/больничных
        }

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

            if (ContractsComboBox.SelectedIndex == -1)
            {
                errorText += "Не выбран трудовой договор";
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
