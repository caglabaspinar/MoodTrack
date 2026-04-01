using System;
using System.Collections.Generic;
using System.Linq;

namespace MoodTrack
{
    public class AnalysisService
    {
        private readonly Database db;

        public AnalysisService(Database database)
        {
            db = database;
        }

        public string BuildWeeklyReport()
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate.AddDays(-6);

            return BuildReport(startDate, endDate, "Haftalık");
        }

        public string BuildMonthlyReport()
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate.AddDays(-29);

            return BuildReport(startDate, endDate, "Aylık");
        }

        private string BuildReport(DateTime startDate, DateTime endDate, string title)
        {
            List<Entry> allEntries = db.GetAllEntries();

            var filtered = allEntries
                .Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)
                .ToList();

            string[] categories = { "hayat", "iliski" };

            int expectedCount = 0;
            int actualCount = 0;
            int autoFilledCount = 0;

            List<int> hayatScores = new List<int>();
            List<int> iliskiScores = new List<int>();

            foreach (var day in EachDay(startDate, endDate))
            {
                foreach (var category in categories)
                {
                    expectedCount++;

                    Entry? entry = filtered.FirstOrDefault(x =>
                        x.Date.Date == day.Date &&
                        x.Category == category);

                    if (entry != null)
                    {
                        actualCount++;

                        if (category == "hayat")
                            hayatScores.Add(entry.Score);
                        else
                            iliskiScores.Add(entry.Score);
                    }
                    else
                    {
                        if (DateTime.Today.Date >= day.Date.AddDays(2))
                        {
                            autoFilledCount++;

                            if (category == "hayat")
                                hayatScores.Add(3);
                            else
                                iliskiScores.Add(3);
                        }
                    }
                }
            }

            double usageRate = expectedCount == 0 ? 0 : (actualCount * 100.0 / expectedCount);

            double hayatAvg = hayatScores.Count == 0 ? 0 : hayatScores.Average();
            double iliskiAvg = iliskiScores.Count == 0 ? 0 : iliskiScores.Average();

            string hayatComment = GetSoftComment(hayatAvg, "Hayat");
            string iliskiComment = GetSoftComment(iliskiAvg, "İlişki");

            return
$@"{title} Rapor

Tarih Aralığı: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}

Kullanım Oranı: %{usageRate:F1}
Beklenen Kayıt: {expectedCount}
Senin Girdiğin Kayıt: {actualCount}
Sistemin 3 Kabul Ettiği Kayıt: {autoFilledCount}

Hayat Ortalaması: {hayatAvg:F2}
{hayatComment}

İlişki Ortalaması: {iliskiAvg:F2}
{iliskiComment}";
        }

        private string GetSoftComment(double average, string label)
        {
            if (average == 0)
                return $"{label} alanı için henüz yeterli veri yok.";

            if (average < 1.5)
                return $"{label} alanında bu dönem oldukça zorlayıcı geçmiş gibi görünüyor.";
            if (average < 2.5)
                return $"{label} alanında bu dönem biraz düşük bir seyir var gibi duruyor.";
            if (average < 3.5)
                return $"{label} alanında bu dönem daha çok orta seviyede geçmiş görünüyor.";
            if (average < 4.5)
                return $"{label} alanında bu dönem genel olarak iyi görünüyor.";

            return $"{label} alanında bu dönem oldukça iyi görünüyor.";
        }

        private IEnumerable<DateTime> EachDay(DateTime startDate, DateTime endDate)
        {
            for (DateTime day = startDate.Date; day <= endDate.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
    }
}