using HR_Automation_System.Classes;
using System;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Логика взаимодействия для DismissEmployeeWindow.xaml
    /// </summary>
    public partial class DismissEmployeeWindow : Window
    {
        // Код сотрудника
        public int _employeeId;

        public DismissEmployeeWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            DismissDatePicker.Text = DateTime.Now.ToString();
        }

        // Кнопка "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите уволить данного сотрудника?",
                "Внимание",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                if (!ValidateFields())
                    return;

                var result = GlobalStaticParameters.Database.DismissEmployee(
                    _employeeId,
                    OrderNumberTextBox.Text,
                    ReasonTextBox.Text,
                    DateTime.Parse(DismissDatePicker.Text));

                if (!result)
                    return;

                this.Close(); 
            }
        }

        // Кнопка "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Валидация правильности заполнения полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (string.IsNullOrEmpty(OrderNumberTextBox.Text))
            {
                errorText += "- Необходима указать приказ";
            }

            if (string.IsNullOrEmpty(ReasonTextBox.Text))
            {
                errorText += "- Необходима указать причину увольнения";
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
