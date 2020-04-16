using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

namespace VanillaTweaks
{
    public static class BetterPrototypeMovement
    {
        public static bool Init()
        {
           GameObject prototype = Resources.Load<GameObject>("Prefabs/CharacterBodies/megadronebody");
            foreach (MonoBehaviour skill in prototype.GetComponentsInChildren<MonoBehaviour>())
            {
                if (skill as RoR2.RigidbodyStickOnImpact)
                {
                    GameObject.DestroyImmediate(skill);
                }
            }
            return true;
        }
    }
}
