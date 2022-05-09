using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPoint : MonoBehaviour
{
    public int addValue = 10;
    // Start is called before the first frame update
    
    private void OnTriggerEnter(Collider other)
    {
        var battleUnit = other.gameObject.GetComponent<Steve>();
        if (battleUnit)
            battleUnit.EnterDefendState(addValue);
    }
    
    private void OnTriggerExit(Collider other)
    {
        var battleUnit = other.gameObject.GetComponent<Steve>();
        if (battleUnit)
            battleUnit.ExitDefendState(addValue);
    }
}
