using HR_Automation_System.Classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeFilterWindow.xaml
    /// </summary>
    public partial class EmployeeFilterWindow : Window
    {
        public FilterParameters Filter;

        public EmployeeFilterWindow()
        {
            InitializeComponent();
            List<BookClasses.Department> tempList = GlobalStaticParameters.Database.GetDepartmentsList();
            tempList.Insert(0, new BookClasses.Department { DepartmentId = -1, DepartmentName = "Не выбран" }); // Добавляем в начало элемент не выбран с кодом -1
            DepartmentsComboBox.ItemsSource = tempList;
        }

        // Кнопка "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Кнопка "Поиск"
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Filter = new FilterParameters
            {
                Name = !string.IsNullOrEmpty(NameTextBox.Text) ? NameTextBox.Text : null,
                DepartmentId = (int)DepartmentsComboBox.SelectedValue,
                BirthDate = !string.IsNullOrEmpty(BirthDatePicker.Text) ? DateTime.Parse(BirthDatePicker.Text) : DateTime.MinValue,
                StartDate = !string.IsNullOrEmpty(ContractDatePicker.Text) ? DateTime.Parse(ContractDatePicker.Text) : DateTime.MinValue
            };

            this.DialogResult = true;
            this.Close();
        }
    }
}
