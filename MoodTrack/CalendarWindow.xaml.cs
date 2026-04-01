using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MoodTrack
{
    public partial class CalendarWindow : Window
    {
        private readonly Database db = new Database();

        public CalendarWindow()
        {
            InitializeComponent();
            calendarEntries.SelectedDate = DateTime.Today;
            LoadEntriesForDate(DateTime.Today);
        }

        private void CalendarEntries_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendarEntries.SelectedDate != null)
            {
                LoadEntriesForDate(calendarEntries.SelectedDate.Value);
            }
        }

        private void LoadEntriesForDate(DateTime date)
        {
            txtSelectedDate.Text = $"{date:dd.MM.yyyy}";

            List<Entry> entries = db.GetEntriesByDate(date);

            lstEntries.Items.Clear();

            if (entries.Count == 0)
            {
                lstEntries.Items.Add("Bu gün için kayıt bulunamadı.");
                return;
            }

            foreach (var entry in entries)
            {
                string kategori = entry.Category == "hayat" ? "Hayat" : "İlişki";

                string block =
                    $"Kategori: {kategori}\n" +
                    $"Puan: {entry.Score}\n" +
                    $"Yorum: {entry.Comment}\n" +
                    $"------------------------------";

                TextBlock textBlock = new TextBlock
                {
                    Text = block,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10),
                    FontSize = 15,
                    MaxWidth = 500
                };

                lstEntries.Items.Add(textBlock);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}