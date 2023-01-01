using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorfulSheep : MonoBehaviour
{
    public SkinnedMeshRenderer sheepFur;
    // Start is called before the first frame update
    void Start()
    {
        sheepFur.materials[0].color = new Color(UnityEngine.Random.Range(0f, 1f),UnityEngine.Random.Range(0f, 1f),UnityEngine.Random.Range(0f, 1f));
    }

    
}
