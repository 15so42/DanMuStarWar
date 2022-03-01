using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public float rotateSpeed=5;
    
    public Vector3 rotateDir=Vector3.one;

    public bool randomDir = false;
    // Start is called before the first frame update
    void Start()
    {
        if (randomDir)
            rotateDir = Random.insideUnitSphere;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateDir * (rotateSpeed * Time.deltaTime));
    }
}
