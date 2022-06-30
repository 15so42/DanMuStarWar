using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleFightingManager : FightingManager
{
    
     public override void OnJoinPlayingDanMuReceived(Player toJoinPlayer)
    {
        base.OnJoinPlayingDanMuReceived(toJoinPlayer);
        var newPlayer1 = toJoinPlayer;

        JoinGame(newPlayer1);

        PlanetCommander commander = null;

        commander = new SteveCommander(newPlayer1.uid, newPlayer1,
            colorTable.colors[players.Count]);


        BiliUserInfoQuerier.Instance.Query(newPlayer1.uid, newPlayer1);

        int min = 9999;
        var lessPlanet = PlanetManager.Instance.allPlanets[0];
        foreach (var planet in PlanetManager.Instance.allPlanets)
        {
            if(planet.die)
                continue;
            var count = planet.planetCommanders.Count;
            if (count < min)
            {
                min = count;
                lessPlanet = planet;
            }
        }

        
        lessPlanet.AddCommander(commander,0);
        if (exitPlayers.Contains(commander.player.uid) == false)
        {
            commander.AddPoint(roundManager.elapsedTime / 20);
        }
    }

    protected override void SetOwnersAfter1Second()
    {
        base.SetOwnersAfter1Second();

        var planetCount = PlanetManager.Instance.allPlanets.Count;

        for (int i = 0; i < players.Count; i++)
        {
            PlanetCommander commander = null;

            commander = new SteveCommander(players[i].uid, players[i], colorTable.colors[i]);

            
            PlanetManager.Instance.allPlanets[i%planetCount].AddCommander(commander,0);
            if (i < planetCount)
            {
                PlanetManager.Instance.allPlanets[i].SetOwner(players[i],commander);
            }
            
        }

        //互相设置为敌人
        foreach (var planet in PlanetManager.Instance.allPlanets)
        {
            foreach (var ePlanet in PlanetManager.Instance.allPlanets)
            {
                if (ePlanet != planet && planet.enemyPlanets.Contains(ePlanet)==false)
                {
                    planet.enemyPlanets.Add(ePlanet);
                }
            }
        }


       
    }
}
