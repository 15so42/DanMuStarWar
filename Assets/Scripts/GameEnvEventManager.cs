using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvEventManager : MonoBehaviour
{
    public static GameEnvEventManager Instance;

    public List<GameEnvEvent> events=new List<GameEnvEvent>();
    private void Awake()
    {
        Instance = this;
    }

    public int level = 0;

    public void Init()
    {
        level = 0;
    }

    public void PlayRandomEvent()
    {
        if(events.Count==0)
            return;
        events[UnityEngine.Random.Range(0, events.Count)].Run(level);
        level++;
    }

   
    
}
