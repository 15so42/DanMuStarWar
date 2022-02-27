using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public float rotateSpeed=5;
    
    public Vector3 rotateDir=Vector3.one;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateDir * (rotateSpeed * Time.deltaTime));
    }
}
