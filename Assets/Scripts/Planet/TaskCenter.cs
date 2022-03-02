using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCenter : MonoBehaviour
{
    public List<PlanetTask> tasks=new List<PlanetTask>();
    public void Run()
    {
        foreach (var t in tasks)
        {
            t.Run();
        }
    }
}
