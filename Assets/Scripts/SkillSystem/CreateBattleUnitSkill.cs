using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/生产单位")]
public class CreateBattleUnitSkill : SkillBase
{
    public string unitName = "BattleUnit_战斗机";
    public float duration = 5;
    [Header("生产数量")]
    public int num;
    protected override void Play()
    {
        base.Play();
        var planet = gameEntity as Planet;
        if (planet)
        {
            for (int i = 0; i < num; i++)
            {
                planet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,unitName,5),planetCommander));
            }
            
            
        }
        
        
    }
}
