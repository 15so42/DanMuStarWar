using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
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

    public Text usePointText;
    public Text removePointText;

    public SkillBase skillBase;

    private UnityTimer.Timer showDescTimer;
    public void Init(SkillBase skillBase)
    {
        this.skillName.text = skillBase.skillName+"";
        this.skillBase = skillBase;
        icon.sprite = skillBase.icon;
        lifeCount.text = this.skillBase.life+"";
        fill.fillAmount = 1;
        usePointText.text = skillBase.usePoint + "";
        removePointText.text = skillBase.removePoint + "";
        
        skillBase.onFinished += OnFinished;//技能完毕事件，技能使用完后对应Ui自毁
        skillBase.onLifeChangedAction += OnLifeChanged;
    }

    public void ShowSkillDesc()
    {
        var lastText = this.skillName.text;//记录之前的文字
        this.skillName.text = this.skillBase.desc;
        showDescTimer=UnityTimer.Timer.Register(5, () =>
        {
            this.skillName.text = lastText;
        });
    }

    public void UpdateIndex(int index)
    {
        skillIndex.text = index+1 + "";
    }

    private void Update()
    {
        this.fill.fillAmount = skillBase.GetLeftCdRatio();
    }

    
  

    void OnFinished()
    {
        if(gameObject)
            Destroy(gameObject);
        
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    void OnLifeChanged(int life)
    {
        lifeCount.text = life.ToString();
    }

    private void OnDisable()
    {
        showDescTimer?.Cancel();
        skillBase.onFinished -= OnFinished;//技能完毕事件，技能使用完后对应Ui自毁
    }
}
