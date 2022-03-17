using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/生产单位")]
public class CreateBattleUnitSkill : SkillBase
{
    public string unitName = "BattleUnit_战斗机";
    public float duration = 5;
    protected override void Play()
    {
        base.Play();
        var planet = gameEntity as Planet;
        if (planet)
        {
            planet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,unitName,5)));
            
        }
        
        
    }
}
