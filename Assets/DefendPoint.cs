using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPoint : MonoBehaviour
{
    public int rangeValue = 10;
    public int attackDistanceValue = 10;
    // Start is called before the first frame update
    
    private void OnCollisionEnter(Collision other)
    {
        var battleUnit = other.gameObject.GetComponent<Steve>();
        if (battleUnit)
            battleUnit.EnterDefendState(rangeValue,attackDistanceValue);
    }
    
    private void OnCollisionExit(Collision other)
    {
        var battleUnit = other.gameObject.GetComponent<Steve>();
        if (battleUnit)
            battleUnit.ExitDefendState(rangeValue,attackDistanceValue);
    }
}
