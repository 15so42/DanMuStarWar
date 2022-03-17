using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            BattleOverDialog.ShowDialog(15,new Player(1,"云上空", "ss","ss"),null );
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlanetManager.Instance.BattleOverByAdmin();
        }
        
        
    }
}
