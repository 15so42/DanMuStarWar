using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class McFightingManager : FightingManager
{

    private string nextMap = "村庄";

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

    protected void Start()
    {
        base.Start();
        //mcPosManager=MCPosManager.Instance;
        nextMap = "村庄";
        SceneManager.LoadScene(GetSceneNameByChinese(nextMap), LoadSceneMode.Additive);
    }

    // public void SetAdditiveScene(string name)
    // {
    //     SceneManager.LoadScene(name, LoadSceneMode.Additive);
    //     
    // }
    //

    public void SetNextMap(string key)
    {
        nextMap = key;
    }

    protected override void OnGameOver()
    {
        base.OnGameOver();
        if (GetSceneNameByChinese(nextMap) == SceneManager.GetSceneAt(1).name)
        {
            return;
        }
        StartCoroutine(NewMap());
    }

    IEnumerator NewMap()
    {
        var mainMap = SceneManager.GetSceneAt(0).name;
        if (mainMap == "McWarScene")
        {
            if (nextMap == "PVE")
            {
                SceneManager.LoadScene("McWarPveScene",LoadSceneMode.Single);
                yield break;
            }
            var unload=SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
            yield return unload.isDone;
            SceneManager.LoadScene(GetSceneNameByChinese(nextMap), LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene("McWarScene",LoadSceneMode.Single);
        }
        
       
        
       

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