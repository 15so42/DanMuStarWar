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

    public bool autoRun=true;

    
    // Start is called before the first frame update
    void Start()
    {
        taskCenter = GetComponent<TaskCenter>();
        taskCenter.Init(this);
        dayLightManager=DayLightManager.Instance;
        fightingManager=FightingManager.Instance;
        EventCenter.Broadcast(EnumEventType.OnMonsterSpawnerInit,this);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        taskCenter.Run();
        if(!autoRun)
           return;
        
        if (dayLightManager.IsDay() == false && FightingManager.Instance.gameStatus == GameStatus.Playing)
        {
            timer -= Time.deltaTime * timeScale;
            if (timer <= 0)
            {
                void GoToZeroPos(GameObject gameObject)
                {
                    UnityTimer.Timer.Register(1, () =>
                    {
                        if (gameObject)
                        {
                            var mcUnit = gameObject.GetComponent<McUnit>();
                            EventCenter.Broadcast(EnumEventType.OnMonsterInit, mcUnit);
                        }
                    });
                }
                
                taskCenter.AddTask(new PlanetTask(new TaskParams(TaskType.Create, "BattleUnit_Zombie", 1, GoToZeroPos),
                    null));
                timer = 90 - (fightingManager.roundManager.elapsedTime / 60);
                Debug.Log("间隔时间" + timer);
                if (timer < 45)
                {
                    timer = 45;
                }
            }
        }
    }
    
    void GoToZeroPos(GameObject gameObject)
    {
        UnityTimer.Timer.Register(1, () =>
        {
            if (gameObject)
            {
                var mcUnit = gameObject.GetComponent<McUnit>();
                EventCenter.Broadcast(EnumEventType.OnMonsterInit, mcUnit);
            }
        });
    }

    public void Spawn(string name)
    {
        taskCenter.AddTask(new PlanetTask(new TaskParams(TaskType.Create, name, 1, GoToZeroPos),
            null));
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
