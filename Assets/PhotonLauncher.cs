using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//游玩方式，直播，Photon
public enum PlayMode
{
    Live,
    Photon
}

public class PhotonLauncher : MonoBehaviour
{
    public static PlayMode playMode;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeModeToLive()
    {
        playMode = PlayMode.Live;
        SceneManager.LoadScene("StarWarScene");
    }
    
    public void ChangeModeToPhoton()
    {
        playMode = PlayMode.Live;
        SceneManager.LoadScene("PunCockpit-Scene");
    }
}
