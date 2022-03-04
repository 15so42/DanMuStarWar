using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private PlanetCommander planetCommander;
    [HideInInspector]
    public PlanetResMgr planetResMgr;
    private TaskCenter[] taskCenters;

    public Player owner = null;
    [Header("手动设置半径")] public float radius=5;
    
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
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_探索船",5)));
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_纸飞机",5)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position,radius);
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
