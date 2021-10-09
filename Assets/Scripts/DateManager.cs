using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : MonoBehaviour
{


    private static System.DateTime startDate;
    private static System.DateTime today;




    public static DateManager dateManager;


    private void Awake()
    {
        if (dateManager == null)
            dateManager = this;
        else
            Destroy(this);
    }

    private void Start()
    {

        SetStartDate();
        Debug.Log(GetDaysPassed());
    }



    // send the passed days for each plant to finish the 30 days cycle
    void SetStartDate()
    {
        if (PlayerPrefs.HasKey("DateInitialized")) //if we have the start date saved, we'll use that
            startDate = System.Convert.ToDateTime(PlayerPrefs.GetString("DateInitialized"));
        else //otherwise...
        {
            startDate = System.DateTime.Now; //save the start date ->
            PlayerPrefs.SetString("DateInitialized", startDate.ToString());
        }
    }


    public static string GetDaysPassed()
    {
        today = System.DateTime.Now;

        //days between today and start date -->
        System.TimeSpan elapsed = today.Subtract(startDate);

        double days = elapsed.TotalDays;

        return days.ToString("0");
    }


    /// <summary>
    /// Get interval seconds
    /// </summary>
    /// <param name="startTimer"></param>
    /// <param name="endTimer"></param>
    /// <returns></returns>
    public int GetSubSeconds(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

        //Return the time difference (the return value is the number of seconds of the difference)
        return subTimer.Seconds;
    }

    /// <summary>
    /// Get the difference between two times in minutes
    /// </summary>
    /// <param name="startTimer"></param>
    /// <param name="endTimer"></param>
    /// <returns></returns>
    public int GetSubMinutes(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

        //Return the difference time (the return value is the number of minutes of the difference)
        return subTimer.Minutes;
    }


    /// <summary>
    /// Get the difference in hours between two times
    /// </summary>
    /// <param name="startTimer"></param>
    /// <param name="endTimer"></param>
    /// <returns></returns>
    public int GetSubHours(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

        //Return the length of the difference (the return value is the difference in hours)
        return subTimer.Hours;
    }

    /// <summary>
    /// Get the difference between two times in days
    /// </summary>
    /// <param name="startTimer"></param>
    /// <param name="endTimer"></param>
    /// <returns></returns>
    public int GetSubDays(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

        //Return the time difference (the return value is the number of days of the difference)
        return subTimer.Days;
    }

    public string DateTimeToTicks()
    {
        var ticks = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
        return ticks;
    }

    public DateTime TicksToDateTime(string timeTicks)
    {
        DateTime dateTime = JsonUtility.FromJson<JsonDateTime>(timeTicks);
        return dateTime;

    }


    public string GetFullTimeDifference(DateTime datetime)
    {
        var time = string.Format("Days {0} Hours {1} Minutes {2}", GetSubDays(datetime, DateTime.UtcNow), GetSubHours(datetime, DateTime.UtcNow), GetSubMinutes(datetime, DateTime.UtcNow));
        return time;
    }

    struct JsonDateTime
    {
        public long value;
        public static implicit operator DateTime(JsonDateTime jdt)
        {
            return DateTime.FromFileTimeUtc(jdt.value);
        }

        public static implicit operator JsonDateTime(DateTime dt)
        {
            JsonDateTime jdt = new JsonDateTime();
            jdt.value = dt.ToFileTimeUtc();
            return jdt;
        }
    }

}
