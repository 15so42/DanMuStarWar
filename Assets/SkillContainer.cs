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
        return true;
    }

    public bool UseSkill(int index,PlanetCommander planetCommander)
    {
        // if (planetCommander != skills[index].planetCommander)
        // {
        //     gameEntity.LogTip("只有购买者可以使用此卡牌");
        //     return false;
        // }
        ErrorCode errCode = null;
        skills[index].useCommander = planetCommander;//技能以使用者为主人
        if(skills[index].passive==false)//主动技能
            errCode=skills[index].PlayCheck();
        else//被动技能
        {
            return false;
        }
        
        if(errCode!=null && errCode.code!=ErrorType.Success)
            return false;

        
        return true;

    }

    public void ChangeSkill(int index,PlanetCommander planetCommander)
    {
        skills[index].Kill();
        int techLv = (gameEntity as Planet).GetTechLevelByRes();
        AddRandomSkill(techLv,planetCommander);
        
    }

    public void ShowSkillDesc()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skillItemUi.ShowSkillDesc();
        }
    }
  

    public void AddRandomSkill(int techLevel,PlanetCommander planetCommander)
    {
        var skillName = SkillManager.Instance.GetRandomSkillByTech(techLevel,skills).skillName;
        SkillManager.Instance.AddSkill(skillName,gameEntity,planetCommander);
    }

    public bool BuySkill(int index,GameMode gameMode,PlanetCommander planetCommander)
    {
        var techLevel = 1;
        if (gameMode == GameMode.Normal)
            techLevel = (gameEntity as Planet).GetTechLevelByRes();
        //程序index
        return SkillManager.Instance.BuySkill(index,gameEntity,techLevel ,planetCommander);
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

    public void RemoveSkill(int index)
    {
        skills[index].Kill();
        
        skills.RemoveAt(index);
    }
}
