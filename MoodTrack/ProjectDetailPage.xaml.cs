using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class ProjectDetailPage : Page
    {
        private readonly int _projectId;
        private readonly Database _db = new Database();

        public ProjectDetailPage(int projectId, string projectName)
        {
            InitializeComponent();
            _projectId = projectId;
            txtTitle.Text = projectName;
            LoadTasks();
        }

        private void LoadTasks()
        {
            lstActive.Items.Clear();
            lstCompleted.Items.Clear();

            var tasks = _db.GetProjectTasks(_projectId);

            foreach (var task in tasks)
            {
                var checkBox = new CheckBox
                {
                    Content = task.Title,
                    IsChecked = task.IsCompleted,
                    Tag = task.Id,
                    Margin = new Thickness(4),
                    FontSize = 14
                };

                checkBox.Checked += TaskChanged;
                checkBox.Unchecked += TaskChanged;

                if (task.IsCompleted)
                    lstCompleted.Items.Add(checkBox);
                else
                    lstActive.Items.Add(checkBox);
            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string taskTitle = txtTask.Text.Trim();

            if (string.IsNullOrWhiteSpace(taskTitle))
            {
                MessageBox.Show("Lütfen hedef gir.");
                return;
            }

            _db.AddProjectTask(_projectId, taskTitle);
            txtTask.Clear();
            LoadTasks();
        }

        private void TaskChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is int taskId)
            {
                _db.ToggleProjectTask(taskId, cb.IsChecked == true);
                LoadTasks();
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
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