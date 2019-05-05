using System;

namespace HR_Automation_System.Classes
{
    // Класс параметров фильтра
    public class FilterParameters
    {
        // ФИО сотрудника
        public string Name { get; set; }

        // Код отдела
        public int DepartmentId { get; set; }

        // Дата рождения
        public DateTime BirthDate { get; set; }

        // Дата заключения трудового договора
        public DateTime StartDate { get; set; }
    }
}
