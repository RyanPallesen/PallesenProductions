using BepInEx;
using BetterMetamorph;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

namespace PallesenProductions
{
    [R2APISubmoduleDependency(nameof(BuffAPI))]
    [R2APISubmoduleDependency(nameof(ItemAPI))]
    [R2APISubmoduleDependency(nameof(LanguageAPI))]
    [R2APISubmoduleDependency(nameof(R2API.CustomElite))]
    [R2APISubmoduleDependency(nameof(R2API.EliteAPI))]
    [R2APISubmoduleDependency("LoadoutAPI")]
    //[R2APISubmoduleDependency("EntityAPI")]
    //[BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.FloodWarning", "FloodWarning", "4.1.0")]

    public class FloodWarning : BaseUnityPlugin
    {
        public static FloodWarning instance;

        public void Awake()
        {
            //On.RoR2.Networking.GameNetworkManager.OnClientConnect += (self, user, t) => { };

            instance = this;

            Artifacts.Init();
            Elites.Init();
            Skills.Init();
            //PlayableScavenger.Init();
        }

        internal void Log(string v)
        {
            base.Logger.LogInfo(v);
        }
    }
}