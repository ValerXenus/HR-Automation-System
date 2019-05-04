using HR_Automation_System.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for AddNewContract.xaml
    /// </summary>
    public partial class AddNewContract : Window
    {
        public AddNewContract()
        {
            InitializeComponent();
            ContractDatePicker.Text = DateTime.Now.ToString();
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

        // Кнопка "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            string currecntDirectory = Directory.GetCurrentDirectory();
            string extention = Path.GetExtension(FilenameTextBox.Text);

            if (!Directory.Exists(Path.Combine(currecntDirectory, "Contracts"))) // Проверяем существование папки Contracts
            {
                // Если ее нет, то создаем
                Directory.CreateDirectory(Path.Combine(currecntDirectory, "Contracts"));
            }

            var guid = Guid.NewGuid().ToString() + extention; // Создаем новый уникальный идентификатор для имени файла
            var result = GlobalStaticParameters.Database.AddNewContract(ContractNumberTextBox.Text, guid, ContractDatePicker.DisplayDate); // Добавляем трудовой договор в БД
            if (!result) // Если запрос не выполнился, то не сохраняем документ
                return;

            File.Copy(FilenameTextBox.Text, Path.Combine(currecntDirectory, "Contracts", guid));

            this.DialogResult = true; // Указываем, что результат успешен
            this.Close();
        }

        // Валидация правильности заполнения полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (string.IsNullOrEmpty(ContractNumberTextBox.Text))
            {
                errorText += "- Необходима указать номер трудового договора";
            }
            else
            {
                if (Regex.IsMatch(ContractNumberTextBox.Text, @"/^\d+$/"))
                {
                    errorText += "- Поле \"Номер трудового договора\" может содержать только цифры\n";
                }
            }

            if (string.IsNullOrEmpty(FilenameTextBox.Text))
            {
                errorText += "- Необходима выбрать файл трудового договора";
            }

            if (!File.Exists(FilenameTextBox.Text))
            {
                errorText += "- Указанный файл не найден";
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
