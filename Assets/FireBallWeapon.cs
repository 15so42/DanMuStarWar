using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallWeapon : BowWeapon
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

 

    

    public override int GetWeaponLevelByNbt(string key)
    {
        if (key == "火焰")
        {
            return 5;
        }
        return base.GetWeaponLevelByNbt(key);
    }
}
