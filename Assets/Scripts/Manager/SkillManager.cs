using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[System.Serializable]
public class ShopSkillPair
{
    public int skillId;
    public string skillName;
    public SkillBase lv1;
    public SkillBase lv2;
    public SkillBase lv3;
    public SkillBase lv4;
}

//此类没有先后顺序影响，直接单例
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    [Header("可购买的技能")]
    public List<ShopSkillPair> shopSkillPairs=new List<ShopSkillPair>();
    
    [Header("出场自带技能")]
    public List<SkillBase> initSkill=new List<SkillBase>();
    [Header("特殊技能（比如加速)")]
    public List<SkillBase> specialSkill=new List<SkillBase>();
    
    public List<SkillBase> allSkill=new List<SkillBase>();
    public List<BuffBase> allBuff=new List<BuffBase>();
    
    public List<SkillBase> lv1Skill=new List<SkillBase>();
    public List<SkillBase> lv2Skill=new List<SkillBase>();
    public List<SkillBase> lv3SKill = new List<SkillBase>();
    public List<SkillBase> lv4Skill = new List<SkillBase>();

    public GameObject skillItemUiPfb;
    private void Awake()
    {
        Instance = this;
        lv2Skill = lv2Skill.Concat(lv1Skill).ToList();
        lv3SKill = lv3SKill.Concat(lv2Skill).ToList();
        lv4Skill = lv4Skill.Concat(lv3SKill).ToList();

        allSkill = allSkill.Concat(lv4Skill).ToList();

        allSkill = allSkill.Concat(specialSkill).ToList();


        // for (int i = 0; i < shopSkillPairs.Count; i++)
        // {
        //     allSkill.Add(shopSkillPairs[i].lv1);
        //     allSkill.Add(shopSkillPairs[i].lv2);
        //     allSkill.Add(shopSkillPairs[i].lv3);
        //     allSkill.Add(shopSkillPairs[i].lv4);
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

    public bool BuySkill(int index,GameEntity target,int techLevel)
    {
        if (index < 0 || index >= shopSkillPairs.Count)//从1开始输到4
        {
            target.LogTip("序号错误");
            return false;
        }
        
        var skillBase = shopSkillPairs[index].lv1;
        if(techLevel==2)
            skillBase = shopSkillPairs[index].lv2;
        if(techLevel==3)
            skillBase = shopSkillPairs[index].lv3;
        if(techLevel==4)
            skillBase = shopSkillPairs[index].lv4;

      
        var planet = target as Planet;
        if (planet)
        {
            if (planet.planetResContainer.GetResNumByType(ResourceType.DicePoint) < 1 + skillBase.usePoint)
            {
                target.LogTip("购买所需点数不够");
                return false;
            }
            var skillName = skillBase.skillName;
            AddSkill(skillName,target);
            (target as Planet)?.UseSkill(target.skillContainer.skills.Count);
            return true;
        }

        return false;

    }

    public void AddBuff(string buffName, GameEntity target)
    {
        var skill = GetBuffInstance(buffName);
        skill.Init(target);
        var skillItemUi = GameObject.Instantiate(skillItemUiPfb).GetComponent<SkillItemUI>();
        skillItemUi.Init(skill);
        target.AddSkill(skillItemUi);
    }
    public void AddSkill(string skillName, GameEntity target)
    {
        if (target.AddSkillCheck(skillName)==false)//可以添加重复的
        {
            return;
        }
        var skill = GetSkillInstance(skillName);
        skill.Init(target);
        var skillItemUi = GameObject.Instantiate(skillItemUiPfb).GetComponent<SkillItemUI>();
        skillItemUi.Init(skill);
        target.AddSkill(skillItemUi);
        
    }

    public SkillBase GetSkillInstance(string skillName)
    {
        var skill = allSkill.Find(x => x.name == skillName);
        return Object.Instantiate(skill);
    }

    public BuffBase GetBuffInstance(string buffName)
    {
        var buff = allBuff.Find(x => x.name == buffName);
        return Object.Instantiate(buff);
    }

    
    public List<SkillBase> GetSkillListByTech(int techLevel)
    {
        if (techLevel == 1)
            return lv1Skill;
        if (techLevel == 2)
            return lv2Skill;
        if (techLevel == 3)
            return lv3SKill;
        if (techLevel == 4)
            return lv4Skill;
        
        
        if (techLevel < 1)
            return lv1Skill;
        
        return lv4Skill;
    }
    public SkillBase GetRandomSkillByTech(int techLevel,List<SkillBase> skills)
    {
        var skillList = GetSkillListByTech(techLevel);
        var tSkill=skillList[Random.Range(0, skillList.Count)];
        var curSkill = skills.Find(x => x.skillName == tSkill.skillName);//找到同名技能
        while (curSkill!=null && tSkill.passive )//找到的是同名技能并且是被动技能，重找
        {
            tSkill=skillList[Random.Range(0, skillList.Count)];
        }

        return tSkill;
    }
}
