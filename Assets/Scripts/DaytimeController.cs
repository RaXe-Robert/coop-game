using UnityEngine;
using System.Collections;
using System;

public class DaytimeController : MonoBehaviour
{
    public const int MINUTESPERDAY = 24 * 60; 
    
    [SerializeField] private GameObject sunLight = null;
    [Range(0, 45)]
    [SerializeField] private float rotationCurveOffset = 0f;

    [Tooltip("Ticks per second, a tick is 1 MINUTE in gametime.")]
    [SerializeField] int tickRate = 1;

    [Header("Time Settings")]
    [Tooltip("Hours/Minutes/Seconds")]
    [SerializeField] private Vector3 initialStartTime = new Vector3(12, 0, 0);

    [Range(1, 10)] [SerializeField] private int dayStartHour = 7;
    [Range(16, 24)] [SerializeField] private int nightStartHour = 23;

    private TimeSpan currentTime;
    public TimeSpan CurrentTime
    {
        get { return currentTime; }
        private set
        {
            currentTime = value;
            targetAngle = CalculateNextSunAngle();
        }
    }
    /// <summary>
    /// Returns the duration of the dayTime in hours.
    /// </summary>
    public int DayDuration => nightStartHour - dayStartHour;
    /// <summary>
    /// Returns the duration of the nightTime in hours.
    /// </summary>
    public int NightDuration => 24 - DayDuration;
    
    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private float currentVelocity = 0f;
    
    private void Start()
    {
        sunLight.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, rotationCurveOffset));
        
        CurrentTime = VectorToTimeSpan(initialStartTime);
    }

    private void OnEnable()
    {
        StartCoroutine(TimeProgression());
    }

    private void OnDisable()
    {
        StopCoroutine(TimeProgression());
    }

    private void Update()
    {
        ApplySunPosition();
    }

    private IEnumerator TimeProgression()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

        while (true)
        {
            CurrentTime = CurrentTime.Add(TimeSpan.FromMinutes(tickRate));
            yield return waitForSeconds;
        }
    }

    /// <summary>
    /// Calculates the sun position based on the <see cref="CurrentTime"/>, <seealso cref="dayStartHour"/> and <seealso cref="nightStartHour"/>.
    /// </summary>
    private float CalculateNextSunAngle()
    {
        float dayMinutes = CurrentTime.Hours > 0 ? CurrentTime.Hours * 60 + CurrentTime.Minutes : CurrentTime.Minutes;
        dayMinutes += tickRate; // Add the tickrate to the current amount of minutes to get the total of minutes on the next tick

        if (dayMinutes > MINUTESPERDAY)
            dayMinutes = Mathf.Abs(MINUTESPERDAY - dayMinutes);

        float currentPercentage = dayMinutes / MINUTESPERDAY * 100f;
        if (dayStartHour * 60 <= dayMinutes && nightStartHour * 60 > dayMinutes)
        {
            int startMinute = dayStartHour * 60;
            float dayPercentage = (dayMinutes - startMinute) / (DayDuration * 60) * 100;

            return 180f / 100f * dayPercentage;
        }
        else
        {
            int startMinute = nightStartHour * 60;
            float nightPercentage = (dayMinutes - startMinute) / (NightDuration * 60) * 100;

            // If we reached midnight we should calculate the percentage differently
            if (nightPercentage < 0)
                nightPercentage = (dayMinutes + (MINUTESPERDAY - startMinute)) / (NightDuration * 60) * 100;
            
            return 180f / 100f * nightPercentage + 180;
        }
    }

    private void ApplySunPosition()
    {
        float angle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentVelocity, 0.3f);

        sunLight.transform.rotation = Quaternion.identity;
        sunLight.transform.Rotate(Vector3.forward, rotationCurveOffset);
        sunLight.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.right);

        currentAngle = angle;
    }

    private TimeSpan VectorToTimeSpan(Vector3 vector)
    {
        return new TimeSpan((int)vector.x, (int)vector.y, (int)vector.z);
    }

    private Vector3 TimeSpawnToVector(TimeSpan timeSpan)
    {
        return new Vector3(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
