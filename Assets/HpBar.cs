using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class HpBar : MonoBehaviour
{
    public Image hpBg;
    public Image hpFill;
    public Vector3 offset=Vector3.up;

    public Transform skillUiGroup;
    private GameEntity owner;

    public Text tipText;

    public UnityTimer.Timer logTimer;
    private Camera mainCamera;
    private void Awake()
    {
        mainCamera=Camera.main;
        tipText.gameObject.SetActive(false);
    }

    public void OnHpChanged(int hp,int maxHP)
    {
       UpdateHp(hp,maxHP);
    }

    private void OnDestroy()
    {
        owner.onHpChanged =null;
        
    }

    void UpdateHp(int hp,int maxHP)
    {
        hpFill.fillAmount = (float)hp / maxHP;
        
    }

    void UpDatePos()
    {
        if(owner)
            transform.position = mainCamera.WorldToScreenPoint(owner.transform.position)+offset;
    }

    public void SetColor(Color color)
    {
        hpFill.color = color;
    }

    private void LateUpdate()
    {
        UpDatePos();
    }

    public void Init(GameEntity gameEntity)
    {
        this.owner = gameEntity;
        gameEntity.onHpChanged += OnHpChanged;
        this.offset = gameEntity.hpUIOffse;
        //this.transform.localScale = gameEntity.hpUIScale;
        var rect = hpBg.GetComponent<RectTransform>();
        var width = rect.rect.width;
        var height = rect.rect.height;
        hpBg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width* gameEntity.hpUIScale.x );
        hpBg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,height* gameEntity.hpUIScale.y );
        
      
        //this.hpFill.color = color;
    }

    public void OnAddSkill(SkillBase skillBase)
    {
        
    }

    public void LogTip(string msg)
    {
        tipText.gameObject.SetActive(true);
        tipText.text = msg;
        logTimer?.Cancel();
        logTimer = UnityTimer.Timer.Register(3, () =>
        {
            tipText.gameObject.SetActive(false);
        });
    }
    
    
}
