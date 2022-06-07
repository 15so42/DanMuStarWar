using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/点数领域")]
public class PointTerritorySkill : SkillBase
{
    public float value = 1;
    

    public override void Init(GameEntity gameEntity, PlanetCommander planetCommander)
    {
        base.Init(gameEntity, planetCommander);
        
    }

    protected override void Play()
    {
        base.Play();
        var inTerritory = Physics.OverlapSphere(skillContainer.transform.position, 15);
        
      
        
        foreach (var c in inTerritory)
        {
            var b = c.GetComponent<BattleUnit>();
            if(!b)
                continue;
            
            if (!b.die)
            {
                //Debug.Log("给"+b.name+"添加护盾");
                //b.OnAttacked(new AttackInfo(gameEntity,AttackType.Heal,value));
                b.planetCommander?.AddPoint(value);

            }
                
        }
        ResFactory.Instance.CreateFx(GameConst.FX_POINT_TERRIORITORY, skillContainer.transform.position);
        
    }
}
