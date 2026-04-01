using System;
using System.Windows;

namespace MoodTrack
{
    public partial class BossWindow : Window
    {
        private readonly Database db = new Database();
        private readonly DateTime selectedDate = DateTime.Today;

        public BossWindow()
        {
            InitializeComponent();

            this.Loaded += BossWindow_Loaded;

            txtDate.Text = selectedDate.ToString("dd.MM.yyyy");
            LoadData();
        }

        private void BossWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.WorkArea.Height - 20;
            this.MaxWidth = SystemParameters.WorkArea.Width - 20;

            if (this.Height > this.MaxHeight)
                this.Height = this.MaxHeight;

            if (this.Width > this.MaxWidth)
                this.Width = this.MaxWidth;

            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
            this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
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
            txtSaveStatus.Text = "Boss kaydı kaydedildi / güncellendi.";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}