using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class ReportsPage : Page
    {
        private readonly Database db = new Database();
        private readonly AnalysisService analysisService;

        public ReportsPage()
        {
            InitializeComponent();

            db.Initialize();
            analysisService = new AnalysisService(db);

            LoadReports();
        }

        private void LoadReports()
        {
            txtWeeklyReport.Text = analysisService.BuildWeeklyReport();
            txtMonthlyReport.Text = analysisService.BuildMonthlyReport();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                return;
            }

            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ReturnToHome();
            }
        }
    }
}