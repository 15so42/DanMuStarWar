using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private PlanetCommander planetCommander;
    [HideInInspector]
    public PlanetResMgr planetResMgr;
    private TaskCenter[] taskCenters;

    public Player owner = null;
    
    // Start is called before the first frame update
    void Start()
    {
        planetCommander = GetComponent<PlanetCommander>();
        planetResMgr = GetComponent<PlanetResMgr>();
        taskCenters = GetComponents<TaskCenter>();
        
        
        
        foreach (var t in taskCenters)
        {
            t.Init(this);
        }
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_纸飞机",5)));
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_纸飞机",5)));
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_纸飞机",5)));
    }

    public Player GetOwner()
    {
        return owner;
    }

    private void AddTask(PlanetTask planetTask)
    {
        taskCenters[0].AddTask(planetTask);
    }
    
    // Update is called once per frame
    void Update()
    {
        foreach (var taskCenter in taskCenters)
        {
            taskCenter.Run();
        }
    }
}
