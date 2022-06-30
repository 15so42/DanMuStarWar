using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class LoadJson{
    public static T LoadJsonFromFile<T>(string fileName)where T:class
    {
        var path = Application.dataPath + "/StreamingAssets/" + fileName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("not Found");
            return null;
        }
        
        string json = File.ReadAllText(path);
 
        
        if (json.Length > 0)
        {
            return JsonUtility.FromJson<T>(json);
        }
        return null;
    }
}