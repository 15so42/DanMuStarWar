using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PVEManager : MonoBehaviour
{
   

    private FightingManager fightingManager;
 
    //public List<ZombieSpawner> zombieSpawners=new List<ZombieSpawner>();
  

    public List<ZombieSpawner> spawners=new List<ZombieSpawner>();
    private List<string> toSpawnList=new List<string>();
    private void Start()
    {
        fightingManager=FightingManager.Instance;
        EventCenter.AddListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.AddListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.AddListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.AddListener(EnumEventType.OnBattleStart,OnBattleStart);
        SceneManager.LoadScene("McWarScene_Guard", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBattleStart()
    {
        var spawnersGo = GameObject.FindGameObjectsWithTag("ZombieSpawner");
        for (int i = 0; i < spawnersGo.Length; i++)
        {
            spawners.Add(spawnersGo[i].GetComponent<ZombieSpawner>());
        }
        StopAllCoroutines();
        StartCoroutine(SpawnMonster());
    }
   
    void OnMonsterSpawnerInit(ZombieSpawner zombieSpawner)
    {
        if (spawners .Count==0)
        {
            spawners.Add(zombieSpawner);
        }

        StopAllCoroutines();
        StartCoroutine(SpawnMonster());
    }

    float GetElapsedTime()
    {
        return fightingManager.roundManager.elapsedTime;
    }

    //决定附魔栏位和附魔次数
    void OnMonsterInit(McUnit mcUnit)
    {
        var pos = fightingManager.mcPosManager.GetPosByIndex(0);
        
        if (mcUnit)
        {
            
            mcUnit.GoMCPos(pos, false);

            var weapon = mcUnit.GetActiveWeapon();
            var spellCount = GetElapsedTime() / 90 + 1;

            var maxSpellSlot = (int)GetElapsedTime() / 300 + 1;
            weapon.SetMaxSpellCount(maxSpellSlot);
            for (int i = 0; i < spellCount; i++)
            {
                weapon.RandomSpell(false);
            }

            mcUnit.canSetPlanetEnemy = true;

        }
    }

    void SpawnByCount(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
            spawner.Spawn(toSpawnList[UnityEngine.Random.Range(0, toSpawnList.Count)]);
        }
    }

    //决定从哪里生成，能生成什么，生成几个
    void SpawnMonsterByTimeAndPopulation()
    {
        var time = GetElapsedTime();
        if (time < 300)
        {
            
            SpawnByCount((int)time/90+1);
        }
        else if (time < 900)
        {
            SpawnByCount((int)time/90);
        }
        else if (time < 1800)
        {
            SpawnByCount((int)time/120);
        }
        else
        {
            SpawnByCount((int)time/150);
        }
    }
    
    IEnumerator SpawnMonster()
    {
        toSpawnList.Add("BattleUnit_Zombie");

        int count = 5;
        while (count > 0)
        {
            yield return new WaitForSeconds(45);
            SpawnMonsterByTimeAndPopulation();
            count--;
        }
        
        toSpawnList.Add("BattleUnit_Skeleton");
        
        count = 5;
        while (count > 0)
        {
            yield return new WaitForSeconds(60);
            SpawnMonsterByTimeAndPopulation();
            count--;
        }
        
        toSpawnList.Add("BattleUnit_Creeper");

        int cd = 60;
        while (true)
        {
            yield return new WaitForSeconds(cd);
            SpawnMonsterByTimeAndPopulation();
            cd -= 5;
            if (cd < 10)
                cd = 10;
        }
    }

    void OnBattleOver()
    {
        StopAllCoroutines();
        spawners.Clear();
        toSpawnList.Clear();
        
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.RemoveListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.RemoveListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.RemoveListener(EnumEventType.OnBattleStart,OnBattleStart);
    }
}
