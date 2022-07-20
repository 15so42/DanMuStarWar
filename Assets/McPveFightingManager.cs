using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class McPveFightingManager : FightingManager
{
    private string nextMap = "PVE";

    public string GetSceneNameByChinese(string chinese)
    {
        if (chinese == "村庄")
        {
            return "McWarScene_Village";
        }

        if (chinese == "矿井")
        {
            return "McWarScene_Mine";
        }

        if (chinese == "PVE")
        {
            
            
            return "McWarScene_Guard";
        }

        return "McWarScene_Village";
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
            PlanetCommander commander = null;

            commander = new SteveCommander(players[i].uid, players[i], colorTable.colors[i]);


            //var index = ((planetNum / playersCount) * i) % planetNum;

            PlanetManager.Instance.allPlanets[firstPlanetIndex].AddCommander(commander, 0);
            if (i == 0)
                PlanetManager.Instance.allPlanets[firstPlanetIndex].SetOwner(players[i], commander);

        }
    }
    
    protected override void OnGameOver()
    {
        base.OnGameOver();
        
        StartCoroutine(NewMap());
    }
    
    IEnumerator NewMap()
    {
        var mainMap = SceneManager.GetSceneAt(0).name;
        if (nextMap == "PVE")
        {
            yield break;
        }
        if (mainMap == "McWarPveScene")
        {
            if (nextMap == "PVE")
            {
                
                yield break;
            }
            else
            {
                SceneManager.LoadScene("McWarScene",LoadSceneMode.Single);
            }
            
        }
        else
        {
            
        }
        
       
        
       

    }

   
}
