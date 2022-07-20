using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SkillContainer skillContainer = GetComponent<SkillContainer>();
        SkillManager.Instance.AddSkillToObj("Skill_腐蚀领域_LV1",skillContainer);
    }
}
