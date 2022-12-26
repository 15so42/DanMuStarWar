using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DebugManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            DebugDialog.ShowDialog();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Time.timeScale = 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Time.timeScale = 2;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Time.timeScale = 5;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program
            Application.Quit(); //kill current process
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (PVEManager.Instance)
            {
                PVEManager.Instance.difficulty = 60;
                //PVEManager.Instance.SpawnByPlayerCount(3);
                PVEManager.Instance.SetMonsterList(new List<string>(){"BattleUnit_EnderDragon"});
                PVEManager.Instance.SpawnByPlayerCount(1,true);

            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            var steve = (PlanetManager.Instance.allPlanets[0].planetCommanders[0] as SteveCommander)
                .FindFirstValidSteve();
            steve.ChangeWeapon(5);
            for (int i = 0; i < 1000; i++)
            {
                steve.SpecificSpell(false, "空间斩");
                steve.SpecificSpell(false, "吸血");
                steve.SpecificSpell(false, "保护");
                steve.SpecificSpell(false, "灵盾");
                
            }
        }
        
        

        if (Input.GetKeyDown(KeyCode.T))
        {
            DanMuReciver.Instance.SendFakeDanMu("第三者",UnityEngine.Random.Range(100,200),"加入游戏");
        }
        
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     BattleOverDialog.ShowDialog(15,new Player(1,"云上空","s","s"),null );
        // }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlanetManager.Instance.BattleOverByAdmin();
        }
        
        
        
        
    }
}
