using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarWarFightingManager : FightingManager
{
    public SkillBase shieldSkillBase;


    public override void OnJoinPlayingDanMuReceived(Player toJoinPlayer)
    {
        base.OnJoinPlayingDanMuReceived(toJoinPlayer);


        if (gameMode == GameMode.Normal)//混战模式照可加入星球进行加入
        {
            for (int i = 0; i < PlanetManager.Instance.allPlanets.Count; i++)
            {
                var planet = PlanetManager.Instance.allPlanets[i];
                if (planet.owner == null && Math.Abs(planet.colonyPoint) < 1 &&
                    planet.occupied == false && planet.die == false) //没有玩家且没有被占领
                {
                
                    JoinGame(toJoinPlayer);
                    BiliUserInfoQuerier.Instance.Query(toJoinPlayer.uid, toJoinPlayer);
                    planet.SetOwner(toJoinPlayer, null);
                    planet.maxSkillCount = 2;


                    break;
                }
            }
        }
        else //团战模式，加入人少的一方
        {
            var newPlayer1 = toJoinPlayer;

            JoinGame(newPlayer1);

            PlanetCommander commander = null;

            commander = new PlanetCommander(newPlayer1.uid, newPlayer1,
                colorTable.colors[players.Count]);


            BiliUserInfoQuerier.Instance.Query(newPlayer1.uid, newPlayer1);

            var firstCount = PlanetManager.Instance.allPlanets[firstPlanetIndex].planetCommanders
                .Count;
            var lastCount = PlanetManager.Instance.allPlanets[lastPlanetIndex].planetCommanders
                .Count;
            var lessPlanet = firstCount > lastCount ? lastPlanetIndex : firstPlanetIndex;

            PlanetManager.Instance.allPlanets[lessPlanet]
                .AddCommander(commander, lessPlanet == firstPlanetIndex ? 0 : 1);
            if (exitPlayers.Contains(commander.player.uid) == false)
            {
                commander.AddPoint(roundManager.elapsedTime / 20);
            }
        }

    }

    protected override void SetOwnersAfter1Second()
    {
        base.SetOwnersAfter1Second();
        if (gameMode == GameMode.BattleGround)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PlanetCommander commander = null;

                commander = new PlanetCommander(players[i].uid, players[i], colorTable.colors[i]);


                //var index = ((planetNum / playersCount) * i) % planetNum;
                if (i % 2 == 0)
                {
                    PlanetManager.Instance.allPlanets[firstPlanetIndex].AddCommander(commander, 0);
                    if (i == 0)
                        PlanetManager.Instance.allPlanets[firstPlanetIndex].SetOwner(players[i], commander);
                }
                else
                {
                    PlanetManager.Instance.allPlanets[lastPlanetIndex].AddCommander(commander, 1);
                    if (i == lastPlanetIndex)
                        PlanetManager.Instance.allPlanets[lastPlanetIndex].SetOwner(players[i], commander);
                }
            }

            var leftPlanet = PlanetManager.Instance.allPlanets[firstPlanetIndex];
            var rightPlanet = PlanetManager.Instance.allPlanets[lastPlanetIndex];

            //PlanetManager.Instance.allPlanets[firstPlanetIndex].SetOwner(new Player(23477,"混沌","",""));
            //PlanetManager.Instance.allPlanets[lastPlanetIndex].SetOwner(new Player(765642,"秩序","",""));

            //设置护盾星球
            if (PlanetManager.Instance.allPlanets.Count > 10)
            {
                SkillManager.Instance.AddSkill(shieldSkillBase.skillName, PlanetManager.Instance.allPlanets[12], null);
                PlanetManager.Instance.allPlanets[12].needRingPoint = 300;
                SkillManager.Instance.AddSkill(shieldSkillBase.skillName, PlanetManager.Instance.allPlanets[13], null);
                PlanetManager.Instance.allPlanets[13].needRingPoint = 300;
            }
        }
        else
        {
            var planetNum = FightingManager.Instance.maxPlayerCount;
            var playersCount = players.Count;
            //玩家依次占领星球P
            for (int i = 0; i < players.Count; i++)
            {
                var index = ((planetNum / playersCount) * i) % planetNum;
                PlanetManager.Instance.allPlanets[index].SetOwner(players[i], null);
                PlanetManager.Instance.allPlanets[index].maxSkillCount = 2;
            }

            //设置护盾星球
            SkillManager.Instance.AddSkill(shieldSkillBase.skillName, PlanetManager.Instance.allPlanets[12], null);
            PlanetManager.Instance.allPlanets[12].needRingPoint = 300;
        }
    }
}
