using HR_Automation_System.Classes;
using System.Windows.Controls;

namespace HR_Automation_System.Pages
{
    /// <summary>
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        public StatisticsPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        // Метод загрузки статистики
        private void LoadStatistics()
        {
            var statistics = GlobalStaticParameters.Database.GetStatistics();
            OverallNumber.Content = statistics.Overall;
            WorkingNumber.Content = statistics.Working;
            VacationNumber.Content = statistics.Vacation;
            SickNumber.Content = statistics.Sick;
            MaternityNumber.Content = statistics.Maternity;
            DismissNumber.Content = statistics.Dismissed;
        }
    }
}
