using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{
    public List<BattleUnit> allBattleUnits=new List<BattleUnit>();

    void OnBattleUnitCreate(BattleUnit battleUnit)
    {
        allBattleUnits.Add(battleUnit);
    }
}
