using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Freehill.DailyScheduleApp
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class TaskView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _taskName;

        private RectTransform _rectTransform;
        private Image _taskBackground;
        private Task _task;
        private float _initialYOffset = 0.0f;

        /// <summary> The vertical screen space one minute of task time covers. </summary>
        public const float UNITS_PER_MINUTE = 1.5f;

        public float LocalHeight => _rectTransform.rect.height;
        public float LocalPosition => _rectTransform.localPosition.y;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _taskBackground = GetComponent<Image>();
        }

        public void Initialize(Task task, float initialYOffset) 
        {
            _task = task;
            _taskName.text = _task.Name;
            _taskBackground.color = _task.Color;
            _initialYOffset = initialYOffset;

            TimeSpan duration = _task.EndTime - _task.StartTime;
            Vector2 taskSize = _rectTransform.sizeDelta;
            taskSize.y = (float)duration.TotalMinutes * UNITS_PER_MINUTE;
            _rectTransform.sizeDelta = taskSize;
        }

        public void AlignRectToNow(float nowMarkerYOffset)
        {
            float elapsedTaskMinutes = (float)(DateTime.Now - _task.StartTime).TotalMinutes;
            float elapsedTaskYOffset = elapsedTaskMinutes * UNITS_PER_MINUTE;
            Vector2 taskPosition = _rectTransform.anchoredPosition;
            taskPosition.y = _initialYOffset + nowMarkerYOffset - elapsedTaskYOffset;
            _rectTransform.anchoredPosition = taskPosition;
        }
    }
}
