using HR_Automation_System.Classes;
using System;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeFilterWindow.xaml
    /// </summary>
    public partial class EmployeeFilterWindow : Window
    {
        public EmployeeFilterWindow()
        {
            InitializeComponent();
            DepartmentsComboBox.ItemsSource = GlobalStaticParameters.Database.GetDepartmentsList();
        }

        // Кнопка "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
