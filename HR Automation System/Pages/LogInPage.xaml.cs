using HR_Automation_System.Classes;
using System.Text.RegularExpressions;
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == string.Empty)
            {
                MessageBox.Show("Заполните поле \"Логин\"", "Ошибка");
                return;
            }

            if (PasswordTextBox.Password == string.Empty)
            {
                MessageBox.Show("Заполните поле \"Пароль\"", "Ошибка");
                return;
            }

            // Проверка формата логина
            string pattern = @"^[a-zA-Z]{1,50}-[a-zA-Z]{1,2}$"; // формат: (любые латинские буквы от 1 до 50)-(любые латинские буквы от 1 до 2)
            if (!Regex.IsMatch(LoginTextBox.Text, pattern))
            {
                MessageBox.Show("Логин не соответствует формату \"name-f\"", "Ошибка");
                return;
            }

            // Вызываем объект для подключения к БД
            try
            {
                var db = new DatabaseUtility();
                int userId = db.CheckUserAuth(LoginTextBox.Text, PasswordTextBox.Password); // Запрос на поиск сочетания логина и пароля

                if (userId == -1)
                {
                    db.Disconnect();
                    MessageBox.Show("Логин или пароль введены неверно!", "Ошибка");
                    return;
                }

                userAuth(db, userId);
            }
            catch
            {

            }
        }

        // Авторизация пользователя и переход на главное окно программы
        private void userAuth(DatabaseUtility db, int userId)
        {
            GlobalStaticParameters.Database = db;
            GlobalStaticParameters.UserId = userId;
            var window = new MainWindow();
            window.Show();
            this.Close();
        }

        // Добавляем/убираем водяной текст для поля "Логин"
        private void LoginTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (LoginTextBox.Text == string.Empty)
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
            if (PasswordTextBox.Password == string.Empty)
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
