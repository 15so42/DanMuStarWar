using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnderDragon : McUnit
{
    protected override void Start()
    {
        
        base.Start();
        var liveWeapon = GetComponentInChildren<HandWeapon>();
        liveWeapon.Init(this);
        liveWeapon.randomStrs.Remove("烈阳");
        canPushBack = false;
        
        onHpChanged += UpdateHpUIByNameText;
        UpdateHpUIByNameText(props.hp,props.maxHp,props.shield,props.maxShield);
    }
    
    void UpdateHpUIByNameText(int hp,int maxHp,int shield,int maxShield)
    {
        hpUI.SetNameText(hp+"/"+maxHp);
    }
    
    private void OnDisable()
    {
        onHpChanged -= UpdateHpUIByNameText;
    }
}
