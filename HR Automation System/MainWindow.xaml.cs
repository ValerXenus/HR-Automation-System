using HR_Automation_System.Classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HR_Automation_System
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Конструктор окна
        public MainWindow(DatabaseUtility db, int userId)
        {
            InitializeComponent();
            db.Disconnect();
        }
    }
}
