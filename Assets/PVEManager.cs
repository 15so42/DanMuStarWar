using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PVEManager : MonoBehaviour
{

    public static PVEManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private FightingManager fightingManager;
 
    //public List<ZombieSpawner> zombieSpawners=new List<ZombieSpawner>();
  

    public List<ZombieSpawner> spawners=new List<ZombieSpawner>();
    private List<string> toSpawnList=new List<string>();

    //测试用timeScale
    public float timeScale = 1;
    private void Start()
    {
        fightingManager=FightingManager.Instance;
        EventCenter.AddListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.AddListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.AddListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.AddListener(EnumEventType.OnBattleStart,OnBattleStart);
        SceneManager.LoadScene("McWarScene_Guard", LoadSceneMode.Additive);
        RenderSettings.ambientLight=new Color(0.65f,0.65f,0.65f);
    }

    private float timer5 = 0;
    private void Update()
    {
        timer5 += Time.deltaTime;
        if (timer5 > 5)
        {
            if (PlanetManager.Instance.allPlanets.Count > 0)
            {
                var planetComanders = PlanetManager.Instance.allPlanets[0].planetCommanders;
                for (int i = 0; i < planetComanders.Count; i++)
                {
                    planetComanders[i].AddPoint(0.5f);
                }
            }
            
            timer5 = 0;
        }
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
            var spellCount = (GetElapsedTime() / 100) + 1;

            var maxSpellSlot = ((int)GetElapsedTime() / (30*60)) +3;
            if (GetElapsedTime() < 300)
            {
                spellCount = 0;
            }
            weapon.SetMaxSpellCount(maxSpellSlot);
            for (int i = 0; i < spellCount; i++)
            {
                weapon.RandomSpellBySpellCount();
            }

            mcUnit.canSetPlanetEnemy = true;

        }
    }

    void SpawnUnit(string unitName)
    {
        var spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
        spawner.Spawn(unitName);
        
    }
    public void SpawnByCount(int count)
    {
        var playerCount = fightingManager.players.Count;
        var rate = ((float) playerCount / 6);
        if (rate < 0.5f)
            rate = 0.5f;
        if (rate > 2f)
            rate = 2f;
        var realCount=Mathf.CeilToInt(count * rate );
        Debug.Log("生成"+realCount+"个野怪");
        for (int i = 0; i < realCount; i++)
        {
            var spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
            spawner.Spawn(toSpawnList[UnityEngine.Random.Range(0, toSpawnList.Count)]);
           
        }
    }
    
    

    //决定从哪里生成，能生成什么，生成几个
    void SpawnMonsterByTimeAndPopulation()
    {
        var time = GetElapsedTime();
        if (time < 300)//5分钟
        {
            SpawnByCount(((int)time/90)+2);
        }
        else if (time < 900)
        {
            SpawnByCount((int)time/120);
        }
        else if (time < 1800)//30分钟
        {
            SpawnByCount((int)time/120);
        }
        else
        {
            SpawnByCount((int)time/180);
        }
    }
    
    IEnumerator SpawnMonster()
    {
       
        
        toSpawnList.Add("BattleUnit_Zombie");
        //toSpawnList.Add("BattleUnit_Blaze");
        
        int count = 10;
        while (count > 0)
        {
            yield return new WaitForSeconds(30);
            SpawnMonsterByTimeAndPopulation();
            count--;
        }
        
        toSpawnList.Add("BattleUnit_Skeleton");
        
        
        count = 10;
        while (count > 0)
        {
            yield return new WaitForSeconds(45);
            SpawnMonsterByTimeAndPopulation();
            count--;
        }
        
        toSpawnList.Add("BattleUnit_Creeper");

        count = 10;
        while (count > 0)
        {
            yield return new WaitForSeconds(45);
            SpawnMonsterByTimeAndPopulation();
            count--;
        }
        
        StartCoroutine(SpawnGolems());
        toSpawnList.Add("BattleUnit_Blaze");
        
        count = 10;
        while (count > 0)
        {
            yield return new WaitForSeconds(45);
            SpawnMonsterByTimeAndPopulation();
            
        }
    }
    
    

    IEnumerator SpawnGolems()
    {
        while (true)
        {
            var count = 1;
            if (GetElapsedTime() > 3 * 600)
            {
                count = 2;
            }

            if (GetElapsedTime() > 4 * 600)
            {
                count = 4;
            }

            if (GetElapsedTime() > 5 * 600)
            {
                count = 6;
            }

            if (GetElapsedTime() > 6 * 600)
            {
                count = 8;
            }

            //1/3概率生成凋零而不是铁傀儡，生成的数量为铁傀儡的1/3,最少为1
            var rand = UnityEngine.Random.Range(0, 3);
            
            
            var toSpawn = "BattleUnit_EvilIronGolem";
            if (rand == 0 && GetElapsedTime()>=3*600)
            {
                count = Mathf.CeilToInt (count / 3f);
                toSpawn = "BattleUnit_Wither";

            }
           
            for (int i = 0; i < count; i++)
            {
                SpawnUnit(toSpawn);
            }
            
            yield return new WaitForSeconds(300);
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
