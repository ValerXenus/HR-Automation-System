﻿using System;
using System.Collections.ObjectModel;
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

            var employees = new ObservableCollection<Employee>
            {
                new Employee { EmployeeName = "Иванов Иван Иванович", Position = "Уборщик", Department = "Завхоз"},
                new Employee { EmployeeName = "Долгов Герман Борисович", Position = "Системный администратор", Department = "ИТ отдел"}
            };

            EmployeesList.ItemsSource = employees;
        }

        public class Employee
        {
            // ФИО сотрудника
            public string EmployeeName { get; set; }

            // Должность
            public string Position { get; set; }

            // Отдел
            public string Department { get; set; }
        }
    }
}