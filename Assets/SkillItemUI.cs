﻿using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillItemUI : MonoBehaviour
{
    public Image icon;

    public Image fill;
    public TMP_Text lifeCount;

    public SkillBase skillBase;
    

    public void Init(SkillBase skillBase)
    {
        this.skillBase = skillBase;
        icon.sprite = skillBase.icon;
        lifeCount.text = this.skillBase.life+"";
        fill.fillAmount = 1;
        
        skillBase.onFinished += OnFinished;//技能完毕事件，技能使用完后对应Ui自毁
        skillBase.onLifeChangedAction += OnLifeChanged;
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
