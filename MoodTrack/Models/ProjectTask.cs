namespace MoodTrack
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = "";
        public bool IsCompleted { get; set; }
    }
}