using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    

    public Transform[] clouds;

    public Vector3 cloudSize = Vector3.one;
    // Start is called before the first frame update
    void Start()
    {
        var parent=new GameObject("CloudParent");
        
        parent.transform.SetParent(transform);
        parent.transform.localPosition=Vector3.zero;
        parent.transform.localScale = Vector3.one;
        
        var iCloud = GameObject.Instantiate(clouds[Random.Range(0,clouds.Length)],parent.transform);
        var transform1 = iCloud.transform;
        transform1.localPosition=Vector3.zero;
        transform1.localScale=cloudSize;
        iCloud.gameObject.AddComponent<RotateSelf>();

    }

    
}
