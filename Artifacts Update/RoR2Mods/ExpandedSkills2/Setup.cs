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

    public class ExpandedSkills : BaseUnityPlugin
    {

        public void Awake()
        {
            Commando.Setup();
            Huntress.Setup();
            Artificer.Setup();
            Mercenary.Setup();
            Engineer.Setup();
            Acrid.Setup();

            
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

            //LoadoutAPI.AddSkill(typeof(VTStates.States.Mercenary.SummonManyClones));
            //{
            //    SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //    mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mercenary.SummonManyClones));
            //    mySkillDef.activationStateMachineName = "Weapon";
            //    mySkillDef.baseMaxStock = 8;
            //    mySkillDef.baseRechargeInterval = 8f;
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
            //    mySkillDef.skillDescriptionToken = "Summon a miniature clone that inherits your items";
            //    mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONMANYCLONE_SPECIAL";
            //    mySkillDef.skillNameToken = "Swarming Clones";

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



        //#region networking

        //public const Int16 HandleId = 255;

        //public class MyMessage : MessageBase
        //{
        //    public NetworkInstanceId objectID;
        //    public int summonType;

        //    public override void Serialize(NetworkWriter writer)
        //    {
        //        writer.Write(objectID);
        //        writer.Write(summonType);
        //    }

        //    public override void Deserialize(NetworkReader reader)
        //    {
        //        objectID = reader.ReadNetworkId();
        //        summonType = reader.ReadInt32();
        //    }
        //}

        //public static void SendSummonMaster(NetworkInstanceId myObjectID, string masterBody)
        //{
        //    NetworkServer.SendToAll(HandleId, new MyMessage
        //    {
        //        objectID = myObjectID,
        //        summonType = summoningType
        //    });
        //}

        //[RoR2.Networking.NetworkMessageHandler(msgType = HandleId, client = true)]
        //public static void HandleDropItem(NetworkMessage netMsg)
        //{
        //    var MyMessage = netMsg.ReadMessage<MyMessage>();


        //    if (NetworkServer.active)
        //    {
        //        CharacterBody characterBody = ClientScene.FindLocalObject(MyMessage.objectID).GetComponent<CharacterBody>();
        //        CharacterMaster characterMaster;
        //        if (characterBody)
        //        {
        //            {

        //                characterMaster = new MasterSummon
        //                {
        //                    masterPrefab = MasterCatalog.FindMasterPrefab("MercMonsterMaster"),
        //                    position = characterBody.footPosition + characterBody.transform.up,
        //                    rotation = characterBody.transform.rotation,
        //                    summonerBodyObject = null,
        //                    ignoreTeamMemberLimit = true,
        //                    teamIndexOverride = characterBody.teamComponent.teamIndex

        //                }.Perform();



        //                characterMaster.bodyPrefab = characterBody.master.bodyPrefab;
        //                characterMaster.Respawn(characterMaster.GetBody().footPosition, Quaternion.identity);

        //                characterMaster.inventory.CopyItemsFrom(characterBody.inventory);
        //                characterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);

        //                characterMaster.inventory.CopyEquipmentFrom(characterBody.inventory);

        //                SkinDef[] skins = BodyCatalog.GetBodySkins(BodyCatalog.FindBodyIndex(characterMaster.GetBody()));

        //                int skinIndex = (int)characterBody.skinIndex;

        //                CharacterModel mymodel = characterMaster.GetBody().modelLocator.modelTransform.GetComponentInChildren<CharacterModel>();

        //                characterMaster.loadout.bodyLoadoutManager.SetSkinIndex(BodyCatalog.FindBodyIndex(characterMaster.GetBody()), (uint)skinIndex);
        //                skins[skinIndex].Apply(mymodel.gameObject);



        //                //Array.Resize(ref skins[skinIndex].gameObjectActivations, skins[skinIndex].gameObjectActivations.Length + 1);
        //                //skins[skinIndex].gameObjectActivations[skins[skinIndex].gameObjectActivations.Length - 1] = new SkinDef.GameObjectActivation() { gameObject = mymodel.gameObject, shouldActivate = true };

        //                skins[skinIndex].Apply(characterMaster.GetBody().modelLocator.modelTransform.GetComponent<CharacterModel>().gameObject);

        //                if (MyMessage.summonType == 1) // brood
        //                {

        //                    characterMaster.GetBody().modelLocator.modelBaseTransform.transform.localScale *= 0.5f;
        //                    characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 16);

        //                    characterMaster.GetBody().baseMaxHealth /= 4;
        //                    characterMaster.GetBody().levelMaxHealth /= 4;
        //                    characterMaster.GetBody().baseDamage /= 4;
        //                    characterMaster.GetBody().levelDamage /= 4;

        //                }
        //                else if (MyMessage.summonType == 2) // many clones
        //                {

        //                    characterMaster.GetBody().modelLocator.modelBaseTransform.transform.localScale *= 0.5f;
        //                    characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 8);
        //                    characterMaster.GetBody().baseMaxHealth /= 8;
        //                    characterMaster.GetBody().levelMaxHealth /= 8;
        //                    characterMaster.GetBody().baseDamage /= 8;
        //                    characterMaster.GetBody().levelDamage /= 8;
        //                }
        //                else // normal 2 clones
        //                {
        //                    characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 16);
        //                    characterMaster.GetBody().baseMaxHealth /= 4;
        //                    characterMaster.GetBody().levelMaxHealth /= 4;
        //                    characterMaster.GetBody().baseDamage /= 4;
        //                    characterMaster.GetBody().levelDamage /= 4;
        //                }
        //            }
        //        }
        //    }

        //}
        //#endregion networking
    }


}

