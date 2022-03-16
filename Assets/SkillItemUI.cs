using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillItemUI : MonoBehaviour
{
    public Text skillName;
    public Text skillIndex;
    
    public Image icon;

    public Image fill;
    public TMP_Text lifeCount;

    public SkillBase skillBase;
    

    public void Init(SkillBase skillBase)
    {
        this.skillName.text = skillBase.skillName+"";
        this.skillBase = skillBase;
        icon.sprite = skillBase.icon;
        lifeCount.text = this.skillBase.life+"";
        fill.fillAmount = 1;
        
        skillBase.onFinished += OnFinished;//技能完毕事件，技能使用完后对应Ui自毁
        skillBase.onLifeChangedAction += OnLifeChanged;
    }

    public void UpdateIndex(int index)
    {
        skillIndex.text = index + "";
    }

    private void Update()
    {
        this.fill.fillAmount = skillBase.GetLeftCdRatio();
    }

    void OnFinished()
    {
        Destroy(gameObject);
        
    }

    void OnLifeChanged(int life)
    {
        lifeCount.text = life.ToString();
    }
    
}
