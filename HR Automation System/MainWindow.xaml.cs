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
        public MainWindow()
        {
            InitializeComponent();
            DatabaseUtility db = new DatabaseUtility();
            db.AddFamilyStatus("Тест");
            db.Disconnect();
        }
    }
}
