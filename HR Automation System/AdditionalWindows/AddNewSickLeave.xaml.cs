using HR_Automation_System.Classes;
using System;
using System.Windows;


namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for AddNewSickLeave.xaml
    /// </summary>
    public partial class AddNewSickLeave : Window
    {
        public int _employeeId;

        public AddNewSickLeave(int employeeId)
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

            // Добавляем отпуск
            var result = GlobalStaticParameters.Database.AddNewSickLeave(
                DateTime.Parse(StartDatePicker.Text),
                DateTime.Parse(EndDatePicker.Text),
                _employeeId);

            if (result)
            {
                this.Close();
            }
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

            if (DateTime.Parse(StartDatePicker.Text) > DateTime.Parse(EndDatePicker.Text))
            {
                errorText += "- Дата начала больничного не может быть позже даты окончания";
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
