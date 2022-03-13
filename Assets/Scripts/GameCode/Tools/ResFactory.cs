﻿using BattleScene.Scripts;
using UnityEngine;

namespace GameCode.Tools
{
    public class ResFactory : Singleton<ResFactory>
    {
        
        public string planetPath="Prefab/Planet/";
        public string battleUnitPath="Prefab/BattleUnit/";
        public string bulletPath="Prefab/Bullet/";

        GameObject InsByResLoad(string path, string name)
        {
            GameObject res = Resources.Load<GameObject>(path + name);
            return GameObject.Instantiate(res);
        }
        
        public GameObject CreatePlanet(string name)
        {
            GameObject go = InsByResLoad(planetPath, name);
            return go;
        }

        public GameObject CreateBattleUnit(string name,Vector3 pos)
        {
            GameObject go = InsByResLoad(battleUnitPath, name);
            go.transform.position = pos;
            return go;
        }

        public GameObject CreateBullet(string name,Vector3 pos)
        {
            RecycleAbleObject poolGo = UnityObjectPoolManager.Allocate(name);
            if (poolGo != null)
            {
                poolGo.transform.position = pos;
                return poolGo.gameObject;
                
            }

            GameObject go = InsByResLoad(bulletPath, name);
            go.transform.position = pos;
            return go;
        }
    }
}