using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class GoalsPage : Page
    {
        private readonly Database db = new Database();
        private readonly DateTime selectedDate = DateTime.Today;
        private readonly List<GoalItem> goals = new List<GoalItem>();

        public GoalsPage()
        {
            InitializeComponent();

            txtDate.Text = selectedDate.ToString("dd.MM.yyyy");
            LoadData();
        }

        private void LoadData()
        {
            goals.Clear();

            string rawGoals = db.GetGoalsByDate(selectedDate);
            goals.AddRange(DeserializeGoals(rawGoals));

            RenderGoals();
            txtSaveStatus.Text = "";
        }

        private void BtnAddGoal_Click(object sender, RoutedEventArgs e)
        {
            string newGoal = txtNewGoal.Text.Trim();

            if (string.IsNullOrWhiteSpace(newGoal))
            {
                txtSaveStatus.Text = "Önce bir hedef yaz.";
                return;
            }

            goals.Add(new GoalItem
            {
                Text = newGoal,
                IsCompleted = false
            });

            txtNewGoal.Text = "";
            txtSaveStatus.Text = "";
            RenderGoals();
        }

        private void RenderGoals()
        {
            goalsPanel.Children.Clear();

            if (goals.Count == 0)
            {
                goalsPanel.Children.Add(new TextBlock
                {
                    Text = "Henüz hedef eklenmedi.",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(4)
                });
                return;
            }

            for (int i = 0; i < goals.Count; i++)
            {
                var goal = goals[i];
                int index = i;

                var rowGrid = new Grid
                {
                    Margin = new Thickness(0, 0, 0, 8)
                };

                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var checkBox = new CheckBox
                {
                    Content = goal.Text,
                    IsChecked = goal.IsCompleted,
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };

                checkBox.Checked += (s, e) =>
                {
                    goals[index].IsCompleted = true;
                };

                checkBox.Unchecked += (s, e) =>
                {
                    goals[index].IsCompleted = false;
                };

                Grid.SetColumn(checkBox, 0);
                rowGrid.Children.Add(checkBox);

                var deleteButton = new Button
                {
                    Content = "Sil",
                    Width = 48,
                    Height = 28,
                    FontSize = 12,
                    Background = System.Windows.Media.Brushes.White,
                    BorderBrush = System.Windows.Media.Brushes.LightPink,
                    Foreground = System.Windows.Media.Brushes.DarkMagenta
                };

                deleteButton.Click += (s, e) =>
                {
                    goals.RemoveAt(index);
                    RenderGoals();
                };

                Grid.SetColumn(deleteButton, 2);
                rowGrid.Children.Add(deleteButton);

                goalsPanel.Children.Add(rowGrid);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            db.SaveOrUpdateGoals(selectedDate, SerializeGoals(goals));
            txtSaveStatus.Text = "Goals kaydedildi / güncellendi.";
        }

        private string SerializeGoals(List<GoalItem> items)
        {
            if (items.Count == 0)
                return "";

            return string.Join("\n", items.Select(x =>
                $"{(x.IsCompleted ? 1 : 0)}|{x.Text.Replace("\n", " ").Replace("|", "/")}"));
        }

        private List<GoalItem> DeserializeGoals(string raw)
        {
            var list = new List<GoalItem>();

            if (string.IsNullOrWhiteSpace(raw))
                return list;

            var lines = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('|', 2);

                if (parts.Length == 2)
                {
                    list.Add(new GoalItem
                    {
                        IsCompleted = parts[0] == "1",
                        Text = parts[1]
                    });
                }
            }

            return list;
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

        private class GoalItem
        {
            public string Text { get; set; } = "";
            public bool IsCompleted { get; set; }
        }
    }
}