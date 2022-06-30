using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ZombieSpawner : MonoBehaviour,ITaskAble
{
    private TaskCenter taskCenter;

    private DayLightManager dayLightManager;

    private FightingManager fightingManager;

    public float timeScale = 1;
    private float timer=90;
    // Start is called before the first frame update
    void Start()
    {
        taskCenter = GetComponent<TaskCenter>();
        taskCenter.Init(this);
        dayLightManager=DayLightManager.Instance;
        fightingManager=FightingManager.Instance;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        taskCenter.Run();
        if (dayLightManager.IsDay()==false && FightingManager.Instance.gameStatus==GameStatus.Playing)
        {
            timer -= Time.deltaTime*timeScale;
            if (timer <= 0)
            {
                
                
                taskCenter.AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_Zombie",1),null ));
                timer = 90 - (fightingManager.roundManager.elapsedTime / 60);
                Debug.Log("间隔时间"+timer);
                if (timer < 45)
                {
                    timer = 45;
                }
            }
        }
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
