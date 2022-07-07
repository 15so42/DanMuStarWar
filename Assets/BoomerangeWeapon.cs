using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class BoomerangeWeapon : BowWeapon
{
    public bool back = true;

    private float leftTimer = 0;

    [Header("大于几秒后强制收回")] public bool backByWeapon = true;
    public float maxLeftTimer = 6;
    
    public override bool FireCheck()
    {
        if (back == false)
            return false;
        else
        {
            return base.FireCheck();
        }
        //return base.FireCheck();
    }

    public override void Fire()
    {
        base.Fire();
        OnFlyOut();
    }

    protected override void Update()
    {
        base.Update();
        if(!backByWeapon)
            return;
        if (!back)
        {
            leftTimer += Time.deltaTime;
            if (leftTimer > maxLeftTimer)
            {
                back = true;
                leftTimer = 0;
            }
        }
        else
        {
            leftTimer = 0;
        }
    }

    public void OnFlyOut()
    {
        back = false;
    }
    
    
}
