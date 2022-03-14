using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/Heal")]
public class HealSkill : SkillBase
{
    public int healValue=1;
    private BattleUnitProps prop;
    public override void Init(GameEntity gameEntity)
    {
        base.Init(gameEntity);
        prop=gameEntity.GetComponent<BattleUnitProps>();
    }

    public override void Play()
    {
        base.Play();
        gameEntity.OnAttacked(new AttackInfo(gameEntity,AttackType.Real, healValue));
        //Debug.Log("治疗");
    }
}
