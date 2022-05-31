using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangeWeapon : BowWeapon
{
    public bool back = true;

    public override bool FireCheck()
    {
        if (back == false)
            return false;
        return base.FireCheck();
    }

    public override void Fire()
    {
        base.Fire();
        OnFlyOut();
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
