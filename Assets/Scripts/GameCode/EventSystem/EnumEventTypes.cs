using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumEventType
{
    OnDanMuReceived,
    OnGiftReceived,
    OnPlanetCreated,
    OnBattleUnitCreated,
    OnPlayerJoined,
    OnBattleStart,
    OnPlanetsSpawned,
    OnPlanetDie,
    OnStartWaitingJoin,//开始等待加入，此时所有隐藏单位销毁自己
    OnPlanetOccupied,//星球被驻守成功
    OnColonyLost,//驻守地失守
    OnSteveDied,
    OnMcUnitDied,
    OnBattleOver,

    //送礼
    OnMcBatteryReceived,
    
    //PVE模式
    OnMonsterInit,
    //MonsterSpawner生成
    OnMonsterSpawnerInit,
}

