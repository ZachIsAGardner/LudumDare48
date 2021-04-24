using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float Duration;
    public Action Action;
    private float time;

    public static Timer Create(float duration, Action action)
    {
        GameObject go = new GameObject("Timer");
        Timer timer = go.AddComponent<Timer>();
        timer.Duration = duration;
        timer.Action = action;
        timer.time = duration;
        return timer;
    }

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Action();
            Destroy(gameObject);
        }
    }   
}
