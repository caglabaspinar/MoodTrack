using System;
using System.Windows;

namespace MoodTrack
{
    public partial class MainWindow : Window
    {
        private readonly Database db = new Database();
        private AnalysisService? analysisService;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this.Loaded += MainWindow_Loaded;

                db.Initialize();
                analysisService = new AnalysisService(db);

                var activeUser = db.GetActiveUser();

                if (activeUser == null)
                {
                    SetupWindow setupWindow = new SetupWindow();
                    setupWindow.ShowDialog();

                    activeUser = db.GetActiveUser();

                    if (activeUser == null)
                    {
                        MessageBox.Show("Kullanıcı oluşturulamadı.");
                        Application.Current.Shutdown();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("MainWindow açılırken hata oluştu:\n\n" + ex.ToString());
                Application.Current.Shutdown();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string pin = txtPin.Password.Trim();

            if (string.IsNullOrWhiteSpace(pin))
            {
                txtLoginStatus.Text = "PIN gir.";
                return;
            }

            if (pin.Length != 4)
            {
                txtLoginStatus.Text = "PIN 4 haneli olmalı.";
                return;
            }

            bool isValid = db.CheckPin(pin);

            if (!isValid)
            {
                txtLoginStatus.Text = "PIN yanlış.";
                return;
            }

            var user = db.GetActiveUser();

            LoginPanel.Visibility = Visibility.Collapsed;
            HomePanel.Visibility = Visibility.Visible;

            txtWelcome.Text = $"Hoş geldin, {user?.FullName}";
            txtPin.Clear();
            txtLoginStatus.Text = "";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            HomePanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;

            txtPin.Clear();
            txtLoginStatus.Text = "Çıkış yapıldı.";
        }

        private void BtnDailyEntry_Click(object sender, RoutedEventArgs e)
        {
            new DailyEntryWindow().ShowDialog();
        }

        private void BtnGoals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new GoalsWindow().ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            new CalendarWindow().ShowDialog();
        }

        private void BtnBoss_Click(object sender, RoutedEventArgs e)
        {
            new BossWindow().ShowDialog();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            new ReportsWindow().ShowDialog();
        }
    }
}