//public class GenericOrb : LightningOrb
//{
//    public string prefabPath;
//    public bool canBounceOnSameTarget;
//    public override void Begin()
//    {

//        base.duration = 0.1f;

//        EffectData effectData = new EffectData
//        {
//            origin = origin,
//            genericFloat = base.duration
//        };
//        if (target)
//        {
//            effectData.SetHurtBoxReference(target);
//        }
//        EffectManager.SpawnEffect(Resources.Load<GameObject>(prefabPath), effectData, true);
//    }

//    public override void OnArrival()
//    {


//        if (target)
//        {
//            for (int i = 0; i < targetsToFindPerBounce; i++)
//            {
//                if (bouncesRemaining > 0)
//                {
//                    if (canBounceOnSameTarget)
//                    {
//                        bouncedObjects.Clear();
//                    }
//                    if (bouncedObjects != null)
//                    {
//                        bouncedObjects.Add(target.healthComponent);
//                    }
//                    HurtBox hurtBox = PickNextTarget(target.transform.position);
//                    if (hurtBox)
//                    {
//                        GenericOrb lightningOrb = new GenericOrb();
//                        //lightningOrb.search = this.search;
//                        lightningOrb.origin = target.transform.position;
//                        lightningOrb.target = hurtBox;
//                        lightningOrb.attacker = attacker;
//                        lightningOrb.teamIndex = teamIndex;
//                        lightningOrb.damageValue = damageValue * damageCoefficientPerBounce;
//                        lightningOrb.bouncesRemaining = bouncesRemaining - 1;
//                        lightningOrb.isCrit = isCrit;
//                        lightningOrb.bouncedObjects = bouncedObjects;
//                        lightningOrb.lightningType = lightningType;
//                        lightningOrb.procChainMask = procChainMask;
//                        lightningOrb.procCoefficient = procCoefficient;
//                        lightningOrb.damageColorIndex = damageColorIndex;
//                        lightningOrb.damageCoefficientPerBounce = damageCoefficientPerBounce;
//                        lightningOrb.speed = speed;
//                        lightningOrb.range = range;
//                        lightningOrb.prefabPath = prefabPath;

//                        OrbManager.instance.AddOrb(lightningOrb);
//                    }
//                }
//            }

//            HealthComponent healthComponent = target.healthComponent;
//            if (healthComponent)
//            {
//                DamageInfo damageInfo = new DamageInfo();
//                damageInfo.damage = damageValue;
//                damageInfo.attacker = attacker;
//                damageInfo.inflictor = null;
//                damageInfo.force = Vector3.zero;
//                damageInfo.crit = isCrit;
//                damageInfo.procChainMask = procChainMask;
//                damageInfo.procCoefficient = procCoefficient;
//                damageInfo.position = target.transform.position;
//                damageInfo.damageColorIndex = damageColorIndex;
//                damageInfo.damageType = damageType;
//                healthComponent.TakeDamage(damageInfo);
//                GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
//                GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
//            }
//        }
//    }
//}

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