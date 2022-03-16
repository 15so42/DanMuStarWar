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
    
    public virtual bool AddSkillCheck(string skillName)
    {
        var skill = skills.Find(x => x.skillName == skillName);
        if (skill)
        {
            skill.ResetTimer();
            //添加同名技能时 重置对应技能冷却时间
            return false;

        }

        return true;
    }

    //传入skillItem，生成技能之后对UI进行初始化
    
    public void AddSkill(SkillItemUI skillItemUi)
    {
        
        skills.Add(skillItemUi.skillBase);
        skillItemUi.skillBase.skillItemUi = skillItemUi;
        //UI处理
        var trans = skillItemUi.transform;
        if (simpleSkillUI)
        {
            trans.SetParent(gameEntity.hpUI.skillUiGroup);
            trans.gameObject.SetActive(false);
            skillItemUi.transform.localScale=new Vector3(1,1,1);
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
                    skills[i].SetItemIndex(i);
                }
            }
            //删除无用技能，对技能重新排序
            SortSkill();
        
        
    }

    public void SortSkill()
    {
        
    }
}
