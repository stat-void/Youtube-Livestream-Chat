using System;
using UnityEngine;

/// <summary>
/// CURRENTLY BROKEN - For some reason, inverting APITimer calls with this resulted in Unity WebRequests not working.
/// ___
/// Extended Timer class where it's possible to ask how much time has passed before hitting the elapsed point. 
/// Borrowed from https://stackoverflow.com/questions/2278525/system-timers-timer-how-to-get-the-time-remaining-until-elapse;
/// Original code made by "Samuel Neff", with this variation by "Mike".
/// </summary>
public class TimerPlus : System.Timers.Timer
{
    private DateTime m_dueTime;
    private double TrueInterval;

    public TimerPlus() : base() => this.Elapsed += this.ElapsedAction;
    public double TimeLeft => (this.m_dueTime - DateTime.Now).TotalMilliseconds;

    public new double Interval
    {
        get
        {
            return base.Interval;
        }
        set
        {
            TrueInterval = value;
            base.Interval = value;
            Debug.Log($"trueinterval set to {TrueInterval}; base at {base.Interval}");
            
        }
    }

    public new void Start()
    {
        this.m_dueTime = DateTime.Now.AddMilliseconds(this.Interval);
        base.Start();
    }

    public new void Stop()
    {
        base.Interval = TrueInterval;
        base.Stop();
    }

    protected new void Dispose()
    {
        this.Elapsed -= this.ElapsedAction;
        base.Dispose();
    }

    public void Pause()
    {
        base.Interval = TimeLeft;
        base.Stop();
    }

    public void Resume()
    {
        this.m_dueTime = DateTime.Now.AddMilliseconds(this.Interval);
        base.Start();
    }


    private void ElapsedAction(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (base.Interval != TrueInterval)
            base.Interval = TrueInterval;

        if (this.AutoReset)
            this.m_dueTime = DateTime.Now.AddMilliseconds(this.Interval);
    }

    
}
