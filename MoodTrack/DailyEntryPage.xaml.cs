using System;
using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class DailyEntryPage : Page
    {
        private readonly Database db = new Database();

        public DailyEntryPage()
        {
            InitializeComponent();

            dpDate.SelectedDate = DateTime.Today;
            cbLifeScore.SelectedIndex = 2;
            cbRelationshipScore.SelectedIndex = 2;

            LoadExistingEntriesForSelectedDate();
        }

        private void BtnSaveDailyEntry_Click(object sender, RoutedEventArgs e)
        {
            if (dpDate.SelectedDate == null)
            {
                txtStatus.Text = "Lütfen tarih seç.";
                return;
            }

            if (cbLifeScore.SelectedItem == null || cbRelationshipScore.SelectedItem == null)
            {
                txtStatus.Text = "Lütfen puanları seç.";
                return;
            }

            string lifeScoreText = ((ComboBoxItem)cbLifeScore.SelectedItem).Content.ToString() ?? "3";
            string relationshipScoreText = ((ComboBoxItem)cbRelationshipScore.SelectedItem).Content.ToString() ?? "3";

            int lifeScore = int.Parse(lifeScoreText.Substring(0, 1));
            int relationshipScore = int.Parse(relationshipScoreText.Substring(0, 1));

            Entry lifeEntry = new Entry
            {
                Date = dpDate.SelectedDate.Value,
                Category = "hayat",
                Score = lifeScore,
                Comment = txtLifeComment.Text.Trim()
            };

            Entry relationshipEntry = new Entry
            {
                Date = dpDate.SelectedDate.Value,
                Category = "iliski",
                Score = relationshipScore,
                Comment = txtRelationshipComment.Text.Trim()
            };

            db.SaveOrUpdateEntry(lifeEntry);
            db.SaveOrUpdateEntry(relationshipEntry);

            txtStatus.Text = "Günlük giriş kaydedildi / güncellendi.";
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
        private void dpDate_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            LoadExistingEntriesForSelectedDate();
        }

        private void LoadExistingEntriesForSelectedDate()
        {
            if (dpDate.SelectedDate == null)
                return;

            var selectedDate = dpDate.SelectedDate.Value;

            Entry? lifeEntry = db.GetEntryByDateAndCategory(selectedDate, "hayat");
            Entry? relationshipEntry = db.GetEntryByDateAndCategory(selectedDate, "iliski");

            if (lifeEntry != null)
            {
                cbLifeScore.SelectedIndex = lifeEntry.Score - 1;
                txtLifeComment.Text = lifeEntry.Comment;
            }
            else
            {
                cbLifeScore.SelectedIndex = 2;
                txtLifeComment.Clear();
            }

            if (relationshipEntry != null)
            {
                cbRelationshipScore.SelectedIndex = relationshipEntry.Score - 1;
                txtRelationshipComment.Text = relationshipEntry.Comment;
            }
            else
            {
                cbRelationshipScore.SelectedIndex = 2;
                txtRelationshipComment.Clear();
            }

            txtStatus.Text = "";
        }
    }
}