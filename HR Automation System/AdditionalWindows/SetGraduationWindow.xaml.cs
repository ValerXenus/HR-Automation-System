using HR_Automation_System.Classes;
using System;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for SetGraduationWindow.xaml
    /// </summary>
    public partial class SetGraduationWindow : Window
    {
        private int _employeeId;
        private string _employeeName;
        private string _employeeEmail;

        public SetGraduationWindow(int employeeId, string employeeName, string employeeEmail)
        {
            InitializeComponent();
            _employeeId = employeeId;
            _employeeName = employeeName;
            _employeeEmail = employeeEmail;
            GraduationDatePicker.Text = DateTime.Now.ToString();
            EmployeeTextBox.Text = _employeeName;
        }

        // Кнопка "Назначить"
        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalStaticParameters.Database.CheckGraduation(_employeeId))
            {
                MessageBox.Show("На данного сотрудника уже имеется запись о предстоящей аттестации",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dbResult = GlobalStaticParameters.Database.AddNewGraduaiotion(_employeeId,
                DateTime.Parse(GraduationDatePicker.Text).ToString());
            if (!dbResult)
                return;

            EmailUtility emailClient = null;
            try
            {
                emailClient = new EmailUtility(_employeeEmail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при отправке почты:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var sendEmail = emailClient.SendGraduationMail(_employeeName,
                DateTime.Parse(GraduationDatePicker.Text).ToString("dd.MM.yyyy"));
            if (!sendEmail)
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
    }
}
