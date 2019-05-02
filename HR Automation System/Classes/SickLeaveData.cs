using System;

namespace HR_Automation_System.Classes
{
    // Класс хранения данных о больничных
    public class SickLeaveData
    {
        // Код больничного
        public int Id { get; set; }

        // Дата начала
        public DateTime StartDate { get; set; }

        // Дата окончания
        public DateTime EndDate { get; set; }
    }
}
