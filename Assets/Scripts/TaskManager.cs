using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

namespace Freehill.DailyScheduleApp
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField] private TaskView _taskViewPrefab; 
        [SerializeField] private float _tickIntervalSeconds = 1.0f;
        [SerializeField] private RectTransform _nowMarkerRectTransform;

        private List<TaskView> _activeTaskViews = new List<TaskView>();

        private void Awake()
        {
            DateTime now = DateTime.Now;
            float screenYOffset = 0.0f;
            // TODO: =====SPAWN ALGORITHM======
            // get the first task that overlaps NOW (if any),
            // then spawn it,
            // then check if screen is filled,
            // if not, find the next task that abuts the current task (if any) 
            // then spawn it, and repeat
            // if no task abuts the current task (or next task) then insert a gray-colored "Free Time" task to fills gaps
            // =================================

            // TODO: locate the DailySchedule.json file on disk (NOT in StreamingAssets, or PersistentDataPath, or DataPath, just in external android #Files)

            // https://discussions.unity.com/t/how-can-i-browse-files-on-android-outside-of-the-unity-app-folder/178021
            // Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS);
            // https://learn.microsoft.com/en-us/dotnet/api/android.os.environment.getexternalstoragepublicdirectory?view=net-android-34.0&viewFallbackFrom=xamarin-android-sdk-12
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AndroidJavaClass.html
            // https://developer.android.com/reference/android/os/Environment#getExternalStoragePublicDirectory(java.lang.String)
            // https://developer.android.com/reference/java/io/FileReader#FileReader(java.io.File)
            string fileJson = "bob";
            var dailyScheduleJSON = JsonUtility.FromJson<DailySchedule>(fileJson);


            // TODO: only spawn those that will cover the screen
            // TODO: account for tasks prior to NOW-overlap which may be needed for screen coverage (ie: a series of short-duration tasks and a low now-marker)
            foreach (TaskJSON taskJSON in dailyScheduleJSON.taskJSONs) 
            {
                var task = new Task(taskJSON);
                if (task.Intersects(now)) 
                { 
                    TaskView taskView = Instantiate(_taskViewPrefab, transform);
                    taskView.Initialize(task, screenYOffset);
                    taskView.AlignRectToNow(_nowMarkerRectTransform.anchoredPosition.y);
                    // TODO: check screencoverage, if sufficient, then stop
                    screenYOffset += taskView.LocalHeight; // FIXME(?): base the root-offset on heights, not screen position of rects
                }

            }
            
        }

        //private string GetAndroidExternalStoragePath()
        //{
        //    string path = string.Empty;
        //    try
        //    {
        //        var javaClass = new AndroidJavaClass("android.os.Environment");
        //        path = javaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log(e.Message);
        //    }

        //    return path;
        //}

        private IEnumerator Start()
        {
            while (Application.isPlaying) 
            { 
                yield return new WaitForSecondsRealtime(_tickIntervalSeconds);
                foreach (var taskView in _activeTaskViews) 
                {
                    taskView.AlignRectToNow(_nowMarkerRectTransform.anchoredPosition.y);
                }

                // TODO: destroy task that goes above screen
                // TODO: spawn as many tasks as it takes to cover the screen height
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) 
            { 
                Application.runInBackground = false; // don't waste battery
            }
        }
    }
}
