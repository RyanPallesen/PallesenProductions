using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.RoR2.ConVar;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace PallesenProductions
{
    class Waveify : BaseUnityPlugin
    {
        public static int CurrentWave = 0;

        //SYSTEM:
        //Money earned by killing enemies (amount does not increase at higher levels)
        //Pay X to re-roll the enemy type (wisp, golem, bison, etc)
        //Pay X to re-roll the item they are given

        //Pay X to upgrade the enemy type (wisp -> ancient wisp -> archaic wisp)
        //Pay X to upgrade the amount of random items they are given (using GiveRandomItems, displayed on a world-space inventory display)

        //Pay X to add an elite type
        //Pay X to spawn an extra enemy

        //Note; base elite type off of eliteTierDef
        //Note; base available enemies off of current accepted enemies
        //Note; base available elite types off of current possible elite types
        //Note; custom enemy-type upgrades (Miniature golem -> normal golem -> miniature colossus -> normal collosus)
        //Note; allow buy turret to build, permanent ice wall to build, permanent mine swarm to build.

        //Note; allow buy upgrades to your skills (edit the skilldef to have 1.25x stocks, increase damage by 1.1x, increase proc coefficient by 1.1x)

            //Have a shrine/something you interact with, which opens the UI and unlocks your mouse. ESC will close it automatically, moving too far away will close it automatically.
        private void Awake()
        {
            On.RoR2.Run.Awake += NewGame;

            On.RoR2.TeleporterInteraction.OnStateChanged += StateChanged;

            On.RoR2.SceneDirector.PopulateScene += DoNotPopulateScene;
        }

        private void DoNotPopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, SceneDirector self)
        {
            self.SetFieldValue<float>("interactableCredit",0);

            orig(self);
        }

        private void NewGame(On.RoR2.Run.orig_Awake orig, Run self)
        {
            CurrentWave = 0;

            orig(self);
        }

        private void StateChanged(On.RoR2.TeleporterInteraction.orig_OnStateChanged orig, TeleporterInteraction self, int oldActivationState, int newActivationState)
        {
            
            if((newActivationState == 1))//On click to start, spawn a new wave and go to charging.
            {
                NextWave();
                orig(self, 0, 1);
            }
            if ((newActivationState == 2))//when start charging, go back to idle.
            {
                orig(self, 0, 0);
            }
        }

        private void NextWave()
        {
            CurrentWave++;

        }
    }
}

