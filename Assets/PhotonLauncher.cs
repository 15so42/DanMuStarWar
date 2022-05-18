using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

//游玩方式，直播，Photon
public enum PlayMode
{
    Live,
    Photon
}

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static PlayMode playMode;

    private bool allLoaded;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        //初始化房间hash
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ChangeModeToLive()
    {
        playMode = PlayMode.Live;
        SceneManager.LoadScene("StarWarScene");
    }
    
    public void ChangeModeToPhoton()
    {
        playMode = PlayMode.Live;
        SceneManager.LoadScene("StarWarLobbyScene");
    }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player player, Hashtable hashtable)
    {
        //根据游戏进程决定
        
        //如果还没有进入游戏场景，所有玩家准备完毕，进入场景
      
            if ((bool) allLoaded)
            {
                Debug.Log("全部加载场景完成");
                
            }
            else
            {
                if ( CheckPlayersLoaded())
                {
                    
                    FightingManager.Instance.StartBattleByPhoton();
                    FightingManager.Instance.SyncWatingStartByRPC();
                    //PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{"allPlayerLoaded", true}});
                    
                    allLoaded = true;
                }
            }
        
        
        
    }
    
    private bool CheckPlayersLoaded()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue("battleSceneLoaded", out isPlayerReady))
            {
                if (!(bool) isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
