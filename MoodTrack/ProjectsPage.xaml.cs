using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class ProjectsPage : Page
    {
        private readonly Database _db = new Database();

        public ProjectsPage()
        {
            InitializeComponent();
            LoadProjects();
        }

        private void LoadProjects()
        {
            lstProjects.ItemsSource = null;
            lstProjects.ItemsSource = _db.GetProjects();
        }

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            string name = txtProjectName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Lütfen proje adı gir.");
                return;
            }

            _db.AddProject(name);
            txtProjectName.Clear();
            LoadProjects();
        }

        private void Project_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int projectId = (int)btn.Tag;

                var project = _db.GetProjects()
                                 .FirstOrDefault(p => p.Id == projectId);

                if (project != null)
                {
                    NavigationService?.Navigate(new ProjectDetailPage(project.Id, project.Name));
                }
            }
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