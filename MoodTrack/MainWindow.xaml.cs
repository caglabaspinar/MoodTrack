using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MoodTrack
{
    public partial class MainWindow : Window
    {
        private readonly Database db = new Database();

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                db.Initialize();

                var activeUser = db.GetActiveUser();

                if (activeUser == null)
                {
                    var setupWindow = new SetupWindow();
                    bool? result = setupWindow.ShowDialog();

                    activeUser = db.GetActiveUser();

                    if (activeUser == null || result != true)
                    {
                        MessageBox.Show("Kullanıcı oluşturulamadı.");
                        Application.Current.Shutdown();
                        return;
                    }
                }

                txtWelcome.Text = $"Hoş geldin, {activeUser.FullName}";
                MainFrame.Navigated += MainFrame_Navigated;
            }
            catch (Exception ex)
            {
                MessageBox.Show("MainWindow açılırken hata oluştu:\n\n" + ex);
                Application.Current.Shutdown();
            }
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
            txtSubtitle.Text = "Bugün hangi sayfaya gitmek istersin?";

            txtPin.Clear();
            txtLoginStatus.Text = "";

            ShowHomeMenu();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;
            MainFrame.Visibility = Visibility.Collapsed;
            HomeMenuPanel.Visibility = Visibility.Visible;

            HomePanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;

            txtPin.Clear();
            txtLoginStatus.Text = "Çıkış yapıldı.";
        }

        public void ReturnToHome()
        {
            ShowHomeMenu();
        }

        private void BtnDailyEntry_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new DailyEntryPage(), "Daily Entry");
        }

        private void BtnGoals_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new GoalsPage(), "Goals");
        }

        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new CalendarPage(), "Calendar");
        }

        private void OpenProjects(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new ProjectsPage(), "Projects");
        }

        private void BtnBoss_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new BossPage(), "Boss");
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new ReportsPage(), "Reports");
        }

        private void NavigateToPage(Page page, string title)
        {
            HomeMenuPanel.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            txtSubtitle.Text = title;

            MainFrame.Navigate(page);
        }

        private void ShowHomeMenu()
        {
            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }

            MainFrame.Content = null;
            MainFrame.Visibility = Visibility.Collapsed;
            HomeMenuPanel.Visibility = Visibility.Visible;
            txtSubtitle.Text = "Bugün hangi sayfaya gitmek istersin?";
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content == null)
            {
                ShowHomeMenu();
            }
        }
    }
}