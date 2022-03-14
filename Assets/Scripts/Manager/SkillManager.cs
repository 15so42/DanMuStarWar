﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

//此类没有先后顺序影响，直接单例
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    [Header("出场自带技能")]
    public List<SkillBase> initSkill=new List<SkillBase>();
    public List<SkillBase> allSkill=new List<SkillBase>();
    public List<BuffBase> allBuff=new List<BuffBase>();

    public GameObject skillItemUiPfb;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (target.AddSkillCheck(skillName)==false)//不能添加重复的，如果已经有了则刷新持续时间然后返回false
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
}
