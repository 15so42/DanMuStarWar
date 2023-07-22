using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoConnectBOpen : MonoBehaviour
{
    public string idCode;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ConnectViaCode>().LinkStart(idCode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
