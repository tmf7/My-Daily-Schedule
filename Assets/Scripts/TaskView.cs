using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Freehill.DailyScheduleApp
{
    [RequireComponent(typeof(Image), typeof(LayoutElement))]
    public class TaskView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _taskName;
        [SerializeField] private TMP_Text _taskNotes;

        private Image _taskBackground;
        private LayoutElement _taskLayout;
        private Task _task;

        public Task Task => _task;
        public DateTime StartTime => _task.StartTime;

        private void Awake()
        {
            _taskBackground = GetComponent<Image>();
            _taskLayout = GetComponent<LayoutElement>();
        }

        public void Initialize(Task task, float unitsPerMinute) 
        {
            _task = task;
            _taskName.text = _task.Category;
            _taskNotes.text = _task.Notes;
            _taskBackground.color = _task.Color;
            _taskLayout.preferredHeight = _task.DurationMinutes * unitsPerMinute;
        }
    }
}
