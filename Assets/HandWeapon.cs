using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using Ludiq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityTimer;
using Random = System.Random;

[Serializable]
public class HandWeapon : Weapon,IDamageAble
{
    [Header("武器id")] public int mcWeaponId;
    
    private Transform root;
    [HideInInspector] public Animator animator;
    
    
    //附魔用nbt代替
    public SteveWeaponNbt weaponNbt;

    [Header("附魔列表")] public List<string> randomStrs = new List<string>();
    [Header("互斥附魔")] public List<string> mutexSpells = new List<string>();
    [Header("高阶附魔")] public List<string> rareRandomStrs=new List<string>();

    [Header("最大耐久")] public int maxEndurance=15;
    [Header("耐久")] public int endurance;
    

    [Header("击飞高度和力度")]
    public int pushBackHeight=4;
    public int pushBackStrength=1;
    
    //特定附魔计时
    private float lastThundersTime = 0;//落雷
    private float lastPhoenixTime = 0;
    private float lastRainAttackTime = 0;
    private float lastDrawTime = 0;
    private float lastSummonTime = 0;
    private float lastImmortalTime = 0;
    public float immortalCd = 5;
    private float lastJudgeTime = 0;
    private int pressAttackLayer = 0;//强攻层数
    private float lastDamageFixTime = 0;
    
    //记录召唤列表
    public List<McUnit> summons=new List<McUnit>();

    
    //鬼火数量
    public int ignisFatuus = 0;
    
    private void Awake()
    {
        randomStrs.Add("落雷");
        randomStrs.Add("雨裁");
        randomStrs.Add("汲取");
        randomStrs.Add("烈阳");
        randomStrs.Add("愤怒");
        randomStrs.Add("不死鸟");
        randomStrs.Add("坚韧");
        randomStrs.Add("噬魂");
        randomStrs.Add("自爆");
        randomStrs.Add("灵盾");
        randomStrs.Add("不灭");
        randomStrs.Add("过量治疗");
        randomStrs.Add("审判");
        randomStrs.Add("强攻");
        randomStrs.Add("振荡");
    }

    public void SetMaxSpellCount(int value)
    {
        weaponNbt.maxSpellCount = value;
    }
    
    
    
    public override void Init(BattleUnit owner)
    {
        base.Init(owner);
        root = transform.root;
        animator = root.GetComponentInChildren<BattleUnit>().animator;
        if(animator==null)
            Debug.Log(name+"需要手动配置");
        (owner as McUnit).SetAttackDistance(attackDistance);
        
        weaponNbt=new SteveWeaponNbt();

        endurance = maxEndurance;
        OnEnduranceChange(endurance,maxEndurance);

        
        
        AddEvent();
    }

    void AddEvent()
    {
        
        owner.onAttackOther += OnAttackOther;
        
        owner.onBeforeAttacked += OnBeforeAttacked;

        owner.onAttacked += OnAttacked;
        owner.onSlainOther += OnSlainOther;
        owner.onDie += OnOwnerDie;



    }

    public float GetAttackDistance()
    {
        var attackDistanceLevel = GetWeaponLevelByNbt("空间斩");
        return attackDistance + attackDistanceLevel*0.24f;
    }

    void OnOwnerDie()
    {
        var selfExplosionLevel = GetWeaponLevelByNbt("自爆");
        if (selfExplosionLevel > 0)
        {
            if(owner==null)
                Debug.LogError("自爆，owner为null");
            AttackManager.Instance.Explosion(new AttackInfo(owner,AttackType.Physics,Mathf.CeilToInt(selfExplosionLevel*2.5f)),this,owner.transform.position,15+selfExplosionLevel*0.3f );
            (owner.planetCommander as SteveCommander)?.ReduceRespawnTime(
                UnityEngine.Random.Range(1, selfExplosionLevel));
            //(owner.planetCommander as SteveCommander).unityTimer.ReduceDuration(UnityEngine.Random.Range(1,selfExplosionLevel));
            if(PVEManager.Instance)
                PVEManager.Instance.difficulty += 0.15f;
        }

        if (GetWeaponLevelByNbt("召唤") > 1)
        {
            foreach (var summon in summons)
            {
                summon.Die();
            }
        }
    }

    //记录噬魂的事件监听
    private bool addedSourCatch = false;
    //记录审判的事件监听
    private bool addedJudgeEvent = false;
    void OnMcUnitDie(McUnit mcUnit)
    {
        var sourCatchLevel = GetWeaponLevelByNbt("噬魂");
        if (mcUnit == null )
        {
            Debug.LogError("噬魂异常，mcunit为null");
            return;
        }

        if (owner == null || owner.die)
        {
            Debug.LogError("噬魂异常,owner为null");
            return;
        }
        
        //距离够近的话噬魂
        if (sourCatchLevel>0 && Vector3.Distance(owner.transform.position, mcUnit.transform.position) < 18)
        {
            var value = Mathf.CeilToInt(sourCatchLevel * 0.2f);
            owner.AddMaxHp(value);
            FlyText.Instance.ShowDamageText(owner.transform.position-Vector3.up*3,"噬魂("+value+")");
        }
    }

    public void AddEndurance(int value)
    {
        endurance += value;
        if (endurance > maxEndurance)
            endurance = maxEndurance;
        if (endurance <= maxEndurance * 0.2)
        {
            //(owner.planetCommander as SteveCommander).
            var steveCommander = owner.planetCommander as SteveCommander;
            var roundManager = FightingManager.Instance.roundManager as McRoundManager;
            if (steveCommander!=null && roundManager!=null && steveCommander.autoFixWeapon)
            {
               var success= roundManager.ParseFixWeapon(steveCommander);
               if (success)
               {
                   var damageFixLevel = GetWeaponLevelByNbt("振荡");
                   if (damageFixLevel > 0 && Time.time>lastDamageFixTime+4)
                   {
                       var damage =Mathf.CeilToInt(maxEndurance * 0.01f * damageFixLevel);
                       AttackManager.Instance.Explosion(new AttackInfo(owner,AttackType.Physics,damage),this,owner.transform.position,12, "DamageFixFx");
                       lastDamageFixTime = Time.time;
                   }
               }
            }
            
        }
        
        if (endurance < 0)
            endurance = 0;
        OnEnduranceChange(endurance,maxEndurance);
    }

