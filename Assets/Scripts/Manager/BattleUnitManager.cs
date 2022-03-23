using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{
    public static BattleUnitManager Instance;
    
    public List<BattleUnit> allBattleUnits=new List<BattleUnit>();

    private void Awake()
    {
        Instance = this;
        EventCenter.AddListener<BattleUnit>(EnumEventType.OnBattleUnitCreated,OnBattleUnitCreated);
    }

    void OnBattleUnitCreated(BattleUnit battleUnit)
    {
        allBattleUnits.Add(battleUnit);
    }
}
