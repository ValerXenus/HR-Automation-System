using HR_Automation_System.Classes;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace HR_Automation_System
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool isMenuOpen; // Признак, открыто ли меню или свернуто

        // Конструктор окна
        public MainWindow()
        {
            InitializeComponent();
            LoadUiElements();
        }

        // Открыть/свернуть левое меню
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuOpen)
            {
                CloseLeftkMenu();
            }
            else
            {
                OpenLeftMenu();
            }
            isMenuOpen = !isMenuOpen;
        }
        
        // UI методы

        // Метод, загружающий UI элементы
        private void LoadUiElements()
        {
            isMenuOpen = true;
            ChangePage("EmployeesListPage");
        }

        // Открыть левое меню
        private void OpenLeftMenu()
        {
            DoubleAnimation anim_menu = new DoubleAnimation();
            anim_menu.From = 42;
            anim_menu.To = 200;
            anim_menu.Duration = TimeSpan.FromSeconds(0.2);
            LeftHandMenu.BeginAnimation(WidthProperty, anim_menu);
        }

        // Закрыть левое меню
        private void CloseLeftkMenu()
        {
            DoubleAnimation anim_menu = new DoubleAnimation();
            anim_menu.From = 200;
            anim_menu.To = 42;
            anim_menu.Duration = TimeSpan.FromSeconds(0.2);
            LeftHandMenu.BeginAnimation(WidthProperty, anim_menu);
        }

        // Смена страницы в главном окне
        private void ChangePage(string page_name)  // Changes page on page viewer
        {
            string link = "/HR Automation System;component/Pages/" + page_name + ".xaml";
            PageViewer.Source = new Uri(link, UriKind.Relative);
        }

        // Кнопка меню "Сотрудники"
        private void EmployeesPageButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("EmployeesListPage");
        }

        // Кнопка меню "Документы"
        private void DocumentsPageButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("HrDocuments");
        }

        // Кнопка меню "Статистика"
        private void StatisticsPageButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("StatisticsPage");
        }

        // Событие при закрытии главного окна приложения
        private void Application_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalStaticParameters.Database.Disconnect(); // Отключаем соединение с базой даннхы
        }
    }
}
