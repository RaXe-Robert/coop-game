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
            currentTime.text = time.ToString();
    }
}
