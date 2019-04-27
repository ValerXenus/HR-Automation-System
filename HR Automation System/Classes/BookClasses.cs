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
    }
}
