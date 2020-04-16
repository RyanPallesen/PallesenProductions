using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using System;
using System.Collections.Generic;
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
            tweaks.Add(new Tweak() { title = "Aurelionite Tweaks", description = "Adds progression to Aurelionite and adds a small chance for a natural gold portal", startMethod = AurelioniteTweaks.Init });
            tweaks.Add(new Tweak() { title = "ChanceShrineChanges", description = "Chance shrines have slightly randomized values on load, and the more you fail the better the outcome.", startMethod = ChanceShrineChanges.Init });
            tweaks.Add(new Tweak() { title = "ShrineCanvas", description = "Adds a networked display of chances to the shrine of chance.", startMethod = ShrineCanvasTweak.Init });
            tweaks.Add(new Tweak() { title = "DeathRewards", description = "Elite Teleporter Bosses have a 25% chance to drop an elite affix, normal enemies have a 0.5% chance.", startMethod = DeathRewards.Init });
            tweaks.Add(new Tweak() { title = "MultiElites", description = "Elite enemies can gain multiple elite types if the game had the ability to spawn them naturally.", startMethod = MultiElites.Init });
            tweaks.Add(new Tweak() { title = "NoAutoPickup", description = "In multiplayer you won't automatically pick up items.", startMethod = NoAutoPickup.Init });
            tweaks.Add(new Tweak() { title = "SizeTweaks", description = "Tweaks the size of bosses and elite enemies to feel more scary and natural.", startMethod = SizeTweaks.Init });
            tweaks.Add(new Tweak() { title = "MultiShopNoDuplicates", description = "Makes it much less likely for a multishop to have duplicates.", startMethod = MultiShopNoDuplicates.Init });
            tweaks.Add(new Tweak() { title = "TurrentInheritance", description = "Deployables, such as turrets, recieve your elite affix.", startMethod = SizeTweaks.Init });
            tweaks.Add(new Tweak() { title = "TrueSuicideNullVoid", description = "In the Null Sector, enemies will not revive with Dio's after the round is finished.", startMethod = TrueSuicideVoidSector.Init });
            tweaks.Add(new Tweak() { title = "NoScavengerGhostReset", description = "If a scavenger becomes a ghost through Happiest Mask, it won't reset it's items.", startMethod = NoScavengerResetGhost.Init });
            tweaks.Add(new Tweak() { title = "SolidIceWall", description = "Makes artificer's ice wall solid.", startMethod = SolidIceWall.Init });
            tweaks.Add(new Tweak() { title = "BetterPrototypeMovement", description = "Makes the Prototype drone less wonky when it flies and bumps into things", startMethod = BetterPrototypeMovement.Init });
            
            foreach(Tweak tweak in tweaks)
            {
                if(myConfig.Bind<bool>(new ConfigDefinition("Vanilla Tweaks", tweak.title,tweak.description), true).Value)
                {
                    tweak.startMethod();
                }
            }

        }
    }

}

