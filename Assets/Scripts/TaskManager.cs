using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System.Globalization;
using TMPro;

namespace Freehill.DailyScheduleApp
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField] private TextAsset _dailyScheduleJSON;
        [SerializeField] private TaskView _taskViewPrefab;
        [SerializeField] private RectTransform _tasksContentRectTransform;
        [SerializeField] private RectTransform _nowMarkerRectTransform;
        [SerializeField] private TMP_Text _currentTask;
        [SerializeField] private TMP_Text _currentTaskNotes;
        [SerializeField] private float _updateIntervalSeconds = 1.0f;
        [SerializeField] private float _unitsPerMinute = 1.5f;

        private List<Task> _allTasks = new List<Task>();
        private List<TaskView> _activeTaskViews = new List<TaskView>();
        private Coroutine _taskViewAnimation = null;

        private CultureInfo _culture = new CultureInfo("en-US");

        private const string FREE_TIME_CATEGORY = "Free Time";

        private void Awake()
        {
            // TODO(~): to avoid the need to rebuild when updating the TextAsset, instead use AddressableReference and load from a URL,
            // alternatively read a google sheet directly and parse that
            var dailySchedule = JsonUtility.FromJson<DailySchedule>(_dailyScheduleJSON.text);
            dailySchedule.FixupTaskTimes();
            ClearTasks();
            FillTasks(dailySchedule);
            _taskViewAnimation = StartCoroutine(AnimateTaskScroll());
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                if (_taskViewAnimation != null)
                {
                    StopCoroutine(_taskViewAnimation);
                }

                _taskViewAnimation = StartCoroutine(AnimateTaskScroll());
            }
            else
            {
                Application.runInBackground = false; // don't waste battery
            }
        }

        private void ClearTasks()
        {
            while(_tasksContentRectTransform.childCount > 0) 
            {
                Destroy(_tasksContentRectTransform.GetChild(0));
            }
            _activeTaskViews.Clear();
        }

        /// <summary> Enforces task order by StartTime, and fills gaps in schedule with "Free Time" tasks to avoid view discontinuities </summary>
        private void FillTasks(DailySchedule dailySchedule)
        {
            foreach (TaskJSON taskJSON in dailySchedule.DailyTasks) 
            {
                TaskCategory category = dailySchedule.TaskCategories.FirstOrDefault(category => category.Name == taskJSON.Category);

                if (category != null)
                {
                    var task = new Task(category, taskJSON);
                    if (task.HasValidTimes)
                    {
                        _allTasks.Add(task);
                    }
                }
                else
                {
                    Debug.LogError($"Task Category not found: [{taskJSON.Category}]");
                }
            }

            _allTasks = _allTasks.OrderBy(task => task.StartTime).ToList();

            // fill time gaps, don't corrupt iteration
            // DEBUG: assumes a TaskCategory exists in MyDailySchedule.json with the value of FREE_TIME_CATEGORY (eg: "Free Time")
            TaskCategory freeTimeCategory = dailySchedule.TaskCategories.FirstOrDefault(category => category.Name == FREE_TIME_CATEGORY);
            var freeTimeTasks = new List<Task>();

            for (int i = 0; i < _allTasks.Count - 1; i++) 
            {
                if (_allTasks[i].EndTime != _allTasks[i + 1].StartTime)
                {
                    var freeTask = new Task(freeTimeCategory,
                                            _allTasks[i].EndTime,
                                            _allTasks[i + 1].StartTime);

                    if (freeTask.HasValidTimes) 
                    { 
                        freeTimeTasks.Add(freeTask);
                    }
                }
            }

            _allTasks.AddRange(freeTimeTasks);
            _allTasks = _allTasks.OrderBy(task => task.StartTime).ToList();
        }

        private IEnumerator AnimateTaskScroll()
        {
            while (Application.isPlaying) 
            { 
                yield return new WaitForSecondsRealtime(_updateIntervalSeconds);
                UpdateVisibleTasks();
                AlignToNowMarker();
            }
        }

        private List<Task> GetOnScreenTasks()
        {
            List<Task> onScreenTasks = new List<Task>();

            float totalScreenMinutesCovered = Screen.height / _unitsPerMinute;
            float screenMinutesAboveNowMarker = (_nowMarkerRectTransform.position.y / _unitsPerMinute);
            float screenMinutesBelowNowMarker = totalScreenMinutesCovered - screenMinutesAboveNowMarker;
            DateTime screenTopTime = DateTime.Now - TimeSpan.FromMinutes(screenMinutesAboveNowMarker);
            DateTime screenBottomTime = DateTime.Now + TimeSpan.FromMinutes(screenMinutesBelowNowMarker);

            Task topTask = _allTasks.FirstOrDefault(task => task.Intersects(screenTopTime));
            Task bottomTask = _allTasks.FirstOrDefault(task => task.Intersects(screenBottomTime));

            for (int i = _allTasks.IndexOf(topTask); i <= _allTasks.IndexOf(bottomTask); ++i)
            {
                onScreenTasks.Add(_allTasks[i]);
            }

            return onScreenTasks;
        }

        private void AlignToNowMarker()
        {
            // the first task is anchored at the top of the content rect
            float firstTaskToNowMinutes = (float)(_activeTaskViews[0].StartTime - DateTime.Now).TotalMinutes;
            float contentToNowRectMinutes = (_nowMarkerRectTransform.localPosition.y - _tasksContentRectTransform.localPosition.y) / _unitsPerMinute;
            float minutesOffset = contentToNowRectMinutes - firstTaskToNowMinutes;

            Vector3 contentPosition = _tasksContentRectTransform.localPosition;
            contentPosition.y += (minutesOffset * _unitsPerMinute);
            _tasksContentRectTransform.localPosition = contentPosition;
        }

        private void UpdateVisibleTasks()
        { 
            var onScreenTasks = GetOnScreenTasks();

            // remove off-screen tasks
            for (int i = 0; i < _activeTaskViews.Count; ++i)
            {
                if (!onScreenTasks.Contains(_activeTaskViews[i].Task))
                {
                    Destroy(_activeTaskViews[i].gameObject);
                    _activeTaskViews[i] = null;
                }
            }

            _activeTaskViews.RemoveAll(taskView => taskView == null);

            // create missing on-screen tasks (in sorted order)
            foreach (Task task in onScreenTasks) 
            {
                if (_activeTaskViews.FirstOrDefault(taskView => taskView.Task == task) == null)
                {
                    TaskView newTaskView = Instantiate(_taskViewPrefab, _tasksContentRectTransform);
                    newTaskView.Initialize(task, _unitsPerMinute);
                    _activeTaskViews.Add(newTaskView);
                }
            }

            Task currentTask = onScreenTasks.FirstOrDefault(task => task.Intersects(DateTime.Now));
            _currentTask.text = currentTask.Category;
            _currentTaskNotes.text = currentTask.Notes;
        }
    }
}
