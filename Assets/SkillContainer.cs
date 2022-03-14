using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    public List<SkillBase> skills=new List<SkillBase>();

    public bool simpleSkillUI = false;
    
    public GameEntity gameEntity;
    public void Init(GameEntity gameEntity)
    {
        this.gameEntity = gameEntity;
        
    }

    //传入skillItem，生成技能之后对UI进行初始化
    
    public void AddSkill(SkillItemUI skillItemUi)
    {
        var skill = skills.Find(x => x.skillName == skillItemUi.skillBase.skillName);
        if (skill)
        {
            skill.ResetTimer();
            //添加同名技能时 重置对应技能冷却时间
            return ;
            
        }
        skills.Add(skillItemUi.skillBase);
        //UI处理
        var trans = skillItemUi.transform;
        if (simpleSkillUI)
        {
            trans.SetParent(gameEntity.hpUI.skillUiGroup);
            skillItemUi.transform.localScale=Vector3.one* 0.5f;
            skillItemUi.lifeCount.enabled = false;
        }
        else
        {
            trans.SetParent((gameEntity as Planet).planetUi.skillGroupUI);
            trans.localScale=Vector3.one;
            
        }
    }
    
    private void Update()
    {
       
            for (int i = 0; i < skills.Count; i++)
            {
                if (!skills[i] || skills[i].finished)
                {
                    skills.RemoveAt(i);
                    i--;
                }
                else
                {
                    skills[i].Update();
                }
            }
        
        
    }
}
