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

    public void AddSkill(SkillItemUI skillItemUi)
    {
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
