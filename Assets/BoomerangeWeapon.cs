using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class BoomerangeWeapon : BowWeapon
{
    public bool back = true;

    private float leftTimer = 0;
    
    public override bool FireCheck()
    {
        if (back == false)
            return false;
        else
        {
            return true;
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
        if (!back)
        {
            leftTimer += Time.deltaTime;
            if (leftTimer > 6)
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
    public void OnBoomerangeBack()
    {
        back = true;
    }
    
}
