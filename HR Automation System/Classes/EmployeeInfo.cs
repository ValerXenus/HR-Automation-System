using System;

namespace HR_Automation_System.Classes
{
    // Класс, содержащий информацию о сотруднике
    public class EmployeeInfo
    {
        // ФИО сотрудника
        public string Name { get; set; }
        // Пол сотрудника
        public int Gender { get; set; }
        // Дата рождения
        public DateTime BirthDate { get; set; }
        // Адрес проживания
        public string Address { get; set; }
        // ИНН
        public string Inn { get; set; }
        // СНИЛС
        public string Snils { get; set; }
        // Номер трудовой книжки
        public string EmployeeBook { get; set; }
        // Номер телефона
        public string Phone { get; set; }
        // Адрес электронной почты
        public string Email { get; set; }
        // Код семейного положения
        public int FamilyStatus { get; set; }
        // Код документа
        public int DocumentType { get; set; }
        // Номер документа
        public string DocumentNumber { get; set; }
        // Код отдела
        public int Department { get; set; }
        // Должность
        public string Position { get; set; }
        // Код трудового договора
        public int Contract { get; set; }
        // Оклад
        public double Salary { get; set; }
        // Название картинки в папке Images
        public string ImageName { get; set; }
    }
}
