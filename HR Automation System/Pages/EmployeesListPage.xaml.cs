using HR_Automation_System.AdditionalWindows;
using HR_Automation_System.Classes;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for EmployeesListPage.xaml
    /// </summary>
    public partial class EmployeesListPage : Page
    {
        public ObservableCollection<BookClasses.EmployeeRow> _overallEmployees; // Коллекция хранения списка всех сотрудников
        public ObservableCollection<BookClasses.EmployeeRow> _filteredCollection; // Коллекция хранения отфильтрованного списка сотрудников

        public EmployeesListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        // Добавление нового сотрудника
        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var window = new EmployeeProfileWindow();
            window.ShowDialog();
            RefreshDataGrid();
        }

        // Обновление списка в таблице
        private void RefreshDataGrid()
        {
            _overallEmployees = GlobalStaticParameters.Database.GetEmployeesRows();
            EmployeesList.ItemsSource = _overallEmployees;
        }

        // Обновление списка в таблице по параметрам фильтрации
        private void RefreshFitlteredDataGrid(FilterParameters filter)
        {
            _filteredCollection = new ObservableCollection<BookClasses.EmployeeRow>();

            foreach (var employee in _overallEmployees)
            {
                // Параметр прохождения проверки фильтра
                bool nameParam = true;
                bool birthDateParam = true;
                bool contractDateParam = true;
                bool departmentParam = true;

                if (filter.Name != null && !employee.EmployeeName.Contains(filter.Name))
                {
                    nameParam = false;
                }

                if (filter.BirthDate != DateTime.MinValue && employee.BirthDate.Date != filter.BirthDate.Date)
                {
                    birthDateParam = false;
                }

                if (filter.StartDate != DateTime.MinValue && employee.ContractDate.Date != filter.StartDate.Date)
                {
                    contractDateParam = false;
                }

                if (filter.DepartmentId != -1 && employee.DepartmentId != filter.DepartmentId)
                {
                    departmentParam = false;
                }

                // Общая проверка после фильтрации по параметрам
                if (nameParam && birthDateParam && contractDateParam && departmentParam)
                {
                    _filteredCollection.Add(employee);
                }
            }

            EmployeesList.ItemsSource = _filteredCollection;
        }

        // Кнопка "Редактировать сотрудника"
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesList.SelectedIndex == -1) // Если не было выделено ни одной строки
                return;

            var employee = EmployeesList.SelectedItem as BookClasses.EmployeeRow;
            var window = new EmployeeProfileWindow(employee.EmployeeId);
            window.ShowDialog();

            RefreshDataGrid();
        }

        // Кнопка "Открыть трудовой договор"
        private void OpenContract_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesList.SelectedIndex == -1) // Если не было выделено ни одной строки
                return;

            var employee = EmployeesList.SelectedItem as BookClasses.EmployeeRow;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Contracts", employee.ContractFilename);

            if (!File.Exists(path))
            {
                MessageBox.Show("Файл трудового договора не найден",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Process.Start(path); // Открываем файл трудового договора
        }

        // Кнопка "Назначить аттестацию"
        private void SetGraduation_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesList.SelectedIndex == -1) // Если не было выделено ни одной строки
                return;

            var employee = EmployeesList.SelectedItem as BookClasses.EmployeeRow;
            var window = new SetGraduationWindow(employee.EmployeeId, employee.EmployeeName, employee.Email);
            window.ShowDialog();
        }

        // Кнопка "Фильтр"
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filterWindow = new EmployeeFilterWindow();
            filterWindow.ShowDialog();

            if ((bool)filterWindow.DialogResult)
            {
                RefreshFitlteredDataGrid(filterWindow.Filter);
            }
        }
    }
}
