using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCode.Tools;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "GameEnvEvent/SoldierLine", fileName = "SoldierLine",order=0)]
public class SoldierLine : GameEnvEvent
{
    
    public override void Run(int level)
    {
        var fightingManager = FightingManager.Instance;
        if(fightingManager.gameStatus!=GameStatus.Playing)
            return;
        var planets = PlanetManager.Instance.allPlanets;
        var firstPlanet = planets[0];
        var secondPlanet = planets[1];
        var summonList = new List<string>()
        {
            "BattleUnit_Zombie","BattleUnit_Skeleton","BattleUnit_Creeper",
            "BattleUnit_Blaze"
        };
        var count = UnityEngine.Random.Range(2, 5);
        var soldierName = summonList[UnityEngine.Random.Range(0, summonList.Count)];
        for (int i = 0; i <count ; i++)
        {
            firstPlanet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,soldierName,2, (go) =>
            {
                UnityTimer.Timer.Register(1, () =>
                {
                    if (go)
                    {
                        var mcUnit = go.GetComponent<McUnit>();
                        if (mcUnit)
                        {
                            mcUnit.GoMCWorldPos(secondPlanet.transform.position, false);
                            var diff=fightingManager.roundManager.elapsedTime/60;
                            for (int j = 0; j < diff; j++)
                            {
                                mcUnit.GetActiveWeapon().RandomSpellBySpellCount();
                            }

                            mcUnit.canSetPlanetEnemy = true;
                        }
                        
                    }
                });

            }),null));
            
            secondPlanet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,soldierName,2, (go) =>
            {
                UnityTimer.Timer.Register(1, () =>
                {
                    if (go)
                    {
                        var mcUnit = go.GetComponent<McUnit>();
                        if (mcUnit)
                        {
                            mcUnit.GoMCWorldPos(firstPlanet.transform.position, false);
                            var diff=fightingManager.roundManager.elapsedTime/60;
                            for (int j = 0; j < diff; j++)
                            {
                                mcUnit.GetActiveWeapon().RandomSpellBySpellCount();
                            }
                            mcUnit.canSetPlanetEnemy = true;
                        }
                        
                    }
                });
            }),null));
        }
    }

    
}
