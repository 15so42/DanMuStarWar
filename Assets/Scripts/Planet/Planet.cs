using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private PlanetCommander planetCommander;
    private PlanetResMgr planetResMgr;
    private TaskCenter[] taskCenters;
    
    // Start is called before the first frame update
    void Start()
    {
        planetCommander = GetComponent<PlanetCommander>();
        planetResMgr = GetComponent<PlanetResMgr>();
        taskCenters = GetComponents<TaskCenter>();
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
