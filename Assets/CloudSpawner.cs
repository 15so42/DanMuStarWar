using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    

    public Transform[] clouds;
    // Start is called before the first frame update
    void Start()
    {
        var parent=new GameObject("CloudParent");
        
        parent.transform.SetParent(transform);
        parent.transform.position=Vector3.zero;
        parent.transform.localScale = Vector3.one;
        for (int i = 0; i < clouds.Length; i++)
        {
            var iCloud = GameObject.Instantiate(clouds[i],parent.transform);
            iCloud.Rotate(Random.insideUnitSphere*Random.Range(0,360));
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
