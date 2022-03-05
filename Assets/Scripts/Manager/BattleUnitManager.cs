using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{
    public List<BattleUnit> allBattleUnits=new List<BattleUnit>();

    private void Awake()
    {
        EventCenter.AddListener<BattleUnit>(EnumEventType.OnBattleUnitCreated,OnBattleUnitCreated);
    }

    void OnBattleUnitCreated(BattleUnit battleUnit)
    {
        allBattleUnits.Add(battleUnit);
    }
}
