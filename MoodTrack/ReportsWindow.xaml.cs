using System.Windows;

namespace MoodTrack
{
    public partial class ReportsWindow : Window
    {
        private readonly Database db = new Database();
        private readonly AnalysisService analysisService;

        public ReportsWindow()
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}