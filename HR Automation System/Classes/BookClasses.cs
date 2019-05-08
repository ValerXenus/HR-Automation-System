using System;

namespace HR_Automation_System.Classes
{
    // Класс для справочников
    public class BookClasses
    {
        // Класс "Семейное положение"
        public class FamilyStatuses
        {
            // Код семейного положения
            public int StatusId { get; set; }

            // Название семейного положения
            public string StatusName { get; set; }
        }

        // Класс "Типы документа"
        public class DocumentType
        {
            // Код документа
            public int DocumentId { get; set; }
            
            // Название документа
            public string DocumentName { get; set; }
        }

        // Класс "Отделы"
        public class Department
        {
            // Код отдела
            public int DepartmentId { get; set; }

            // Название отдела
            public string DepartmentName { get; set; }
        }

        // Класс "Трудовой договор"
        public class Contract
        {
            // Код трудового договора
            public int ContractId { get; set; }

            // Номер трудового договора
            public string ContractNumber { get; set; }
        }

        // Класс для отображения сотрудников в таблице
        public class EmployeeRow
        {
            // Код сотрудника
            public int EmployeeId { get; set; }

            // Дата рождения
            public DateTime BirthDate { get; set; }

            // Дата заключения трудового договора
            public DateTime ContractDate { get; set; }

            // ФИО сотрудника
            public string EmployeeName { get; set; }

            // Код отдела
            public int DepartmentId { get; set; }

            // Должность
            public string Position { get; set; }

            // Отдел
            public string Department { get; set; }
        }

        // Класс для получения дат отпуска
        public class VacationDates
        {
            // Код отпуска
            public int VacationId { get; set; }

            // Дата начала
            public DateTime StartDate { get; set; }

            // Дата окончания
            public DateTime EndDate { get; set; }

            // Тип отпуска
            // 0 - Обычный
            // 1 - Больничный
            // 2 - Декретный
            public int VacationType { get; set; }

            // Код сотрудника
            public int EmployeeId { get; set; }
        }

        // Класс для отображения кадровых документов
        public class HrDocument
        {
            // Код документа
            public int Id { get; set; }

            // Наименование документа
            public string Name { get; set; }

            // Имя файла
            public string Filename { get; set; }
        }

        // Класс для хранения данных статистики
        public class Statistics
        {
            // Общее количество сотрудников
            public int Overall { get; set; }

            // Количество работающих сотрудников
            public int Working { get; set; }

            // Количество сотрудников в отпуске
            public int Vacation { get; set; }

            // Количество сотрудников на больничном
            public int Sick { get; set; }

            // Количество сотрудников в декретном отпуске
            public int Maternity { get; set; }

            // Количество уволенных сотрудников
            public int Dismissed { get; set; }
        }
    }
}
