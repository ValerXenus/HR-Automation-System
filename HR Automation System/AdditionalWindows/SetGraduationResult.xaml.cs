using HR_Automation_System.Classes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for SetGraduationResult.xaml
    /// </summary>
    public partial class SetGraduationResult : Window
    {
        private int _employeeId; // Код сотрудника
        private int _graduationId; // Код аттестации
        private int _contractId; // Код трудового договора

        public SetGraduationResult(int employeeId, int graduationId, int contractId, string employeeName)
        {
            InitializeComponent();
            _employeeId = employeeId;
            _graduationId = graduationId;
            _contractId = contractId;
            EmployeeTextBox.Text = employeeName;
            LoadComboBoxes();
        }

        // Кнопка "Применить"
        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            var requestResult = GlobalStaticParameters.Database.AddNewEmployeeInDepartment(
                (int)DepartmentsComboBox.SelectedValue,
                _employeeId,
                PositionTextBox.Text,
                double.Parse(SalaryTextBox.Text),
                _contractId);
            if (!requestResult)
                return;

            var setGraduationResult = GlobalStaticParameters.Database.SetGraduationResult(_graduationId,
                (int)GraduationResultComboBox.SelectedValue);

            if (!setGraduationResult)
                return;

            this.Close(); // Закрываем текущее окно
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

        // Загрузка комбобокса
        private void LoadComboBoxes()
        {
            var resultList = new List<Result>
            {
                new Result {Id = 1, Name = "Аттестация пройдена"},
                new Result {Id = 0, Name = "Аттестация не пройдена"},
            };
            GraduationResultComboBox.ItemsSource = resultList;

            DepartmentsComboBox.ItemsSource = GlobalStaticParameters.Database.GetDepartmentsList();
        }

        // Валидация правильности заполнения полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

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
                if (!Regex.IsMatch(SalaryTextBox.Text, @"^(0|([1-9][0-9]*))(,[0-9]+)?$"))
                {
                    errorText += "- Поле \"Оклад\" не соответствует десятичному формату числа\n";
                }
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

        // Класс хранение результатов
        // -1 - оценка не установлена
        // 0 - аттестация не пройдена
        // 1 - аттестация пройдена
        class Result
        {
            // Код результата
            public int Id { get; set; }

            // Наименование результата
            public string Name { get; set; }
        }
    }
}
