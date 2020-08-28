using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using ExpandedSkills2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.RoR2.ConVar;
using PallesenProductions;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Networking;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PallesenProductions
{
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    [R2APISubmoduleDependency(nameof(R2API.OrbAPI))]
    [R2APISubmoduleDependency(nameof(R2API.PrefabAPI))]
    [R2APISubmoduleDependency(nameof(R2API.AssetAPI))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.ExpandedSkills", "ExpandedSkills", "2.0.0")]

    public class Skills
    {

        public static bool Init()
        {
            Commando.Setup();
            Huntress.Setup();
            Artificer.Setup();
            Mercenary.Setup();
            Engineer.Setup();
            Acrid.Setup();

            return true;
            //#region merc
            //LoadoutAPI.AddSkill(typeof(VTStates.States.Mercenary.SummonClones));
            //{
            //    SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //    mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mercenary.SummonClones));
            //    mySkillDef.activationStateMachineName = "Weapon";
            //    mySkillDef.baseMaxStock = 2;
            //    mySkillDef.baseRechargeInterval = 16f;
            //    mySkillDef.beginSkillCooldownOnSkillEnd = false;
            //    mySkillDef.canceledFromSprinting = false;
            //    mySkillDef.fullRestockOnAssign = true;
            //    mySkillDef.interruptPriority = InterruptPriority.Any;
            //    mySkillDef.isBullets = false;
            //    mySkillDef.isCombatSkill = true;
            //    mySkillDef.mustKeyPress = true;
            //    mySkillDef.noSprint = false;
            //    mySkillDef.rechargeStock = 1;
            //    mySkillDef.requiredStock = 1;
            //    mySkillDef.shootDelay = 0f;
            //    mySkillDef.stockToConsume = 1;
            //    //mySkillDef.icon = Resources.Load<Sprite>()
            //    mySkillDef.skillDescriptionToken = "Summon a shadow clone that inherits your items";
            //    mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONCLONE_SPECIAL";
            //    mySkillDef.skillNameToken = "Shadow Clones";

            //    LoadoutAPI.AddSkillDef(mySkillDef);

            //    GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/mercbody");
            //    SkillLocator component = gameObject.GetComponent<SkillLocator>();
            //    SkillFamily skillFamily = component.special.skillFamily;

            //    Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            //    skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            //    {
            //        skillDef = mySkillDef,
            //        unlockableName = "",
            //        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

            //    };
            //}


            //#endregion merc

            #region commando

            #endregion commando
        }


        public static SkillFamily FromList(List<(SkillDef def, string unlockable)> defs, string name)
        {
            foreach (var (def, unlock) in defs)
            {
                (def as ScriptableObject).name = def.skillName;
            }
            var fam = CreateSkillFamily(defs[0].def, defs.Skip(1).ToArray());
            (fam as ScriptableObject).name = name;

            return fam;
        }
        public static SkillFamily CreateSkillFamily(SkillDef defaultSkill, params (SkillDef skill, String unlockable)[] variants)
        {
            //Log.Counter();

            SkillFamily family = ScriptableObject.CreateInstance<SkillFamily>();
            family.variants = new SkillFamily.Variant[variants.Length + 1];
            family.variants[0] = new SkillFamily.Variant
            {
                skillDef = defaultSkill,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(defaultSkill.skillName, false),
            };
            LoadoutAPI.AddSkillDef(defaultSkill);

            for (Int32 i = 0; i < variants.Length; ++i)
            {
                (SkillDef skill, String unlockable) info = variants[i];
                SkillDef skill = info.skill;
                family.variants[i + 1] = new SkillFamily.Variant
                {
                    skillDef = skill,
                    unlockableName = info.unlockable,
                    viewableNode = new ViewablesCatalog.Node(skill.skillName, false),
                };
                LoadoutAPI.AddSkillDef(skill);
            }

            LoadoutAPI.AddSkillFamily(family);
            //Log.Counter();

            return family;
        }
    }
}




//[Info: Unity Log] [0] AncientWispMaster=ANCIENTWISP_BODY_NAME
//[1] ArchWispMaster = ARCHWISP_BODY_NAME
//[2]ArtifactShellMaster=ArtifactReliquary
//[3] BeetleCrystalMaster = BEETLE_CRYSTAL_BODY_NAME
//[4]BeetleGuardAllyMaster=BeetleGuard
//[5] BeetleGuardMaster = BeetleGuard
//[6]BeetleGuardMasterCrystal=BeetleGuard
//[7] BeetleMaster = Beetle
//[8]BeetleQueenMaster=BeetleQueen
//[9] BellMaster = BrassContraption
//[10]BisonMaster=BighornBison
//[11] BrotherGlassMaster = Mithrix
//[12]BrotherHauntMaster=???
//[13] BrotherHurtMaster = Mithrix
//[14]BrotherMaster=Mithrix
//[15] CaptainMonsterMaster = Captain
//[16]ClayBossMaster=ClayDunestrider
//[17] ClayBruiserMaster = ClayTemplar
//[18]ClaymanMaster=ClayMan
//[19] CommandoMonsterMaster = Commando
//[20]CrocoMonsterMaster=Acrid
//[21] Drone1Master = GunnerDrone
//[22]Drone2Master=HealingDrone
//[23] DroneBackupMaster = StrikeDrone
//[24]DroneMissileMaster=MissileDrone
//[25] ElectricWormMaster = OverloadingWorm
//[26]EmergencyDroneMaster=EmergencyDrone
//[27] EngiBeamTurretMaster = EngineerTurret
//[28]EngiMonsterMaster=Engineer
//[29] EngiTurretMaster = EngineerTurret
//[30]EngiWalkerTurretMaster=EngineerTurret
//[31] EquipmentDroneMaster = EquipmentDrone
//[32]FlameDroneMaster=IncineratorDrone
//[33] GolemMaster = StoneGolem
//[34]GrandparentMaster=Grandparent
//[35] GravekeeperMaster = Grovetender
//[36]GreaterWispMaster=GreaterWisp
//[37] HermitCrabMaster = HermitCrab
//[38]HuntressMonsterMaster=Huntress
//[39] ImpBossMaster = ImpOverlord
//[40]ImpMaster=Imp
//[41] JellyfishMaster = Jellyfish
//[42]LemurianBruiserMaster=ElderLemurian
//[43] LemurianBruiserMasterFire = ElderLemurian
//[44]LemurianBruiserMasterHaunted=ElderLemurian
//[45] LemurianBruiserMasterIce = ElderLemurian
//[46]LemurianBruiserMasterPoison=ElderLemurian
//[47] LemurianMaster = Lemurian
//[48]LoaderMonsterMaster=Loader
//[49] LunarGolemMaster = LunarChimera
//[50]LunarWispMaster=LunarChimera
//[51] MageMonsterMaster = Artificer
//[52]MagmaWormMaster=MagmaWorm
//[53] MegaDroneMaster = TC280Prototype
//[54]MercMonsterMaster=Mercenary
//[55] MiniMushroomMaster = MiniMushrum
//[56]NullifierMaster=VoidReaver
//[57] ParentMaster = Parent
//[58]ParentPodMaster=AncestralPod
//[59] RoboBallBossMaster = SolusControlUnit
//[60]RoboBallMiniMaster=SolusProbe
//[61] ScavLunar1Master = KipkiptheGentle
//[62]ScavLunar2Master=WipwiptheWild
//[63] ScavLunar3Master = TwiptwiptheDevotee
//[64]ScavLunar4Master=GuraguratheLucky
//[65] ScavMaster = Scavenger
//[66]ShopkeeperMaster=Newt
//[67] SquidTurretMaster = SquidTurret
//[68]SuperRoboBallBossMaster=AlloyWorshipUnit
//[69] TitanGoldAllyMaster = Aurelionite
//[70]TitanGoldMaster=Aurelionite
//[71] TitanMaster = StoneTitan
//[72]ToolbotMonsterMaster=MULT
//[73] TreebotMonsterMaster = REX
//[74]Turret1Master=GunnerTurret
//[75] UrchinTurretMaster = MalachiteUrchin
//[76]VagrantMaster=WanderingVagrant
//[77] VultureMaster = AlloyVulture
//[78]WispMaster=LesserWisp
//[79] WispSoulMaster = LesserWisp
//[80]TwitchMonsterMaster=Twitch