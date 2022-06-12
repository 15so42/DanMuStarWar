using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnhancementLevel
{
    public  string enhancementName;
    public int level;

    public EnhancementLevel(string enhancementName, int level)
    {
        this.enhancementName = enhancementName;
        this.level = level;
    }
}

[Serializable]
public class SteveWeaponNbt
{

    public int maxSpellCount = 3;
    public int endurance;//耐久
    public int maxEndurance;
    
    public List<EnhancementLevel> enhancementLevels=new List<EnhancementLevel>();
}
