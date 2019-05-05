using System;

namespace HR_Automation_System.Classes
{
    // Класс, содержащий информацию об обычном отпуске
    public class VacationData
    {
        // Код отпуска
        public int Id { get; set; }

        // Название отпуска
        public string Name { get; set; }

        // Дата начала
        public DateTime StartDate { get; set; }

        // Дата окончания
        public DateTime EndDate { get; set; }
    }
}
