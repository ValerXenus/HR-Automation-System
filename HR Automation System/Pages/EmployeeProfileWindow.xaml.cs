using HR_Automation_System.Classes;
using System;
using System.Windows;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for EmployeeProfileWindow.xaml
    /// </summary>
    public partial class EmployeeProfileWindow : Window
    {
        // Конструктор без параметра Employee (добавление нового сотрудника)
        public EmployeeProfileWindow()
        {
            InitializeComponent();
            LoadFormData(false);
        }

        // Метод загрузки всех данных на форме
        private void LoadFormData(bool isNewEmployee)
        {
            if (isNewEmployee)
            {
                LoadDataFromDatabase();
            }

            // Заполнение ComboBox
            FamilyStatusesComboBox.ItemsSource = GlobalStaticParameters.Database.GetFamilyStatuses();
            DocumentTypesComboBox.ItemsSource = GlobalStaticParameters.Database.GetDocumentTypes();
            DepartmentsComboBox.ItemsSource = GlobalStaticParameters.Database.GetDepartmentsList();

            BirthDatePicker.Text = 
            HiringDatePicker.Text = DateTime.Now.ToString();
        }

        // Автозаполнение данных сотрудника из базы
        private void LoadDataFromDatabase()
        {
            VacationsPanel.Visibility = Visibility.Visible; // Отображаем панель отпусков/больничных
        }
    }
}
