using System;

namespace HR_Automation_System.Classes
{
    // Класс хранения Декретных отпусков
    public class MaternityLeaveData
    {
        // Код декретного отпуска
        public int Id { get; set; }

        // Номер приказа
        public string OrderNumber { get; set; }

        // Дата начала
        public DateTime StartDate { get; set; }

        // Дата окончания
        public DateTime EndDate { get; set; }
    }
}
