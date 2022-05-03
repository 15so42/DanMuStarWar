using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Heal")]
public class GameEnvSkill : SkillBase
{
    public GameEnvEvent targetEvt;
    public int level;
    
    protected override void Play()
    {
        base.Play();
        targetEvt.Run(level);
    }
}
