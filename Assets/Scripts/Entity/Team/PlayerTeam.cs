using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamId
{
    Red,
    Black
}

[CreateAssetMenu(menuName="ScriptableObject/PlayerTeam")]
public class PlayerTeam : ScriptableObject
{
    
    public TeamId teamId = TeamId.Red;

    public Color teamColor = Color.red;
    
    public string teamColorString = "Red";

    public string teamName = "队伍名";
    
   
}
