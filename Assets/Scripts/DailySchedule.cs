namespace Freehill.DailyScheduleApp
{
    public class TaskJSON
    {
        public string TaskName  { get; set; } // "Work" or "Sleep"
        public string StartTime { get; set; } // "WED 9:00 AM" or "WED 11:00 PM"
        public string EndTime   { get; set; } // "WED 5:00 PM" or "THU 8:00 AM"
        public string ColorHEX  { get; set; } // "FF00FF"
    }

    public class DailySchedule
    { 
        public TaskJSON[] taskJSONs { get; set; }
    }
}