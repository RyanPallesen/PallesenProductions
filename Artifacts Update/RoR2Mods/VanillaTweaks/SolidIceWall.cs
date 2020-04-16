using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

namespace VanillaTweaks
{
    public static class SolidIceWall
    {
        public static bool Init()
        {
            //Taken from https://github.com/Legendsmith/ThunderDownUnder/blob/master/SolidIceWall/SolidIceWallLoader.cs
            GameObject pillarprefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            pillarprefab.layer = 11;

            return true;
        }
    }
}
