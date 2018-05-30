using UnityEngine;
using System.Collections;
using System;

public class DaytimeController : Photon.MonoBehaviour, IPunObservable
{
    public static DaytimeController Instance { get; private set; }

    public const int MINUTESPERDAY = 24 * 60; 
    
    [Range(0, 45)]
    [SerializeField] private float rotationCurveOffset = 0f;

    [Tooltip("Ticks per second, a tick is 1 MINUTE in gametime.")]
    [SerializeField] int tickRate = 1;

    [Header("Time Settings")]
    [Range(1, 10)] [SerializeField] private int dayStartHour = 7;
    [Range(16, 24)] [SerializeField] private int nightStartHour = 23;

    [Header("Lighting")]
    [SerializeField] private Light sunLight = null;
    [SerializeField] private Light moonLight = null;

    [Header("Daytime")]
    [SerializeField] private AnimationCurve daySunIntensity;
    [SerializeField] private AnimationCurve dayMoonIntensity;

    [SerializeField] private AnimationCurve daySunShadowStrength;

    [Header("Nighttime")]
    [SerializeField] private AnimationCurve nightMoonIntensity;

    public delegate void TimeChangedHandler(TimeSpan currentTime);
    public static event TimeChangedHandler OnTimeChangedCallback;

    private TimeSpan currentTime;
    public TimeSpan CurrentTime
    {
        get { return currentTime; }
        private set
        {
            currentTime = value;
            TargetAngle = CalculateNextSunAngle();

            OnTimeChangedCallback?.Invoke(currentTime);
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

    private float currentDayPercentage;
    private float currentNightPercentage;

    private float currentSunAngle = 0f;
    public float CurrentSunAngle
    {
        get { return currentSunAngle; }
        private set { currentSunAngle = AngleMod(value); }
    }

    private float targetAngle = 0f;
    private float TargetAngle
    {
        get { return targetAngle; }
        set { targetAngle = AngleMod(value); }
    }

    public bool IsDaytime { get { return CurrentSunAngle >= 0 && CurrentSunAngle <= 180; } }

    private void Awake()
    {
        Instance = FindObjectOfType<DaytimeController>();

        currentTime = TimeSpan.FromTicks((long)PhotonNetwork.room.CustomProperties["gameTime"]);
    }

    private void Start()
    {
        moonLight.transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }

    private void OnEnable() => StartCoroutine(TimeProgression());
    private void OnDisable() => StopCoroutine(TimeProgression());

    private void FixedUpdate()
    {
        ApplyAngle();
    }

    private IEnumerator TimeProgression()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

        while (true)
        {
            if (PhotonNetwork.isMasterClient)
            {
                CurrentTime = CurrentTime.Add(TimeSpan.FromMinutes(tickRate));
                PhotonNetwork.room.CustomProperties["gameTime"] = CurrentTime.Ticks;
            }
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
        
        if (dayStartHour * 60 <= dayMinutes && nightStartHour * 60 > dayMinutes)
        {
            currentDayPercentage = (dayMinutes - dayStartHour * 60) / (DayDuration * 60) * 100;

            return 180f / 100f * currentDayPercentage;
        }
        else
        {
            currentNightPercentage = (dayMinutes - nightStartHour * 60) / (NightDuration * 60) * 100;

            // If we reached midnight we should calculate the percentage differently
            if (currentNightPercentage < 0)
                currentNightPercentage = (dayMinutes + (MINUTESPERDAY - nightStartHour * 60)) / (NightDuration * 60) * 100;
            
            return 180f / 100f * currentNightPercentage + 180;
        }
    }

    private void ApplyAngle()
    {
        CurrentSunAngle = AngleMod(Mathf.LerpAngle(CurrentSunAngle, TargetAngle, Time.deltaTime)); // Smooth the sun angle

        sunLight.transform.rotation = Quaternion.identity;
        sunLight.transform.Rotate(Vector3.forward, rotationCurveOffset);
        sunLight.transform.rotation *= Quaternion.AngleAxis(CurrentSunAngle, Vector3.right);
        
        if (IsDaytime)
        {
            sunLight.intensity = daySunIntensity.Evaluate(currentDayPercentage / 100f);
            moonLight.intensity = dayMoonIntensity.Evaluate(currentDayPercentage / 100f);

            sunLight.shadowStrength = daySunShadowStrength.Evaluate(currentDayPercentage / 100f);
        }
        else
        {
            moonLight.intensity = nightMoonIntensity.Evaluate(currentNightPercentage / 100f);
            sunLight.intensity = 0;
        }
    }

    public static TimeSpan VectorToTimeSpan(Vector3 vector)
    {
        return new TimeSpan((int)vector.x, (int)vector.y, (int)vector.z);
    }

    private float AngleMod(float angle)
    {
        return (angle % 360f + 360f) % 360f;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(CurrentTime.Ticks);
        }
        else
        {
            CurrentTime = TimeSpan.FromTicks((long)stream.ReceiveNext());
        }
    }
}
