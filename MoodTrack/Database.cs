using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace MoodTrack
{
    public class Database
    {
        private readonly string dbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "moodtrack.db");

        private string ConnectionString => $"Data Source={dbPath}";

        public void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT,
                    Email TEXT,
                    Pin TEXT,
                    IsActive INTEGER
                );

                CREATE TABLE IF NOT EXISTS Entries
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    Comment TEXT
                );

                CREATE TABLE IF NOT EXISTS BossEntries
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT UNIQUE,
                    Goals TEXT,
                    Notes TEXT,
                    DailyReview TEXT
                );

                CREATE TABLE IF NOT EXISTS Goals
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT UNIQUE,
                    GoalsText TEXT
                );
            ";
            cmd.ExecuteNonQuery();
        }

        public void CreateUser(User user)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Users (FullName, Email, Pin, IsActive)
                VALUES ($name, $email, $pin, 1);
            ";

            cmd.Parameters.AddWithValue("$name", user.FullName);
            cmd.Parameters.AddWithValue("$email", user.Email);
            cmd.Parameters.AddWithValue("$pin", user.Pin);

            cmd.ExecuteNonQuery();
        }

        public User? GetActiveUser()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, FullName, Email, Pin, IsActive
                FROM Users
                WHERE IsActive = 1
                LIMIT 1
            ";

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    Email = reader.GetString(2),
                    Pin = reader.GetString(3),
                    IsActive = reader.GetInt32(4) == 1
                };
            }

            return null;
        }

        public bool CheckPin(string pin)
        {
            var user = GetActiveUser();

            if (user == null)
                return false;

            return user.Pin == pin;
        }

        public void SaveOrUpdateEntry(Entry entry)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"
                SELECT Id
                FROM Entries
                WHERE Date = $date AND Category = $cat
                LIMIT 1;
            ";

            checkCmd.Parameters.AddWithValue("$date", entry.Date.ToString("yyyy-MM-dd"));
            checkCmd.Parameters.AddWithValue("$cat", entry.Category);

            object? existingId = checkCmd.ExecuteScalar();

            if (existingId != null)
            {
                var updateCmd = connection.CreateCommand();
                updateCmd.CommandText = @"
                    UPDATE Entries
                    SET Score = $score, Comment = $comment
                    WHERE Id = $id;
                ";

                updateCmd.Parameters.AddWithValue("$score", entry.Score);
                updateCmd.Parameters.AddWithValue("$comment", entry.Comment ?? "");
                updateCmd.Parameters.AddWithValue("$id", Convert.ToInt32(existingId));

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO Entries (Date, Category, Score, Comment)
                    VALUES ($date, $cat, $score, $comment);
                ";

                insertCmd.Parameters.AddWithValue("$date", entry.Date.ToString("yyyy-MM-dd"));
                insertCmd.Parameters.AddWithValue("$cat", entry.Category);
                insertCmd.Parameters.AddWithValue("$score", entry.Score);
                insertCmd.Parameters.AddWithValue("$comment", entry.Comment ?? "");

                insertCmd.ExecuteNonQuery();
            }
        }

        public Entry? GetEntryByDateAndCategory(DateTime date, string category)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Date, Category, Score, Comment
                FROM Entries
                WHERE Date = $date AND Category = $category
                LIMIT 1
            ";

            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$category", category);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Entry
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    Category = reader.GetString(2),
                    Score = reader.GetInt32(3),
                    Comment = reader.IsDBNull(4) ? "" : reader.GetString(4)
                };
            }

            return null;
        }

        public List<Entry> GetEntriesByDate(DateTime date)
        {
            List<Entry> list = new List<Entry>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Date, Category, Score, Comment
                FROM Entries
                WHERE Date = $date
                ORDER BY Category
            ";

            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Entry
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    Category = reader.GetString(2),
                    Score = reader.GetInt32(3),
                    Comment = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }

            return list;
        }

        public List<Entry> GetAllEntries()
        {
            List<Entry> list = new List<Entry>();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Date, Category, Score, Comment
                FROM Entries
                ORDER BY Date DESC, Category
            ";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Entry
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    Category = reader.GetString(2),
                    Score = reader.GetInt32(3),
                    Comment = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }

            return list;
        }

        public class BossEntry
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Goals { get; set; } = "";
            public string Notes { get; set; } = "";
            public string DailyReview { get; set; } = "";
        }

        public void SaveOrUpdateBossEntry(BossEntry entry)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"
                SELECT Id
                FROM BossEntries
                WHERE Date = $date
                LIMIT 1;
            ";

            checkCmd.Parameters.AddWithValue("$date", entry.Date.ToString("yyyy-MM-dd"));

            object? existingId = checkCmd.ExecuteScalar();

            if (existingId != null)
            {
                var updateCmd = connection.CreateCommand();
                updateCmd.CommandText = @"
                    UPDATE BossEntries
                    SET Goals = $goals, Notes = $notes, DailyReview = $review
                    WHERE Id = $id;
                ";

                updateCmd.Parameters.AddWithValue("$goals", entry.Goals ?? "");
                updateCmd.Parameters.AddWithValue("$notes", entry.Notes ?? "");
                updateCmd.Parameters.AddWithValue("$review", entry.DailyReview ?? "");
                updateCmd.Parameters.AddWithValue("$id", Convert.ToInt32(existingId));

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO BossEntries (Date, Goals, Notes, DailyReview)
                    VALUES ($date, $goals, $notes, $review);
                ";

                insertCmd.Parameters.AddWithValue("$date", entry.Date.ToString("yyyy-MM-dd"));
                insertCmd.Parameters.AddWithValue("$goals", entry.Goals ?? "");
                insertCmd.Parameters.AddWithValue("$notes", entry.Notes ?? "");
                insertCmd.Parameters.AddWithValue("$review", entry.DailyReview ?? "");

                insertCmd.ExecuteNonQuery();
            }
        }

        public BossEntry? GetBossEntryByDate(DateTime date)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Date, Goals, Notes, DailyReview
                FROM BossEntries
                WHERE Date = $date
                LIMIT 1
            ";

            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new BossEntry
                {
                    Id = reader.GetInt32(0),
                    Date = DateTime.Parse(reader.GetString(1)),
                    Goals = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Notes = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    DailyReview = reader.IsDBNull(4) ? "" : reader.GetString(4)
                };
            }

            return null;
        }

        public void SaveOrUpdateGoals(DateTime date, string goalsText)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"
                SELECT Id
                FROM Goals
                WHERE Date = $date
                LIMIT 1;
            ";

            checkCmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));

            object? existingId = checkCmd.ExecuteScalar();

            if (existingId != null)
            {
                var updateCmd = connection.CreateCommand();
                updateCmd.CommandText = @"
                    UPDATE Goals
                    SET GoalsText = $goalsText
                    WHERE Id = $id;
                ";

                updateCmd.Parameters.AddWithValue("$goalsText", goalsText ?? "");
                updateCmd.Parameters.AddWithValue("$id", Convert.ToInt32(existingId));

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                    INSERT INTO Goals (Date, GoalsText)
                    VALUES ($date, $goalsText);
                ";

                insertCmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
                insertCmd.Parameters.AddWithValue("$goalsText", goalsText ?? "");

                insertCmd.ExecuteNonQuery();
            }
        }

        public string GetGoalsByDate(DateTime date)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT GoalsText
                FROM Goals
                WHERE Date = $date
                LIMIT 1
            ";

            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));

            object? result = cmd.ExecuteScalar();

            return result?.ToString() ?? "";
        }
    }
}