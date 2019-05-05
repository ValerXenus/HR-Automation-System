namespace HR_Automation_System.Classes
{
    // Класс для хранения глобальных параметров
    public static class GlobalStaticParameters
    {
        // ID пользователя
        public static int UserId {get; set;}

        // Подключение к базе данных
        public static DatabaseUtility Database { get; set; }
    }
}