    public virtual void Load(SteveWeaponNbt weaponNbt)
    {
        if(weaponNbt==null)//没有nbt数据
            return;
        this.endurance = weaponNbt.endurance;
        this.maxEndurance = weaponNbt.maxEndurance;
        this.weaponNbt = weaponNbt;
        
        OnSpellChange();
        OnEnduranceChange(endurance,maxEndurance);
        
    }

  

    public void SaveToCommander()
    {
        var steveCommander = owner.planetCommander as SteveCommander;
        if (steveCommander == null)
        {
            Debug.Log("steveCimmander为空");
            return;
        }
           
        
        steveCommander.steveWeaponNbt = this.weaponNbt;
        steveCommander.desireWeaponId = mcWeaponId;
        
    }

    public bool TryRandomSpell(bool byGift)
    {
        // if (weaponNbt.enhancementLevels.Count >= 3 && !byGift)
        // {
        //     MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔已达上限，投喂打call可继续附魔");
        //
        //     return false;
        // }

        // if (weaponNbt.enhancementLevels.Count >= randomStrs.Count )
        // {
        //     //MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"已获得所有附魔,无法再附魔了");
        //     //return false;
        // }
        
        
        

        return true;
    }

    public bool TrySpecificSpell(string name)
    {
        if (IsValidSpell(name) == false)
        {
            MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔失败，因为存在互斥附魔");
            return false;
        }
            
        if (randomStrs.Contains(name) == false)
        {
            var str = "";
            foreach (var t in randomStrs)
            {
                str += " "+ t;
            }
            MessageBox._instance.AddMessage(owner.planetCommander.player.userName,"附魔名称错误，可附魔列表为："+str);
            return false;
        }
        
        if ((owner.planetCommander as SteveCommander).leftSpecificSpell <= 0)
        {
            MessageBox._instance.AddMessage(owner.planetCommander.player.userName,"指定附魔次数已用完");
            return false;
        }
            
        if (weaponNbt.enhancementLevels.Find(x => x.enhancementName == name) != null)
        {
            return true;
        }
        //新附魔
        if (weaponNbt.enhancementLevels.Count >= weaponNbt.maxSpellCount)
        {
            MessageBox._instance.AddMessage(owner.planetCommander.player.userName,"达到附魔栏位限制");
            return false;
        }
        // if (weaponNbt.enhancementLevels.Count >= weaponNbt.maxSpellCount)
        // {
        //     owner.LogTip("到达附魔上限，投喂打call可增加附魔栏位");
        //     return false;
        // }

        

        return true;
    }

    public void SpecificSpell(string name)
    {
        SetWeaponLevel(name,GetWeaponLevelByNbt(name)+1);
        OnSpellChange();
    }

    public void OnlyUpdateSpell()
    {
        var randomEnhancementLevel =  weaponNbt.enhancementLevels[UnityEngine.Random.Range( 0,weaponNbt.enhancementLevels.Count)];
        randomEnhancementLevel.level++;
        OnSpellChange();
    }

