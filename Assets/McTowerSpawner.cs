using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McTowerSpawner : MonoBehaviour,ITaskAble
{
    private TaskCenter taskCenter;
    public int targetPlanetIndex;
    public bool canBeTarget = false;

    private void Awake()
    {
        taskCenter = GetComponent<TaskCenter>();
        EventCenter.AddListener(EnumEventType.OnBattleStart,SpawnTower);
        
    }

    // Start is called before the first frame update
    void Start()
    {
       // taskCenter=
        
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnTower()
    {
        void AddTask()
        {
            PlanetManager.Instance.allPlanets[targetPlanetIndex].AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_McPlanetTowerSmall",1,
                (go) =>
                {
                    go.transform.position = transform.position;
                }),null));
        }

        UnityTimer.Timer.Register(1, AddTask);

    }

    private void OnDisable()
    {
        EventCenter.RemoveListener(EnumEventType.OnBattleStart,SpawnTower);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetSpawnPoint()
    {
        return transform;
    }
}
