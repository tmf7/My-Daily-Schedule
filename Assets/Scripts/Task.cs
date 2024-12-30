using System;
using UnityEngine;

namespace Freehill.DailyScheduleApp
{
    public class Task
    {
        public string Category { get; private set; } = "INVALID";
        public DateTime StartTime { get; private set; } = DateTime.MinValue;
        public DateTime EndTime { get; private set; } = DateTime.MinValue;
        public Color Color { get; private set; } = Color.magenta;
        public string Notes { get; private set; }

        private const int HEX_BASE = 16;
        private const float MAX_RANGE = 255;

        public float DurationMinutes => (float)(EndTime - StartTime).TotalMinutes;

        public Task(TaskCategory taskCategory, TaskJSON taskJson)
        {
            Category = taskCategory.Name;
            Notes = taskCategory.Notes;

            try
            {
                StartTime = DateTime.Parse(taskJson.StartTime);
                EndTime = DateTime.Parse(taskJson.EndTime);

                // 0xFF00FF would be magenta as (1.0f, 0.0f, 1.0f)
                float red = Convert.ToInt32(taskCategory.ColorHex.Substring(0, 2), HEX_BASE) / MAX_RANGE;
                float green = Convert.ToInt32(taskCategory.ColorHex.Substring(2, 2), HEX_BASE) / MAX_RANGE;
                float blue = Convert.ToInt32(taskCategory.ColorHex.Substring(4, 2), HEX_BASE) / MAX_RANGE;
                Color = new Color(red, green, blue, 1.0f);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public Task(TaskCategory taskCategory, DateTime startTime, DateTime endTime)
        {
            Category = taskCategory.Name;
            Notes = taskCategory.Notes;
            StartTime = startTime;
            EndTime = endTime;

            try
            {
                // 0xFF00FF would be magenta as (1.0f, 0.0f, 1.0f)
                float red = Convert.ToInt32(taskCategory.ColorHex.Substring(0, 2), HEX_BASE) / MAX_RANGE;
                float green = Convert.ToInt32(taskCategory.ColorHex.Substring(2, 2), HEX_BASE) / MAX_RANGE;
                float blue = Convert.ToInt32(taskCategory.ColorHex.Substring(4, 2), HEX_BASE) / MAX_RANGE;
                Color = new Color(red, green, blue, 1.0f);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public bool HasValidTimes => EndTime - StartTime > TimeSpan.Zero;

        /// <summary> Returns true if this Task's duration includes the given DateTime value. Returns false otherwise. </summary>
        public bool Intersects(DateTime dateTime)
        {
            return StartTime <= dateTime && EndTime >= dateTime;
        }
    }
}
