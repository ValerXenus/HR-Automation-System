using HR_Automation_System.Classes;
using System;
using System.Windows;


namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for AddNewSickLeave.xaml
    /// </summary>
    public partial class AddNewSickLeave : Window
    {
        public int _employeeId;
        public int _sickLeaveId; // Код больничного

        public AddNewSickLeave(int employeeId, int sickLeaveId = -1)
        {
            InitializeComponent();
            _employeeId = employeeId;
            _sickLeaveId = sickLeaveId;

            StartDatePicker.Text =
            EndDatePicker.Text = DateTime.Now.ToString();

            if (_sickLeaveId != -1)
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

            if (_sickLeaveId != -1)
            {
                var sickLeave = new SickLeaveData
                {
                    Id = _sickLeaveId,
                    StartDate = DateTime.Parse(StartDatePicker.Text),
                    EndDate = DateTime.Parse(EndDatePicker.Text)
                };

                result = GlobalStaticParameters.Database.SaveSickLeave(sickLeave);
            }
            else
            {
                // Добавляем отпуск
                result = GlobalStaticParameters.Database.AddNewSickLeave(
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

        // Автозаполнение полей для редактирования
        private void LoadWindowFields()
        {
            var sickLeave = GlobalStaticParameters.Database.GetSickLeaveRecord(_sickLeaveId);

            StartDatePicker.Text = sickLeave.StartDate.ToString();
            EndDatePicker.Text = sickLeave.EndDate.ToString();
        }

        // Метод проверки заполненных полей
        private bool ValidateFields()
        {
            string errorText = string.Empty;

            if (DateTime.Parse(StartDatePicker.Text) > DateTime.Parse(EndDatePicker.Text))
            {
                errorText += "- Дата начала больничного не может быть позже даты окончания";
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
