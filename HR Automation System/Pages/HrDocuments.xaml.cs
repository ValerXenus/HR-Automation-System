using HR_Automation_System.AdditionalWindows;
using HR_Automation_System.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for HrDocuments.xaml
    /// </summary>
    public partial class HrDocuments : Page
    {
        public HrDocuments()
        {
            InitializeComponent();
            RefreshTable();
        }

        // Метод обновления DataGrid
        private void RefreshTable()
        {
            DocumentsList.ItemsSource = GlobalStaticParameters.Database.GetHrDocumentsList();
        }

        // Кнопка "Добавить новый документ"
        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new AddNewDocument();
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                RefreshTable();
            }
        }

        // Кнопка "Редактировать документ"
        private void EditButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int currentId = (DocumentsList.SelectedItem as BookClasses.HrDocument).Id;

            var window = new AddNewDocument(currentId);
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                RefreshTable();
            }
        }

        // Кнопка "Сохранить документ"
        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int currentId = (DocumentsList.SelectedItem as BookClasses.HrDocument).Id;
            var document = GlobalStaticParameters.Database.GetDocumentData(currentId);

            // Открываем диалог с выбором пути сохранения документа
            var dialog = new SaveFileDialog();
            string tempFilename = document.Name + Path.GetExtension(document.Filename);
            dialog.FileName = tempFilename;
            dialog.ShowDialog();

            if (!dialog.FileName.Equals(tempFilename)) // В случае если заголовок совпадает, то путь не был выбран
            {
                string fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents", document.Filename);

                if (File.Exists(fileDirectory))
                    File.Copy(fileDirectory, dialog.FileName); // сохраняем файл в выбранной пользователем директории
            }
        }

        // Кнопка "Удалить документ"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно удалить данный документ?",
                "Внимание",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                int currentId = (DocumentsList.SelectedItem as BookClasses.HrDocument).Id;
                var document = GlobalStaticParameters.Database.GetDocumentData(currentId);

                GlobalStaticParameters.Database.RemoveDocument(currentId);

                string path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", document.Filename);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                RefreshTable();
            }
        }
    }
}
