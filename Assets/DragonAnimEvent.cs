using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimEvent : MonoBehaviour
{
    public EnderDragon dragon;


    public void MeleeAttack()
    {
        dragon.MeleeAttack();
    }
    public void ImpactFx()
    {
        dragon.ImpactFx();
    }
}
