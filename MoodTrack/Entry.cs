using System;

namespace MoodTrack
{
    public class Entry
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Category { get; set; } = "";

        public int Score { get; set; }

        public string Comment { get; set; } = "";
    }
}