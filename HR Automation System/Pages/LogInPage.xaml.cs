using System.Windows;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Window
    {
        public LogInPage()
        {
            InitializeComponent();
        }

        // Добавляем/убираем водяной текст для поля "Логин"
        private void LoginTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (LoginTextBox.Text.Length == 0)
            {
                LoginWatermark.Visibility = Visibility.Visible;
            }
            else
            {
                LoginWatermark.Visibility = Visibility.Hidden;
            }
        }

        // Добавляем/убираем водяной текст для поля "Пароль"
        private void PasswordTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (PasswordTextBox.Password.Length == 0)
            {
                PasswordWatermark.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordWatermark.Visibility = Visibility.Hidden;
            }
        }        
    }
}