    bool IsValidSpell(string spellStr)
    {
        if (mutexSpells.Contains(spellStr) )
        {
            foreach (var spell in weaponNbt.enhancementLevels)
            {
                if (mutexSpells.Contains(spell.enhancementName) && spell.enhancementName != spellStr)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 在附魔栏位限制内进行附魔
    /// </summary>
    public void RandomSpellBySpellCount()
    {
        if (weaponNbt.enhancementLevels.Count < weaponNbt.maxSpellCount)
        {
            RandomSpell(false);
        }
        else
        {
            OnlyUpdateSpell();
        }
    }
    
    //随机附魔
    public void RandomSpell(bool rare)
    {
        string spellStr = "";
        do
        {
            spellStr = randomStrs[UnityEngine.Random.Range(0, randomStrs.Count)];
        }
        while(!IsValidSpell(spellStr));

        if (owner.planetCommander != null)
        {
            MessageBox._instance.AddMessage("系统", owner.planetCommander.player.userName + "附魔" + spellStr);
        }
            
        
        SetWeaponLevel(spellStr, GetWeaponLevelByNbt(spellStr) + 1);


        OnSpellChange();
        

    }

    public bool RemoveSpell(int index)
    {
        if (index - 1 < 0 || index - 1 >= weaponNbt.enhancementLevels.Count || weaponNbt.enhancementLevels.Count==0)
        {
            owner.planetCommander.commanderUi.LogTip("序号错误");
            return false;
        }

        var spellLevel = weaponNbt.enhancementLevels[index-1].level;
        (owner.planetCommander as SteveCommander)?.AddPoint(spellLevel*3);
        MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"祛魔返还"+spellLevel*3+"点数");
        
        weaponNbt.enhancementLevels.RemoveAt(index-1);
       
        OnSpellChange();
        return true;

    }
    
    public void OnSpellChange()        
    {
        var str = "";
        for (int i = 0; i < weaponNbt.enhancementLevels.Count; i++)
        {
            if (weaponNbt.enhancementLevels[i].level>0)
            {
                str += "|" + weaponNbt.enhancementLevels[i].enhancementName+weaponNbt.enhancementLevels[i].level;
            }
        }

        try
        {
            if(owner.hpUI)
                owner.hpUI.SetWeaponText(weaponName + str);
        }
        catch (Exception e)
        {
            Debug.LogError("特效报错原因"+e.Message);
        }
        
        (owner as McUnit)?.SetAttackDistance(GetAttackDistance());
        
        //特效
        var angry = GetWeaponLevelByNbt("愤怒");
        if (angry > 0)
        {
            
           (owner as McUnit).OpenAngryFx();
            
        }
        else
        { 
            (owner as McUnit).CloseAngryFx();
        }

        var phoenix = GetWeaponLevelByNbt("不死鸟");
        if (phoenix > 0)
        {
            (owner as McUnit).OpenPhoenixFx();
        }
        else
        {
            (owner as McUnit).ClosePhoenixFx();
        }
        
        var searingSun = GetWeaponLevelByNbt("烈阳");
        if (searingSun > 0)
        {
            (owner as McUnit).OpenSunFx();
        }
        else
        {
            (owner as McUnit).CloseSunFx();
        }

        var judge = GetWeaponLevelByNbt("审判");
        {
            if (judge > 0 )
            {
                if (!addedJudgeEvent)
                {
                    EventCenter.AddListener<IAttackAble,IVictimAble>(EnumEventType.OnUnitDamageOther,Judge);
                    addedJudgeEvent = true;
                }
            }
        }

        var sourCatch = GetWeaponLevelByNbt("噬魂");
        if (sourCatch > 0)
        {
            //噬魂附魔
            if (!addedSourCatch)
            {
                EventCenter.AddListener<McUnit>(EnumEventType.OnMcUnitDied,OnMcUnitDie);
                addedSourCatch = true;
            }
            
        }
    }


    void Judge(IAttackAble attackAble,IVictimAble victimAble)
    {
        if (Time.time < lastJudgeTime+4)
        {
            return;
        }

        lastJudgeTime = Time.time;
        if (owner == null || owner.die)
        {
            Debug.LogError("审判Owner异常null");
            return;
        }
        if(attackAble.GetAttackerOwner()==owner.GetAttackerOwner())
            return;//同队伍不触发
        var judgeLevel = GetWeaponLevelByNbt("审判");



        var rate = Mathf.CeilToInt(judgeLevel*0.5f);
        if (true||UnityEngine.Random.Range(0, 100) < rate)
        {
            ResFactory.Instance.CreateFx("JudgeFx", attackAble.GetAttackEntity().transform.position);
            //(attackAble as IVictimAble)?.OnAttacked(new AttackInfo(owner,AttackType.Real,rate));
            DamageOther(attackAble as IVictimAble, new AttackInfo(owner,AttackType.Real,rate));
        }
    }

    public override bool FireCheck()
    {
        if (endurance <= 0)
        {
            return false;
        }
        
        var distance =
            Vector3.Distance(root.transform.position, owner.chaseTarget.GetVictimEntity().transform.position);
        if (distance < GetAttackDistance())
        {
            return true;
        }

        return false;
    }

    public virtual void FireAnim()
    {
        animator.SetTrigger("Attack");
        
        Invoke(nameof(DamageChaseTarget),0.3f);
    }

    public override void Fire()
    {
        FireAnim();

        
        
        OnEnduranceChange(endurance,maxEndurance);
        AddEndurance(-1);
    
    }

    public void OnEnduranceChange(int endurance,int maxEndurance)
    {
        (owner as McUnit).UpdateWeaponEndurance(endurance,maxEndurance);
        weaponNbt.endurance = endurance;
        weaponNbt.maxEndurance = maxEndurance;
    }
    
    
    /// <summary>
    /// 召唤兽
    /// </summary>
    /// <param name="gameObject"></param>
    void GoToZeroPos(GameObject gameObject)
    {
        UnityTimer.Timer.Register(1, () =>
        {
            if (gameObject)
            {
                var mcUnit = gameObject.GetComponent<McUnit>();
                //EventCenter.Broadcast(EnumEventType.OnMonsterInit, mcUnit);
                mcUnit.GoMCWorldPos(owner.transform.position,false);
                summons.Add(mcUnit);
                
                if (mcUnit as Zombie)
                {
                    ((Zombie) mcUnit).selfFire=false;
                    ((GameEntity) mcUnit).isSummoned=true;
                }

                mcUnit.planetCommander = owner.planetCommander;
                //附魔
                var weapon = mcUnit.GetActiveWeapon();

                var summonLevel = GetWeaponLevelByNbt("召唤");
                var spellCount = Mathf.CeilToInt((summonLevel / 2.5f) +1);

                var maxSpellSlot = summonLevel/14 +1;
                
                //凋零攻击力减半
                if (mcUnit as Wither)
                {
                    weapon.attackValue /= 2;
                    //mcUnit.AddMaxHp((int)(-0.5f*mcUnit.props.maxHp));
                }
                
                weapon.SetMaxSpellCount(maxSpellSlot);
                for (int i = 0; i < spellCount; i++)
                {
                    weapon.RandomSpellBySpellCount();
                }

                mcUnit.canSetPlanetEnemy = true;
            }
            
        });
    }
    
    //更新鬼火
    void AddIgnisFatuus(int value)
    {
        ignisFatuus += value;
        UpdateIgnisFatuus();
    }

    private SummonPointUi summonPointUi;
    private void UpdateIgnisFatuus()
    {
        if (summonPointUi == null)
        {
            summonPointUi = GameManager.Instance.uiManager.CreateSummonPointUi(owner);
            
            summonPointUi.offset=new Vector3(-70,8.7f,0);
            summonPointUi.UpdateUi(ignisFatuus);
        }
        else
        {
            summonPointUi.UpdateUi(ignisFatuus);
        }
        
        
        //owner.hpUI.SetNameText(owner.planetCommander.player.userName +"🔥"+ignisFatuus);
    }

    public void Summon(string name)
    {
        int summonLevel = GetWeaponLevelByNbt("召唤");
        bool canSummon = summons.Count <= (summonLevel / 7 + 1);

        if (canSummon)
        {
            var chineseSummonList=new List<string>()
            {
                "僵尸","骷髅","苦力怕",
                "烈焰人","铁傀儡","凋零"
            };
            
            var summonList = new List<string>()
            {
                "BattleUnit_Zombie","BattleUnit_Skeleton","BattleUnit_Creeper",
                "BattleUnit_Blaze","BattleUnit_IronGolem","BattleUnit_Wither"
            };

            var costList = new List<int>()
            {
                2, 3, 2, 4, 8,24
            };

            if (!chineseSummonList.Contains(name))
            {
                MessageBox._instance.AddMessage("系统","召唤名称错误");
                return;
            }

            var index = chineseSummonList.IndexOf(name);
            var cost = costList[index];
            if (ignisFatuus >= cost)
            {
                var summonName = summonList[index];
                var planet = owner.GetAttackerOwner() as Planet;
                if (planet != null)
                    planet.AddTask(new PlanetTask(new TaskParams(TaskType.Create, summonName, 1, GoToZeroPos),
                        null));
                AddIgnisFatuus(-1*cost);
            }
            else
            {
                MessageBox._instance.AddMessage("系统", "召唤"+name+"所需鬼火不足");
            }

        }
        else
        {
            MessageBox._instance.AddMessage("系统","达到召唤限制");
        }
    }
    

    private float spellTimer = 0;
    private float lastTimer = 0;//每秒执行
    protected override void Update()
    {
        base.Update();
        spellTimer += Time.deltaTime;
        lastTimer += Time.deltaTime;

        if (lastTimer > 1)
        {
            lastTimer = 0;
        }
        
        if (spellTimer > 5)
        {
            var enduranceLevel = GetWeaponLevelByNbt("耐久");
            if (enduranceLevel >0)
            {
               
                AddEndurance(enduranceLevel);
                if (endurance >= maxEndurance)
                {
                    //(owner.planetCommander as SteveCommander).AddPoint(0.07f*enduranceLevel);
                     var random=UnityEngine.Random.Range(0, 100);
                    
                         if (random < 10 * enduranceLevel)
                         {
                             maxEndurance++;
                             AddEndurance(1);
                             FlyText.Instance.ShowDamageText(owner.transform.position,"耐久上限+1");
                         }
                     
                }
                
            }




            
            
            var sharpLevel = GetWeaponLevelByNbt("锋利");
            if (sharpLevel > 0)
            {
                var skill=SkillManager.Instance.AddSkill("Skill_加速_LV1", owner, owner.planetCommander, (skill) =>
                {
                    (skill as AccelerateSkill).addMoveSpeed = sharpLevel * 0.35f;
                });
                
            }

            



            var fortuneLevel= GetWeaponLevelByNbt("财运");
            if (fortuneLevel > 0)
            {
                if (owner.planetCommander!=null)
                {
                    (owner.planetCommander as SteveCommander).AddPoint(0.06f*fortuneLevel);
                }
                
            }

            var searingSun = GetWeaponLevelByNbt("烈阳");
            if (searingSun > 0)
            {
                (owner as McUnit).OpenSunFx();
                var enemies = AttackManager.Instance.GetEnemyInRadius(owner, owner.transform.position, owner.findEnemyDistance, 90);
                foreach (var e in enemies)
                {
                    e.OnAttacked(new AttackInfo(owner, AttackType.Real,
                        Mathf.CeilToInt(owner.props.maxHp * 0.004f * searingSun)));
                }
            }
            else
            {
                (owner as McUnit).CloseSunFx();
            }
            
            
            /**加*/
            //rate:几率数组（%），  total：几率总和（100%）
            // Debug.Log(rand(new int[] { 10, 5, 15, 20, 30, 5, 5,10 }, 100));
            int Rand(int[] rate, int total)
            {
                int r = UnityEngine.Random.Range(1, total+1);
                int t = 0;
                for (int i = 0; i < rate.Length; i++)
                {
                    t += rate[i];
                    if (r < t)
                    {
                        return i;
                    }
                }
                return 0;
            }
            // ————————————————
            // 版权声明：本文为CSDN博主「幻世界」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
            // 原文链接：https://blog.csdn.net/qq_37310110/article/details/86139130

            var summonLevel = GetWeaponLevelByNbt("召唤");
            if (summonLevel > 0)
            {
                //召唤物集中过来
                for (int i = 0; i < summons.Count; i++)
                {
                    if (summons[i] == null || summons[i].IsAlive() == false)
                    {
                        summons.RemoveAt(i);
                        i--;
                    }
                    
                    //summons[i].GoMCWorldPos(owner.transform.position,false);
                }

                var rand = UnityEngine.Random.Range(0, 5);
                
                if (rand < 1)
                {
                    AddIgnisFatuus(1);
                }
                
                
            }

            var healLevel = GetWeaponLevelByNbt("回复");
            if (healLevel > 0)
            {
                var healValue = healLevel;
                
                owner.OnAttacked(new AttackInfo(owner, AttackType.Heal, Mathf.CeilToInt(healValue)));
                
                if (healLevel >= 4)
                {
                    var colliders = Physics.OverlapSphere(transform.position, owner.findEnemyDistance);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        var collider1 = colliders[i];
                        var gameEntity = collider1.GetComponent<GameEntity>();
                        if (!gameEntity) //不是单位
                            continue;

                        if (gameEntity.die) //已经死亡
                            continue;
                        if(gameEntity==owner)
                            continue;
                        
                        var gameEntityOwner = gameEntity.GetVictimOwner();
                        if (gameEntityOwner == owner.GetAttackerOwner()) //治疗友军
                        {
                            gameEntity.OnAttacked(new AttackInfo(owner,AttackType.Heal,Mathf.CeilToInt(healValue*0.2f)));
                        }
                        else
                        {
                            if (healLevel >= 8)
                            {
                                var hpAndShield = gameEntity.OnAttacked(new AttackInfo(owner,AttackType.Physics,1));
                                OnDamageOther(gameEntity,hpAndShield);
                            }
                            
                        }
                        
                    }

                }
            }
            
            
            var spiritualShieldLevel = GetWeaponLevelByNbt("灵盾");
            if (spiritualShieldLevel > 0)
            {
               
              
                    owner.props.maxShield = (int)(spiritualShieldLevel*10);
                    //float shieldValue = owner.props.maxHp * (0.09f+spiritualShieldLevel*0.01f);

                    var value = (int) (spiritualShieldLevel * 0.5f);
                    owner.AddShield(value);
                    
                    FlyText.Instance.ShowDamageText(owner.transform.position-Vector3.up*3 ,"灵盾("+value+")");
                
            }

           


            spellTimer = 0;
        }
    }

    public virtual AttackInfo GetBaseAttackInfo()
    {
        return new AttackInfo(this.owner, AttackType.Physics, attackValue);
    }

    
    public void DamageOther(IVictimAble victimAble,AttackInfo attackInfo)
    {
        if(victimAble==null)
            return;

        var victim = victimAble.GetVictimEntity();

        var pressAttackLevel = GetWeaponLevelByNbt("强攻");
        if (pressAttackLevel > 0)
        {
            if (pressAttackLayer < pressAttackLevel)
            {
                pressAttackLayer++;
            }
            else
            {
                pressAttackLayer = pressAttackLayer / 2;
            }

            attackInfo.value += pressAttackLayer*2;

        }

        var angry = GetWeaponLevelByNbt("愤怒");
        if (angry > 0)
        {
            if (owner.props.hp > 0)
            {
                var count = (1 - (float)owner.props.hp / owner.props.maxHp)/0.1f;
                // if (count > 5)
                // {
                //     (owner as McUnit).OpenAngryFx();
                // }
                // else
                // {
                //     (owner as McUnit).CloseAngryFx();
                // }
                attackInfo.value += Mathf.CeilToInt(0.15f * angry * count);
            }
            
        }
        
        var sharpLevel = GetWeaponLevelByNbt("锋利");
        if (sharpLevel > 0)
        {
            attackInfo.value=Mathf.CeilToInt(attackInfo.value*(1+ (0.25f+sharpLevel*0.1f)));
        }

        var swordKing = GetWeaponLevelByNbt("剑圣");
        if (swordKing > 0)
        {
            // var cost = Mathf.CeilToInt(swordKing * ( 1 - (float)swordKing/(swordKing+20) ));
            // if (endurance > cost)
            // {
            //     attackInfo.value=Mathf.CeilToInt(attackInfo.value + cost);
            //     AddEndurance(-1*cost);
            // }
            
            if (endurance >= maxEndurance*0.5f)
            {
                attackInfo.value += endurance;
                AddEndurance(-1*endurance);
                AddEndurance(Mathf.CeilToInt(0.01f*swordKing*maxEndurance));
                owner.OnAttacked(new AttackInfo(owner, AttackType.Heal, Mathf.CeilToInt(0.01f*swordKing*maxEndurance)));
            }
            
        }
       
        
        var spineLevel = GetWeaponLevelByNbt("尖刺");
        if (spineLevel > 0)
        {
            attackInfo.value += spineLevel;
        }
        
        var ghostKillLevel = GetWeaponLevelByNbt("亡灵杀手");
        if (ghostKillLevel > 0)
        {
            if (victim as Zombie)
            {
                attackInfo.value += 3 * ghostKillLevel;
            }
            
        }

       

        var eatLevel = GetWeaponLevelByNbt("吞噬");
        if (eatLevel > 0)
        {
            attackInfo.value += Mathf.CeilToInt (owner.props.maxHp * ( 0.02f * eatLevel));
            float rate = (float)eatLevel / (eatLevel + 20);
            var random = UnityEngine.Random.Range(0, 100);
            if (random < rate * 100)
            {
                owner.props.maxHp++;
                if (owner.planetCommander != null)
                {
                    (owner.planetCommander as SteveCommander).desireMaxHp = owner.props.maxHp;
                }
                
                owner.OnAttacked(new AttackInfo(owner, AttackType.Heal, 1));
                FlyText.Instance.ShowDamageText(owner.transform.position-Vector3.up*2, "最大生命值+1");
            }
        }
        
        var criticalLevel=GetWeaponLevelByNbt("暴击");
        if (criticalLevel > 0)
        {
            var rate = UnityEngine.Random.Range(0, 100);
            var multiplier = 1.5f + criticalLevel * 0.1f;

            var str = "暴击";
            if (rate < criticalLevel*20)
            {
                attackInfo.value = (int) (attackInfo.value * multiplier);
            }
            
            //双重暴击
            var random1= UnityEngine.Random.Range(0, 100);
            if(random1<10)
            {
                attackInfo.value = (int)multiplier*attackInfo.value;
                str = "双重暴击";
            }
            //FlyText.Instance.ShowDamageText(owner.transform.position,str);
        }

        
        var realDamage= victim.OnAttacked(attackInfo);
        var navMeshMoveManager = victim.GetComponent<NavMeshMoveManager>();
        // if(navMeshMoveManager)
        //     navMeshMoveManager.PushBack(victim.transform.position-transform.position+Vector3.up*pushBackHeight,pushBackStrength);
        if(navMeshMoveManager)
            navMeshMoveManager.PushBackByPos(victim.transform.position,owner.GetAttackEntity().transform.position,pushBackHeight,pushBackStrength);
        
        
        
        OnDamageOther(victim,realDamage);
    }

    public void DamageChaseTarget()
    {
        if(owner.chaseTarget==null)
            return;
        //var victim = owner.chaseTarget.GetVictimEntity();
        var attackInfo = GetBaseAttackInfo();
        DamageOther(owner.chaseTarget,attackInfo);
    }


    
    public void OnAttackOther(IVictimAble victimAble,int damage)
    {
        

        
    }
    
    
    //通过武器造成伤害
    public GameEntity GetVictimOwner()
    {
        return owner;
    }

    public GameEntity GetVictimEntity()
    {
        return owner;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }


    public virtual void DamageOtherFx(IVictimAble victimAble)
    {
        
    }

    public void OnDamageOther(IVictimAble victimAble, BattleUnitProps.HpAndShield realDamage)
    {
        if (gameObject.activeSelf == false)
            return;
        DamageOtherFx(victimAble);
        
        EventCenter.Broadcast(EnumEventType.OnUnitDamageOther,owner as IAttackAble,victimAble);
        
        var heavyAttackLevel = GetWeaponLevelByNbt("重击");
        if (heavyAttackLevel > 0)
        {
            var rand = UnityEngine.Random.Range(0, 10);
            if (rand < 3)
            {
                var value = Mathf.CeilToInt(victimAble.GetVictimEntity().props.maxHp * 0.01f * heavyAttackLevel);
                if (value > owner.props.maxHp * 2)
                    value = owner.props.maxHp * 2;
                victimAble.OnAttacked(new AttackInfo(owner, AttackType.Physics,
                    value,Color.red));
                
            }
           
        }

        var spaceChopperLevel = GetWeaponLevelByNbt("空间斩");
        if (spaceChopperLevel > 0)
        {
            var rand = UnityEngine.Random.Range(0, 100);
            if (rand < spaceChopperLevel)
            {
                victimAble.OnAttacked(new AttackInfo(owner, AttackType.Real, spaceChopperLevel));
                AttackManager.Instance.SpaceChopperFx(victimAble.GetVictimEntity().transform.position);
            }
        }
        
        var immortalLevel = GetWeaponLevelByNbt("不灭");
        if (immortalLevel > 0 && Time.time>lastImmortalTime+immortalCd)
        {

            var value = Mathf.CeilToInt(owner.props.maxHp * immortalLevel * 0.01f);
            victimAble.OnAttacked(new AttackInfo(owner, AttackType.Physics, value));
            owner.OnAttacked(new AttackInfo(owner, AttackType.Heal, value));
            
            (owner as McUnit)?.PlayImmortalFx();
            lastImmortalTime = Time.time;
        }
        
        //落雷
        var thunderLevel = GetWeaponLevelByNbt("落雷");
        if (thunderLevel > 0 && Time.time>lastThundersTime+9)
        {
            float radius = 5+ thunderLevel ;
            if (radius > 30)
                radius = 30;
            var count = 1+thunderLevel / 5;
            var attackInfo = GetBaseAttackInfo();
            attackInfo.value += Mathf.CeilToInt( thunderLevel*0.55f);
            attackInfo.attackType =  AttackType.Real;
            //var damage = new AttackInfo(attackInfo.attacker, attackInfo.attackType, attackInfo.value * 5);
            if (owner.chaseTarget != null)
            {
                Vector3 targetPos = owner.chaseTarget.GetVictimEntity().GetVictimPosition();
                AttackManager.Instance.Thunder(owner,attackInfo,this,targetPos+Vector3.up*60,6,targetPos,radius,count);
                lastThundersTime = Time.time;
            }
           
        }

        var rainAttack = GetWeaponLevelByNbt("雨裁");
        if (rainAttack > 0 && Time.time>lastRainAttackTime+2)
        {
            var enemies = AttackManager.Instance.GetEnemyInRadius(owner, victimAble.GetVictimPosition(), 15, 90);
            AttackManager.Instance.AttackEnemies(enemies,new AttackInfo(owner,AttackType.Physics,rainAttack));
            AttackManager.Instance.RainAttackFx(victimAble.GetVictimPosition());
            lastRainAttackTime = Time.time;

        }
        
        var fireLevel = GetWeaponLevelByNbt("火焰");
        if (fireLevel > 0)
        {
            if (victimAble.GetVictimEntity())
            {
                var skill=SkillManager.Instance.AddSkill("Skill_着火_LV1", victimAble.GetVictimEntity(), owner.planetCommander);
                if (skill as FireSkill)//第一次附加火焰没问题，但是之后无法再附加火焰而是刷新火焰Buff
                {
                    (skill as FireSkill).SetAttacker(owner); 
                    (skill as FireSkill).life = 3 + fireLevel;
                    //(skill as FireSkill).damage = Mathf.CeilToInt((float)fireLevel*0.2f);
                }
                
            }
            
        }
        

        var drawLevel = GetWeaponLevelByNbt("汲取");
        if (drawLevel > 0)
        {
            if (victimAble.GetVictimEntity() && Time.time>lastDrawTime+2.5f)
            {
                var skill=SkillManager.Instance.AddSkill("Skill_汲取_LV1", victimAble.GetVictimEntity(), owner.planetCommander);
                if (skill as DrawSkill)//第一次附加火焰没问题，但是之后无法再附加火焰而是刷新火焰Buff
                {
                    (skill as DrawSkill).SetAttacker(owner);
                    (skill as DrawSkill).damage = Mathf.CeilToInt((float)drawLevel*0.2f);
                }

                lastDrawTime = Time.time;

            }
        }
        
        
       
        
        var poisonLevel = GetWeaponLevelByNbt("毒");
        if (poisonLevel > 0)
        {
            if (victimAble.GetVictimEntity())
            {
                var skill=SkillManager.Instance.AddSkill("Skill_毒_LV1", victimAble.GetVictimEntity(), owner.planetCommander);
                if (skill as PoisonSkill)//第一次附加火焰没问题，但是之后无法再附加火焰而是刷新火焰Buff
                {
                    (skill as PoisonSkill).SetAttacker(owner);
                    var maxHp = victimAble.GetVictimEntity().props.maxHp;
                    (skill as PoisonSkill).SetAttackDamage(1+Mathf.CeilToInt (0.0045f*poisonLevel*maxHp)); 
                    (skill as PoisonSkill).life = 3;
                }
                
            }
            
        }

        var vampireLevel = GetWeaponLevelByNbt("吸血");

        if (vampireLevel > 0)
        {
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,Mathf.CeilToInt(realDamage.calAttackInfo.value * (0.1f+0.05f*vampireLevel))));
        }
    }

    
    
    //招架
    public void OnAttacked(AttackInfo attackInfo)
    {
        if (gameObject.activeSelf == false)
            return;
        if (attackInfo.attackType == AttackType.Heal)
        {
            return;
        }
        
        // var thronLevel = GetWeaponLevelByNbt("荆棘");
        // if (thronLevel > 0 && attackInfo.attackType!=AttackType.Reflect)
        // {
        //     attackInfo.attacker.GetAttackEntity()
        //         .OnAttacked(new AttackInfo(owner, AttackType.Reflect, Mathf.CeilToInt(attackInfo.value * (0.15f + thronLevel*0.05f))));
        // }
    }

    public void OnSlainOther(GameEntity victim)
    {
        if (gameObject.activeSelf == false)
            return;

        try
        {
            var triumphLevel = GetWeaponLevelByNbt("凯旋");
            if (triumphLevel > 0)
            {
                // float total = victim.props.maxHp * (0.15f + triumphLevel * 0.1f);//能回的
                // float need = owner.props.maxHp - owner.props.hp;//该回的
                // float overflow = total - need;
                // if (overflow>0)
                // {
                //     if (overflow > owner.props.maxHp * 0.5f)
                //     {
                //         overflow = owner.props.maxHp * 0.5f;
                //     }
                //     owner.props.maxHp += Mathf.CeilToInt(overflow);
                //     (owner.planetCommander as SteveCommander).desireMaxHp = owner.props.maxHp;
                // }
                // owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,Mathf.CeilToInt(total)));  

                bool isSteve = victim as Steve;

                float multiplier = 0.5f; //玩家用0.5倍
                if (isSteve == false) //非玩家效能减半
                {
                    multiplier *= 0.5f;
                    Debug.Log("凯旋非玩家，效能减半");
                }

                if (GetAttackDistance() > 10)
                {
                    multiplier *= 0.5f;
                    Debug.Log("远程，效能减半");
                }

                int total = Mathf.CeilToInt((triumphLevel * multiplier));


                if (total > 0)
                {
                    owner.props.maxHp += total;
                    if (owner.planetCommander != null)
                    {
                        (owner.planetCommander as SteveCommander).desireMaxHp = owner.props.maxHp;

                    }

                    FlyText.Instance.ShowDamageText(owner.transform.position - Vector3.up * 2, "最大生命+" + total);
                }

                owner.OnAttacked(new AttackInfo(owner, AttackType.Heal, total * 2 * 2));
            }



            var expFixLevel = GetWeaponLevelByNbt("经验修补");
            if (expFixLevel > 0)
            {

                maxEndurance += Mathf.CeilToInt(expFixLevel * 0.2f);
                AddEndurance((int) expFixLevel * 2);
                //FlyText.Instance.ShowDamageText(owner.transform.position,"经验修补");
            }

            var pressAttackLevel = GetWeaponLevelByNbt("强攻");
            if (pressAttackLevel > 0)
            {
                pressAttackLayer = 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("HandWeapon OnSlainOther异常"+e.Message);
        }
    }
    
    //灵盾
    private float lastSpiritualShieldTime = 0;

    public virtual AttackInfo OnBeforeAttacked(AttackInfo attackInfo)
    {
        attackInfo=new AttackInfo(attackInfo.attacker,attackInfo.attackType,attackInfo.value);
        if (gameObject.activeSelf == false)
            return attackInfo;

        var overHealLevel = GetWeaponLevelByNbt("过量治疗");
        if (overHealLevel > 0 && attackInfo.attackType == AttackType.Heal)
        {
            var sub = owner.props.maxHp - owner.props.hp;
            var shield = Mathf.CeilToInt((attackInfo.value - sub)*0.08f*overHealLevel);
            owner.AddShield(shield>0?shield:0);
        }
        
        
        var thronLevel = GetWeaponLevelByNbt("荆棘");
        var ignoreDamageType = new List<AttackType>() {AttackType.Reflect,AttackType.Heal,AttackType.Fire,AttackType.Poison};
        if (thronLevel > 0 && !ignoreDamageType.Contains(attackInfo.attackType))
        {
            var value = Mathf.CeilToInt(attackInfo.value * ((float)thronLevel / (thronLevel + 20)));
            attackInfo.attacker.GetAttackEntity()
                .OnAttacked(new AttackInfo(owner, AttackType.Reflect, value));
        }
        
        var mirrorShield=GetWeaponLevelByNbt("镜盾");
        var msIgnoreDamageType= new List<AttackType>() {AttackType.Reflect,AttackType.Heal,AttackType.Fire,AttackType.Poison};
        if (mirrorShield > 0 && !msIgnoreDamageType.Contains(attackInfo.attackType))
        {
            var value = Mathf.CeilToInt(0.16f*mirrorShield);
            attackInfo.attacker.GetAttackEntity()
                .OnAttacked(new AttackInfo(owner, AttackType.Reflect, value));
        }
        
        
        var toughLevel = GetWeaponLevelByNbt("坚韧");
        var toughIgnoreDamageType = new List<AttackType>() {AttackType.Reflect,AttackType.Heal,AttackType.Poison,AttackType.Real,AttackType.Thunder};
        if (toughLevel > 0 && !toughIgnoreDamageType.Contains(attackInfo.attackType))
        {
            attackInfo.value -= Mathf.CeilToInt(0.5f*toughLevel);
            if (attackInfo.value < 0)
                attackInfo.value = 0;
        }
        
        var protectionLevel=GetWeaponLevelByNbt("保护");
        var ignoreDamageType1 = new List<AttackType>() {AttackType.Reflect,AttackType.Heal,AttackType.Real,AttackType.Thunder};
        if (protectionLevel>0 && !ignoreDamageType1.Contains(attackInfo.attackType))
        {
            attackInfo.value = (int) (attackInfo.value * (1 - (float) protectionLevel / (protectionLevel + 10)));
        }

        var squamaLevel = GetWeaponLevelByNbt("龙鳞");
        if (squamaLevel > 0)
        {
            if(attackInfo.attackType!=AttackType.Physics && attackInfo.attackType!=AttackType.Heal)
                attackInfo.value = (int) (attackInfo.value * 0.5f);
        }

        // var arrowProteLevel=GetWeaponLevelByNbt("弹射物保护")
        // {
        //     
        // }

        var phoenix = GetWeaponLevelByNbt("不死鸟");
        if (phoenix > 0)
        {
            if (owner.props.hp > 0)
            {
                var hpRate = (float)owner.props.hp / owner.props.maxHp;
                if ( attackInfo.attackType!=AttackType.Heal)
                {
                    if (Time.time >= lastPhoenixTime+2 )
                    {
                        var rand = UnityEngine.Random.Range(0, 100);
                        if (rand < ((1-hpRate)*0.5f)*100f)
                        {
                            var value = Mathf.CeilToInt(phoenix*0.5f);
                            owner.OnAttacked(new AttackInfo(owner, AttackType.Heal, value,Color.yellow));

                            //Debug.Log("不死鸟回血"+value);
                            lastPhoenixTime = Time.time;
                        }
                       
                    }
                    
                }
            }
        }
        
        

        var sourChain = GetWeaponLevelByNbt("魂链");
        var sourChainIgnoreDamageType = new List<AttackType>() {AttackType.Reflect,AttackType.Heal,AttackType.Poison,AttackType.Real,AttackType.Thunder};
        if (sourChain > 0 && !sourChainIgnoreDamageType.Contains(attackInfo.attackType))
        {
            for (int i = 0; i < summons.Count; i++)
            {
                if (summons[i] != null && summons[i].IsAlive())
                {
                    var value = attackInfo.value < sourChain ? attackInfo.value : sourChain;
                    summons[i].OnAttacked(new AttackInfo(attackInfo.attacker, attackInfo.attackType, value));
                    FlyText.Instance.ShowDamageText(summons[i].transform.position-Vector3.up*3,"魂链("+value+")");
                    attackInfo.value -= value;
                    if (attackInfo.value <= 0)
                    {
                        attackInfo.value = 0;
                        break;
                    }
                        
                }
            }
        }

        var parryLevel = GetWeaponLevelByNbt("格挡");
        var ignoreDamageType2 = new List<AttackType>() {AttackType.Heal};
        if (parryLevel > 0 && !ignoreDamageType2.Contains(attackInfo.attackType))
        {
            if ((float)endurance/maxEndurance<0.25f || endurance < 1  )
            {
                return attackInfo;
            }

            var realDamage = attackInfo.value;
            
           
            float parryRate = (float)endurance / maxEndurance;
            attackInfo.value = (int)(attackInfo.value* (1-parryRate));
            

            if (attackInfo.value < 0)
                attackInfo.value = 0;

            var realCost = realDamage - attackInfo.value;//实际伤害-减免后的伤害=此次格挡的伤害值

            var parryValue = realCost;
            realCost = (int) (realCost * (1 - (float) parryLevel / (parryLevel + 30)));
            
            if (endurance - realCost <= 0)//消耗过大，只消耗endurance的耐久
            {
                realCost = endurance;
            }
            
            if (realCost < 0)
                realCost = 0;
            
            
            
            AddEndurance((int) (-1* realCost ));
            //Debug.Log("减伤"+parryValue+"消耗耐久"+realCost);
            
        }

        

        
        return attackInfo;
    }

    public virtual int GetWeaponLevelByNbt(string key)
    {
        EnhancementLevel ret = null;
        ret = weaponNbt.enhancementLevels.Find(x => x.enhancementName == key);
        if (ret == null)
            return 0;
        else
        {
            return ret.level;
        }
    }

    public void SetWeaponLevel(string key, int level)
    {
        EnhancementLevel ret = null;
        ret = weaponNbt.enhancementLevels.Find(x => x.enhancementName == key);
        if (ret == null)
        {
            weaponNbt.enhancementLevels.Add(new EnhancementLevel(key,level));
            
            return;
        }
        else
        {
            ret.level = level;
        }
    }


    
    private void OnDisable()
    {
        
        //清除所有事件绑定
        owner.onAttacked -= OnAttacked;

        owner.onSlainOther -= OnSlainOther;
        owner.onAttackOther -= OnAttackOther ;
        owner.onBeforeAttacked -= OnBeforeAttacked;
        owner.onDie -= OnOwnerDie;
        
        if(summonPointUi)
            Destroy(summonPointUi.gameObject);
        try//可能之前没有噬魂附魔，因此可能移除不掉导致报错，忽略这次报错
        {
            if(addedSourCatch)
                EventCenter.RemoveListener<McUnit>(EnumEventType.OnMcUnitDied, OnMcUnitDie);
            if (addedJudgeEvent)
            {
                EventCenter.RemoveListener<IAttackAble,IVictimAble>(EnumEventType.OnUnitDamageOther, Judge);
                Debug.LogError("移除审判实践");
            }
                
        }
        catch (Exception e)
        {
            Debug.LogError("handWeapon移除事件异常"+e.Message);
        }
    }
}
