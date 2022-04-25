using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/护盾发生器")]
public class ShieldGeneratorSkill : SkillBase
{
   
    public int value = 1;
    List<BattleUnit> inTerritory=new List<BattleUnit>();

    public override void Init(GameEntity gameEntity, PlanetCommander planetCommander)
    {
        base.Init(gameEntity, planetCommander);
        inTerritory = (base.gameEntity as Planet)?.planetTerritory.inTerritory;
    }

    protected override void Play()
    {
        base.Play();
        if (inTerritory != null && (gameEntity as Planet).occupied)
        {
            foreach (var b in inTerritory)
            {
                if (b && !b.die)
                {
                    //Debug.Log("给"+b.name+"添加护盾");
                    b.AddShield(value);
                }
                
            }
        }
        
        
    }
}
