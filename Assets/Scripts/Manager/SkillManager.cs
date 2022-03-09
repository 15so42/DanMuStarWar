using System;
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

    public void AddSkill(string skillName, GameEntity target)
    {
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
}
