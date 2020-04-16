using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using VanillaTweaks;
using DeathRewards = VanillaTweaks.DeathRewards;

namespace PallesenProductions
{
    [R2APISubmoduleDependency("SurvivorAPI")]
    [R2APISubmoduleDependency("DifficultyAPI")]
    [R2APISubmoduleDependency("SkinAPI")]
    [R2APISubmoduleDependency("SkillAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    public struct Tweak
    {
        public string title;
        public string description;
        public Func<bool> startMethod;
    }

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.VanillaTweaks", "VanillaTweaks", "1.2.0")]
    public class VanillaTweaks : BaseUnityPlugin
    {
        public List<Tweak> tweaks = new List<Tweak>();
        private static BepInEx.Configuration.ConfigFile myConfig = new ConfigFile("BepInEx/Config/VanillaTweaks.cfg", true);



        public void Awake()
        {
            //Taken from https://github.com/Legendsmith/ThunderDownUnder/blob/master/SolidIceWall/SolidIceWallLoader.cs
            GameObject pillarprefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            pillarprefab.layer = 11;

            On.RoR2.ArenaMissionController.EndRound += TrueSuicideOnNullVoid;

            On.RoR2.MultiShopController.CreateTerminals += MultishopNoDuplicates;

            On.RoR2.ScavengerItemGranter.Start += ScavengerNoResetGhost;

            On.RoR2.CharacterMaster.AddDeployable += TurretEliteInheritance;

            tweaks.Add(new Tweak() { title = "Aurelionite Tweaks", description = "Adds progression to Aurelionite and adds a small chance for a natural gold portal", startMethod = AurelioniteTweaks.Init });
            tweaks.Add(new Tweak() { title = "ChanceShrineChanges", description = "Chance shrines have slightly randomized values on load, and the more you fail the better the outcome.", startMethod = ChanceShrineChanges.Init });
            tweaks.Add(new Tweak() { title = "ShrineCanvas", description = "Adds a networked display of chances to the shrine of chance", startMethod = ShrineCanvasTweak.Init });
            tweaks.Add(new Tweak() { title = "DeathRewards", description = "Elite Teleporter Champion Bosses have a 25% chance to drop an elite affix, normal enemies have a 0.5% chance", startMethod = DeathRewards.Init });
            tweaks.Add(new Tweak() { title = "MultiElites", description = "Elite enemies can gain multiple elite types if the game had the ability to spawn them naturally", startMethod = MultiElites.Init });
            tweaks.Add(new Tweak() { title = "NoAutoPickup", description = "In multiplayer you won't automatically pick up items", startMethod = NoAutoPickup.Init });
            tweaks.Add(new Tweak() { title = "SizeTweaks", description = "Tweaks the size of bosses and elite enemies to feel more scary and natural", startMethod = SizeTweaks.Init });


            foreach(Tweak tweak in tweaks)
            {
                if(myConfig.Bind<bool>(new ConfigDefinition("Vanilla Tweaks", tweak.title,tweak.description), true).Value)
                {
                    tweak.startMethod();
                }
            }




        }

        private void TrueSuicideOnNullVoid(On.RoR2.ArenaMissionController.orig_EndRound orig, ArenaMissionController self)
        {
            if (self.currentRound >= self.totalRoundsMax)
            {
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
                for (int j = teamMembers.Count - 1; j >= 0; j--)
                {
                    teamMembers[j].body.inventory.ResetItem(ItemIndex.ExtraLife);
                }
            }

            orig(self);
        }


        private void MultishopNoDuplicates(On.RoR2.MultiShopController.orig_CreateTerminals orig, MultiShopController self)
        {
            orig(self);


            List<ShopTerminalBehavior> terminalBehaviors = new List<ShopTerminalBehavior>();
            foreach (GameObject gameObject in self.GetFieldValue<GameObject[]>("terminalGameObjects"))
            {
                if (gameObject.GetComponent<ShopTerminalBehavior>())
                {
                    terminalBehaviors.Add(gameObject.GetComponent<ShopTerminalBehavior>());
                }
            }

            List<PickupIndex> pickups = new List<PickupIndex>();

            foreach (ShopTerminalBehavior shopTerminalBehavior in terminalBehaviors)
            {
                pickups.Add(shopTerminalBehavior.NetworkpickupIndex);
            }

            foreach (ShopTerminalBehavior shopTerminalBehavior in terminalBehaviors)
            {
                if (pickups.FindAll(x => x == shopTerminalBehavior.NetworkpickupIndex).Count > 1)
                {
                    switch (self.itemTier)
                    {
                        case ItemTier.Tier1:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableTier1DropList));
                            break;
                        case ItemTier.Tier2:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableTier2DropList));
                            break;
                        case ItemTier.Tier3:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableTier3DropList));
                            break;
                        case ItemTier.Lunar:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableLunarDropList));
                            break;
                    }

                }
            }
        }

        private void TurretEliteInheritance(On.RoR2.CharacterMaster.orig_AddDeployable orig, CharacterMaster self, Deployable deployable, DeployableSlot slot)
        {

            orig(self, deployable, slot);

            if (deployable.gameObject.GetComponentInChildren<CharacterMaster>())
            {
                deployable.gameObject.GetComponentInChildren<CharacterMaster>().inventory.CopyEquipmentFrom(self.inventory);
                deployable.gameObject.GetComponentInChildren<CharacterMaster>().inventory.ResetItem(ItemIndex.AutoCastEquipment);
                deployable.gameObject.GetComponentInChildren<CharacterMaster>().inventory.ResetItem(ItemIndex.TonicAffliction);

            }
            else if (deployable.gameObject.GetComponentInParent<CharacterMaster>())
            {
                deployable.gameObject.GetComponentInParent<CharacterMaster>().inventory.CopyEquipmentFrom(self.inventory);
                deployable.gameObject.GetComponentInParent<CharacterMaster>().inventory.ResetItem(ItemIndex.AutoCastEquipment);
                deployable.gameObject.GetComponentInParent<CharacterMaster>().inventory.ResetItem(ItemIndex.TonicAffliction);
            }
        }


        private void ScavengerNoResetGhost(On.RoR2.ScavengerItemGranter.orig_Start orig, ScavengerItemGranter self)
        {
            if (base.GetComponent<Inventory>())
            {
                if (base.GetComponent<Inventory>().GetItemCount(ItemIndex.Ghost) > 0)
                {
                }
                else
                {
                    orig(self);
                }
            }
        }
    }

}

