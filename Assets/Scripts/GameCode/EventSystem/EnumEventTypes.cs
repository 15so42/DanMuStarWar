using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumEventType
{
    OnDanMuReceived,
    OnPlanetCreated,
    OnBattleUnitCreated,
    OnPlayerJoined,
    OnBattleStart,
    OnPlanetsSpawned,
    OnPlanetDie,
    OnStartWaitingJoin,//开始等待加入，此时所有隐藏单位销毁自己
}

