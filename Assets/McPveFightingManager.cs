using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class McPveFightingManager : FightingManager
{
    
   
   
    
    
    public override void OnJoinPlayingDanMuReceived(Player toJoinPlayer)
    {
        base.OnJoinPlayingDanMuReceived(toJoinPlayer);
        var newPlayer1 = toJoinPlayer;

        JoinGame(newPlayer1);

        SteveCommander commander = null;

        commander = new SteveCommander(newPlayer1.uid, newPlayer1,
            colorTable.colors[players.Count]);

        commander.respawnTimRate = 0.6f;

        BiliUserInfoQuerier.Instance.Query(newPlayer1.uid, newPlayer1);

      
        PlanetManager.Instance.allPlanets[firstPlanetIndex]
            .AddCommander(commander,  0 );
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
            SteveCommander commander = null;

            commander = new SteveCommander(players[i].uid, players[i], colorTable.colors[i]);

            commander.respawnTimRate *= 0.6f;

            //var index = ((planetNum / playersCount) * i) % planetNum;

            PlanetManager.Instance.allPlanets[firstPlanetIndex].AddCommander(commander, 0);
            if (i == 0)
                PlanetManager.Instance.allPlanets[firstPlanetIndex].SetOwner(players[i], commander);

        }
    }
    
    
    
    

   
}
