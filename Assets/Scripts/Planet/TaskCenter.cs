using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCenter : MonoBehaviour
{
    public ITaskAble owner;
    
    [HideInInspector]
    public List<PlanetTask> tasks=new List<PlanetTask>();

    public void Init(ITaskAble taskAble)
    {
        this.owner = taskAble;
    }
    
    public void Run()
    {
        if(tasks.Count<1)
            return;
        var t=tasks[0];   
        if(!t.isFininshed)
            t.Run();
        else
        {
            tasks.RemoveAt(0);
        }
        
    }

    public void AddTask(PlanetTask task)
    {
        task.owner = this.owner;
        tasks.Add(task);
    }
}
