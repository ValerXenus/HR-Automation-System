using HR_Automation_System.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for AddNewMaternityLeave.xaml
    /// </summary>
    public partial class AddNewMaternityLeave : Window
    {
        private int _employeeId;

        public AddNewMaternityLeave(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;

            StartDatePicker.Text =
            EndDatePicker.Text = DateTime.Now.ToString();
        }

        // Кнопка "Добавить отпуск"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            var result = GlobalStaticParameters.Database.AddNewMaternityLeave(
                OrderNumberTextBox.Text,
                DateTime.Parse(StartDatePicker.Text),
                DateTime.Parse(EndDatePicker.Text),
                _employeeId);

            if (!result) // Если запрос не выполнился, то не сохраняем документ
                return;

            this.DialogResult = true; // Указываем, что результат успешен
            this.Close();
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
        
        // Метод проверки заполненных полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (string.IsNullOrEmpty(OrderNumberTextBox.Text))
            {
                errorText += "- Не указан номер приказа";
            }

            if (DateTime.Parse(StartDatePicker.Text) > DateTime.Parse(EndDatePicker.Text))
            {
                errorText += "- Дата начала декретного отпуска не может быть позже даты окончания";
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
    }
}
