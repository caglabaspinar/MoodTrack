using System;
using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class BossPage : Page
    {
        private readonly Database db = new Database();
        private readonly DateTime selectedDate = DateTime.Today;

        public BossPage()
        {
            InitializeComponent();

            txtDate.Text = selectedDate.ToString("dd.MM.yyyy");
            LoadData();
        }

        private void LoadData()
        {
            var data = db.GetBossEntryByDate(selectedDate);

            if (data != null)
            {
                txtGoals.Text = data.Goals;
                txtNotes.Text = data.Notes;
                txtReview.Text = data.DailyReview;
            }
            else
            {
                txtGoals.Text = "";
                txtNotes.Text = "";
                txtReview.Text = "";
            }

            txtSaveStatus.Text = "";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var entry = new Database.BossEntry
            {
                Date = selectedDate,
                Goals = txtGoals.Text,
                Notes = txtNotes.Text,
                DailyReview = txtReview.Text
            };

            db.SaveOrUpdateBossEntry(entry);
            txtSaveStatus.Text = "Kaydedildi.";
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