using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McFightingManager : FightingManager
{
    public MCPosManager mcPosManager;

    protected void Start()
    {
        base.Start();
        mcPosManager = MCPosManager.Instance;
    }


    public override void OnJoinPlayingDanMuReceived(Player toJoinPlayer)
    {
        base.OnJoinPlayingDanMuReceived(toJoinPlayer);
        var newPlayer1 = toJoinPlayer;

        JoinGame(newPlayer1);

        PlanetCommander commander = null;

        commander = new SteveCommander(newPlayer1.uid, newPlayer1,
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

    protected override void SetOwnersAfter1Second()
    {
        base.SetOwnersAfter1Second();

        for (int i = 0; i < players.Count; i++)
        {
            PlanetCommander commander = null;

            commander = new SteveCommander(players[i].uid, players[i], colorTable.colors[i]);


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


        if (gameMode == GameMode.MCWar)
        {
            leftPlanet.enemyPlanets.Add(rightPlanet);
            rightPlanet.enemyPlanets.Add(leftPlanet);
        }
    }
}