using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //管理Manager
   
    public FightingManager fightingManager;
    //public UIManager uIManager;

    public PlanetManager planetManager;//存放所有planet
    public BattleUnitManager battleUnitManager;//存放所有battleUnit
    
    

    public UIManager uiManager;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
       
        uiManager.Init(this);
        
        fightingManager.Init(this);
        
        //moveChessManager由GridManager初始化
        //moveChessManager.Init();
        //设置帧率为30fps
        //Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

  
}
