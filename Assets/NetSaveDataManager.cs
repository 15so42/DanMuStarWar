using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSaveDataManager 
{

    public void Save(List<Player> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            PhpTester.Instance.UpdateUser(players[i].userSaveData);
        }
    }
}
