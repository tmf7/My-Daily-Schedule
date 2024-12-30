namespace Freehill.DailyScheduleApp
{
    public class TaskCategory
    { 
        public string Name      { get; set; } // "Game Dev Work"
        public string ColorHex  { get; set; } // "FF00FF"
        public string Notes     { get; set; } // "Jira, Confluence, Git, Meetups (tue|wed|thu)"
    }

    public class TaskJSON
    {
        public string Category  { get; set; } // "Game Dev Work"
        public string StartTime { get; set; } // "Wednesday 9:00 AM"
        public string EndTime   { get; set; } // "Wednesday 5:00 PM"
    }

    public class DailySchedule
    {
        public TaskCategory[] TaskCategories { get; set; }
        public TaskJSON[] DailyTasks         { get; set; }
    }
}