using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCanvas : MonoBehaviour
{
    public GameObject hpBarPfb;
    public Transform hpBarParent;
    public HpBar CreateHpBar(BattleUnit battleUnit)
    {
        return  GameObject.Instantiate(hpBarPfb, hpBarParent).GetComponent<HpBar>();
        
    }
}
