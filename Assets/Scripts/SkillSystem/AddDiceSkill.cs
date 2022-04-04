using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill/AddDice")]
public class AddDiceSkill : SkillBase
{
    public int addValue=1;
    private PlanetResContainer resContainer;
    public override void Init(GameEntity gameEntity)
    {
        base.Init(gameEntity);
        resContainer =gameEntity.GetComponent<PlanetResContainer>();
    }

    protected override void Play()
    {
        base.Play();
        resContainer.AddRes(ResourceType.DicePoint,addValue);
    }
    
}
