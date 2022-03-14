using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[CreateAssetMenu(menuName = "Buff")]
public class BuffBase : SkillBase
{
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ready = true;
            PlayCheck();

        }
    }
}
