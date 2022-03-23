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

    public void PlayRandomEvent()
    {
        events[UnityEngine.Random.Range(0, events.Count)].Run();
    }
    
}
