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
            LoadDataGrid();
        }

        // Добавление нового сотрудника
        private void AddNewEmployee_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new EmployeeProfileWindow();
            window.ShowDialog();
            LoadDataGrid();
        }

        // Обновление списка в таблице
        private void LoadDataGrid()
        {
            var employees = GlobalStaticParameters.Database.GetEmployeesRows();
            EmployeesList.ItemsSource = employees;
        }
    }
}
