using HR_Automation_System.Classes;
using System;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for AddNewVacation.xaml
    /// </summary>
    public partial class AddNewVacation : Window
    {
        public int _employeeId;
        public int _vacationId; // Если это режим редактирования, то код отпуска будетне равен -1

        public AddNewVacation(int employeeId, int vacationId = -1)
        {
            InitializeComponent();
            _employeeId = employeeId;
            _vacationId = vacationId;

            StartDatePicker.Text =
            EndDatePicker.Text = DateTime.Now.ToString();

            if (_vacationId != -1)
            {
                LoadWindowFields();
            }
        }

        // Кнопка "Добавить отпуск"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            bool result;

            if (_vacationId != -1) // Если это режим редактирования отпуска
            {
                result = true;
            }
            else
            {
                // Добавляем отпуск
                result = GlobalStaticParameters.Database.AddNewVacation(
                    VacationNameTextBox.Text,
                    DateTime.Parse(StartDatePicker.Text),
                    DateTime.Parse(EndDatePicker.Text),
                    _employeeId);
            }

            if (result)
            {
                this.Close();
            }
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

        // автозаполнение полей для редактирования
        private void LoadWindowFields()
        {
            var vacation = GlobalStaticParameters.Database.GetVacationRecord(_vacationId);

            VacationNameTextBox.Text = vacation.Name;
            StartDatePicker.Text = vacation.StartDate.ToString();
            EndDatePicker.Text = vacation.EndDate.ToString();
        }

        // Метод проверки заполненных полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (string.IsNullOrEmpty(VacationNameTextBox.Text))
            {
                errorText += "- Не заполнено поле \"Тип отпуска\"";
            }

            if (DateTime.Parse(StartDatePicker.Text) > DateTime.Parse(EndDatePicker.Text))
            {
                errorText += "- Дата начала отпуска не может быть позже даты окончания";
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
