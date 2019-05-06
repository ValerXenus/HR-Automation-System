using HR_Automation_System.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for AddNewDocument.xaml
    /// </summary>
    public partial class AddNewDocument : Window
    {
        public int _documentId;
        public string _currentFilename;

        public AddNewDocument(int documentId = -1) // Если это режим создания нового документ, то код будет равен -1 
        {
            InitializeComponent();
            _documentId = documentId;

            if (_documentId != -1)
            {
                LoadWindowData();
            }
        }

        // Кнопка "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            if (_documentId != -1)
            {
                UpdateDocument();
            }
            else
            {
                SaveNewDocument();
            }
        }

        // Выбор документа
        private void SelectDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();
            result.Filter = "Document Files(*.DOC;*.DOCX;*.PDF)|*.DOC;*.DOCX;*.PDF";
            result.Multiselect = false;
            result.ShowDialog();

            // Если файл каким-либо образом не был выбран, то ничего не делаем
            if (result.FileName == string.Empty && File.Exists(result.FileName))
            {
                return;
            }

            FilenameTextBox.Text = result.FileName;
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

        // Сохранение документа
        private void SaveNewDocument()
        {
            string currecntDirectory = Directory.GetCurrentDirectory();
            string extention = Path.GetExtension(FilenameTextBox.Text);

            if (!Directory.Exists(Path.Combine(currecntDirectory, "Documents"))) // Проверяем существование папки Contracts
            {
                // Если ее нет, то создаем
                Directory.CreateDirectory(Path.Combine(currecntDirectory, "Documents"));
            }

            var guid = Guid.NewGuid().ToString() + extention; // Создаем новый уникальный идентификатор для имени файла

            var result = GlobalStaticParameters.Database.AddNewDocument(DocumentNameTextBox.Text, guid);
            if (!result) // Если запрос не выполнился, то не сохраняем документ
                return;

            File.Copy(FilenameTextBox.Text, Path.Combine(currecntDirectory, "Documents", guid));
            this.DialogResult = true;
            this.Close();
        }

        // Сохранение изменений после редактирования
        private void UpdateDocument()
        {
            string currecntDirectory = Directory.GetCurrentDirectory();
            string extention = Path.GetExtension(FilenameTextBox.Text);
            string guid = _currentFilename;

            if (!FilenameTextBox.Text.Equals(_currentFilename)) // Если картинка не поменялась, то ничего не делаем
            {
                if (!Directory.Exists(Path.Combine(currecntDirectory, "Documents"))) // Проверяем существование папки Contracts
                {
                    // Если ее нет, то создаем
                    Directory.CreateDirectory(Path.Combine(currecntDirectory, "Documents"));
                }

                guid = Guid.NewGuid().ToString() + extention; // Создаем новый уникальный идентификатор для имени файла
                File.Copy(FilenameTextBox.Text, Path.Combine(currecntDirectory, "Documents", guid));
                File.Delete(Path.Combine(currecntDirectory, "Documents", _currentFilename)); // Удаляем старый документ  
            }

            var document = new BookClasses.HrDocument
            {
                Id = _documentId,
                Name = DocumentNameTextBox.Text,
                Filename = guid
            };

            var result = GlobalStaticParameters.Database.UpdateDocument(document);
            if (!result) // Если запрос не выполнился, то удаляем документ
            {
                File.Delete(Path.Combine(currecntDirectory, "Documents", guid));
                return;
            }
            
            this.DialogResult = true;
            this.Close();
        }

        // Автозаполнение полей данными
        private void LoadWindowData()
        {
            var document = GlobalStaticParameters.Database.GetDocumentData(_documentId);
            DocumentNameTextBox.Text = document.Name;
            _currentFilename = document.Filename;
            FilenameTextBox.Text = _currentFilename;
        }

        // Валидация правильности заполнения полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (string.IsNullOrEmpty(DocumentNameTextBox.Text))
            {
                errorText += "- Необходима указать наименование документа\n";
            }

            if (string.IsNullOrEmpty(FilenameTextBox.Text))
            {
                errorText += "- Необходима выбрать файл документа\n";
            }
            else
            {
                if (!FilenameTextBox.Text.Equals(_currentFilename) && !File.Exists(FilenameTextBox.Text))
                {
                    errorText += "- Указанный файл не найден\n";
                }
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
