using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpFill;
    public Vector3 offset=Vector3.up;

    public Transform skillUiGroup;
    private GameEntity owner;

    private Camera mainCamera;
    private void Awake()
    {
        mainCamera=Camera.main;
    }

    public void OnHpChanged(int hp,int maxHP)
    {
       UpdateHp(hp,maxHP);
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

    private void LateUpdate()
    {
        UpDatePos();
    }

    public void Init(GameEntity gameEntity)
    {
        this.owner = gameEntity;
        gameEntity.onHpChanged += OnHpChanged;
        this.offset = gameEntity.hpUIOffse;
        this.transform.localScale = gameEntity.hpUIScale;
    }

    public void OnAddSkill(SkillBase skillBase)
    {
        
    }
    
    
}
