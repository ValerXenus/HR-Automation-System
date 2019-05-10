using HR_Automation_System.Classes;
using System.Windows;

namespace HR_Automation_System.AdditionalWindows
{
    /// <summary>
    /// Interaction logic for PromotionHistory.xaml
    /// </summary>
    public partial class PromotionHistory : Window
    {
        public PromotionHistory(int employeeId)
        {
            InitializeComponent();
            HistoryList.ItemsSource = GlobalStaticParameters.Database.GetEmployeeHistory(employeeId);
        }
    }
}
