using HR_Automation_System.AdditionalWindows;
using HR_Automation_System.Classes;
using System.Windows.Controls;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for EmployeesListPage.xaml
    /// </summary>
    public partial class EmployeesListPage : Page
    {
        public EmployeesListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        // Добавление нового сотрудника
        private void AddNewEmployee_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new EmployeeProfileWindow();
            window.ShowDialog();
            RefreshDataGrid();
        }

        // Обновление списка в таблице
        private void RefreshDataGrid()
        {
            var employees = GlobalStaticParameters.Database.GetEmployeesRows();
            EmployeesList.ItemsSource = employees;
        }

        // Кнопка "Редактировать сотрудника"
        private void EditEmployee_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EmployeesList.SelectedIndex == -1) // Если не было выделено ни одной строки
                return;

            var employee = EmployeesList.SelectedItem as BookClasses.EmployeeRow;
            var window = new EmployeeProfileWindow(employee.EmployeeId);
            window.ShowDialog();

            RefreshDataGrid();
        }

        // Кнопка "Фильтр"
        private void FilterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var filterWindow = new EmployeeFilterWindow();
            filterWindow.ShowDialog();
        }
    }
}
