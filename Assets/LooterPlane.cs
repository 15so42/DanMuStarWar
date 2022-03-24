using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooterPlane : WarPlane
{
    public Looter ownerLooter;


    public void Loot()
    {
        if (chaseTarget.GetVictimOwner() != null)
        {
            var targetPlanet = chaseTarget.GetVictimOwner() as Planet;
            if (targetPlanet)
            {
                targetPlanet.planetResContainer.ReduceRes(ResourceType.Tech,5);
                targetPlanet.planetResContainer.ReduceRes(ResourceType.DicePoint,1);
                
            }
        }    
    }

    public void Delivery()
    {
        ownerLooter.dicePoint += 1;
        ownerLooter.techPoint += 5;
    }
    
    public override GameEntity FindNearEnemy(bool onlyEnemyPlanet)
    {
        GameEntity enemy = null;
        var enemyPlanets = new List<Planet>();
        if (!onlyEnemyPlanet)
        {
            enemyPlanets = PlanetManager.Instance.allPlanets;
        }

        if (enemyPlanets.Count <= 0)
            return null;
        var planet = enemyPlanets[Random.Range(0, enemyPlanets.Count)];
        if(planet)
        {
            if (planet.battleUnits.Count == 0)
            {
                enemy = planet;
            }
            else
            {
                var enemyUnit = planet.battleUnits[Random.Range(0, planet.battleUnits.Count)];
                if (Vector3.Distance(enemyUnit.transform.position, transform.position) < findEnemyDistance)
                {
                   
                    enemy = enemyUnit;
                
                }
            }

        }
        
            
        return enemy;
    }
}
