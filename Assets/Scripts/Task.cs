using System;
using UnityEngine;

namespace Freehill.DailyScheduleApp
{
    public class Task
    {
        public string Name { get; private set; } = "INVALID";
        public DateTime StartTime { get; private set; } = DateTime.MinValue;
        public DateTime EndTime { get; private set; } = DateTime.MinValue;
        public Color Color { get; private set; } = Color.magenta;

        private const int HEX_BASE = 16;
        private const float MAX_RANGE = 255;

        public Task(TaskJSON json)
        {
            Name = json.TaskName;

            try
            {
                StartTime = DateTime.Parse(json.StartTime);
                EndTime = DateTime.Parse(json.EndTime);

                // 0xFF00FF would be magenta as (1.0f, 0.0f, 1.0f)
                float red = Convert.ToInt32(json.ColorHEX.Substring(0, 2), HEX_BASE) / MAX_RANGE;
                float green = Convert.ToInt32(json.ColorHEX.Substring(2, 2), HEX_BASE) / MAX_RANGE;
                float blue = Convert.ToInt32(json.ColorHEX.Substring(4, 2), HEX_BASE) / MAX_RANGE;
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
            // TODO: account for "SLEEP" task that crosses midnight between days? 
            // EG: starts "WED 11:00 PM" ends "THU 8:00 AM"
            // TODO: account for new year change during overlap, but also during display
            return false;
        }
    }
}
