using System;
using System.Collections.Generic;
using System.Linq;

namespace Freehill.DailyScheduleApp
{
    [Serializable]
    public class TaskCategory
    {
        public string Name;// "Game Dev Work"
        public string ColorHex;// "FF00FF"
        public string Notes; // "Jira, Confluence, Git, Meetups (tue|wed|thu)"
    }

    [Serializable]
    public class TaskJSON
    {
        public string Category;// "Game Dev Work"
        public string StartTime;// "Wednesday 9:00 AM"
        public string EndTime; // "Wednesday 5:00 PM"

        /// <summary> Replaces the day of the week within StartTime and EndTime with DD/MMM/YYYY around the current date </summary>
        public void FixupTimes()
        {
            var startTimeSplit = StartTime.Split(" ");
            var endTimeSplit = EndTime.Split(" ");

            var now = DateTime.Now;
            DayOfWeek currentDay = now.DayOfWeek;
            DayOfWeek startDay = GetDayOfTheWeek(startTimeSplit[0].ToLower());
            DayOfWeek endDay = GetDayOfTheWeek(endTimeSplit[0].ToLower());

            startTimeSplit[0] = now.AddDays(startDay - currentDay).ToShortDateString();
            endTimeSplit[0] = now.AddDays(endDay - currentDay).ToShortDateString();

            StartTime = string.Join(" ", startTimeSplit);
            EndTime = string.Join(" ", endTimeSplit);
        }

        private DayOfWeek GetDayOfTheWeek(string taskDay)
        {
            switch (taskDay) 
            {
                case "sunday":
                    return DayOfWeek.Sunday;
                case "monday":
                    return DayOfWeek.Monday;
                case "tuesday":
                    return DayOfWeek.Tuesday;
                case "wednesday":
                    return DayOfWeek.Wednesday;
                case "thursday":
                    return DayOfWeek.Thursday;
                case "friday":
                    return DayOfWeek.Friday;
                case "saturday":
                    return DayOfWeek.Saturday;
                default:
                    throw new ArgumentException($"Invalid day of week string. Check Task JSON for [{taskDay}]");
            }
        }
    }

    [Serializable]
    public class DailySchedule
    {
        public List<TaskCategory> TaskCategories;
        public List<TaskJSON> DailyTasks;

        /// <summary> Replaces the day of the week within each TaskJSON with DD/MMM/YYYY around the current date </summary>
        public void FixupTaskTimes()
        { 
            foreach (TaskJSON taskJSON in DailyTasks) 
            {
                taskJSON.FixupTimes();
            }
        }
    }
}