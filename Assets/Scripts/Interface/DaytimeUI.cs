using System;

using UnityEngine;
using UnityEngine.UI;

public class DaytimeUI : MonoBehaviour
{
    [SerializeField] private Text currentTime;

    private void Start()
    {
        DaytimeController.OnTimeChangedCallback += UpdateTimeDisplay;
    }

    private void UpdateTimeDisplay(TimeSpan time)
    {
        if (currentTime != null)
        {
            string timeHours = time.Hours < 10 ? $"0{time.Hours}" : $"{time.Hours}";
            string timeMinutes = time.Minutes < 10 ? $"0{time.Minutes}" : $"{time.Minutes}";

            currentTime.text = $"Day: {time.Days + 1} | {timeHours}:{timeMinutes}";
        }
    }
}
