using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McPveRoundManager : McRoundManager
{
    private GameObject fireWall;
    private UnityTimer.Timer fireWallTimer;
    protected override void ParseGiftInMcMode(SteveCommander steveCommander, string giftName, int battery)
    {
        base.ParseGiftInMcMode(steveCommander, giftName, battery);
        if (giftName == "情书")
        {
            fireWall=GameObject.FindGameObjectWithTag("FireWall");

            for (int i = 0; i < fireWall.transform.childCount; i++)
            {
                fireWall.transform.GetChild(i).gameObject.SetActive(true);
            }
            fireWall.gameObject.SetActive(true);
            fireWallTimer = UnityTimer.Timer.Register(180, () =>
            {
                for (int i = 0; i < fireWall.transform.childCount; i++)
                {
                    fireWall.transform.GetChild(i).gameObject.SetActive(false);
                }
            });
        }

        if (giftName == "小花花")
        {
            steveCommander.SummonGolem();
        }
    }

    private void OnDisable()
    {
        fireWallTimer?.Cancel();
    }
}
