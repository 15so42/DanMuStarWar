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
    public Image shieldFill;
    public Vector3 offset=Vector3.up;
    //血条刻度线
    [Header("血条刻度线")]
    public Transform hpTile;

    

    public Transform tileLine;

    public Transform skillUiGroup;
    private GameEntity owner;

    public Text tipText;

    [Header("显示名字（Mc模式)")] public Text nameText;
    [Header("武器名称")] public Text weaponNameText;
    public UnityTimer.Timer logTimer;
    private Camera mainCamera;
    private void Awake()
    {
        mainCamera=Camera.main;
        tipText.gameObject.SetActive(false);
        hpTile.gameObject.SetActive(false);
        weaponNameText.gameObject.SetActive(false);
    }

    public void OnHpChanged(int hp,int maxHP,int shield,int maxShield)
    {
       UpdateHp(hp,maxHP,shield,maxShield);
    }

    private void OnDestroy()
    {
        owner.onHpChanged =null;
        
    }

    public void OpenHPTile()
    {
        hpTile.gameObject.SetActive(true);
        UpdateHp(owner.props.hp,owner.props.maxHp,owner.props.shield,owner.props.maxShield);
    }
    public void SetNameText(string name)
    {
        nameText.text = name + "";
    }

    public void SetWeaponText(string weaponName)
    {
        weaponNameText.gameObject.SetActive(true);
        weaponNameText.text = "[" + weaponName + "]";
    }

    void UpdateHp(int hp,int maxHP,int shield,int maxShield)
    {
        // if (hp + shield < maxHP)
        // {
        //     hpFill.fillAmount = (float)hp / maxHP;
        //     shieldFill.fillAmount = (float) (hp + shield) / maxHP;
        // }
        // else
        // {
        //     hpFill.fillAmount = (float)hp / (maxHP+shield);
        //     shieldFill.fillAmount = 1;
        // }
        
        hpFill.fillAmount=hpFill.fillAmount = (float)hp / maxHP;
        shieldFill.fillAmount = (float) shield / maxHP;

        //hpTile.pixelsPerUnitMultiplier =maxHP/160f;
        var targetCount = maxHP / 5;
        var childCount = hpTile.childCount;
        var sub = targetCount - childCount;
        if (sub > 0)
        {
            for (int i = 0; i < sub; i++)
            {
                GameObject.Instantiate(tileLine, hpTile);
            }
        }
    }

    void UpDatePos()
    {
        if(owner)
            transform.position = mainCamera.WorldToScreenPoint(owner.transform.position)+offset;
    }

    public void SetColor(Color color)
    {
        hpFill.color = color;
        //shieldFill.color=new Color(1-color.r,1-color.g,1-color.b);
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
        if(gameObject==null)
            return;
        tipText.gameObject.SetActive(true);
        tipText.text = msg;
        logTimer?.Cancel();
        logTimer = UnityTimer.Timer.Register(6, () =>
        {
            if(tipText==null)
                return;
            tipText.gameObject.SetActive(false);
        });
    }
    
    
}
