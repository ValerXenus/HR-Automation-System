﻿using HR_Automation_System.Classes;
using System;
using System.Collections.Generic;
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
        public MainWindow(DatabaseUtility db, int userId)
        {
            InitializeComponent();
            LoadUiElements();
            db.Disconnect();            
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
    }
}
