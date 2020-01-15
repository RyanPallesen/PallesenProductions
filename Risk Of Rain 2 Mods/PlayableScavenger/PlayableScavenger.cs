using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.ScavMonster;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.RoR2.ConVar;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Networking;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


//no elite equip cooldown?

//size tweaks
//shrine chance returns
//shrine restack reusable
//no auto pickup
//Scavenger ghost doesn't get new items
//item drops have higher wave
//custom equipment
//deployables grab elite type
//AI follow has elite type
//Elite names for yellow / gilded
//Expanded difficulty
//solid arti wall
//higher elite drop chance
//multishop no duplicate
//shrine chance canvas
//void end true kill dios
//red / green / boss triple shop in lunar
//beads of fealty stacking
//AI no inherit spinel debuff

//buy drone with items like equipment
//reopen void field portal with lunar
//cleansing pool in lunar
//multi-elite chance
//void no advance time, or slow time
//chest ping say open or not
//death screen say elite type
//change rex intro pod

namespace PallesenProductions
{
    [R2APISubmoduleDependency("SurvivorAPI")]
    [R2APISubmoduleDependency("DifficultyAPI")]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("PrefabAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.PlayableScavenger", "PlayableScavenger", "1.0.0")]

    public class PlayableScavenger : BaseUnityPlugin
    {
        public GameObject myCharacter;


        public void Awake()
        {

            On.EntityStates.ScavMonster.Death.AttemptDropPack += (orig, self) =>
            {
                if (self.GetTeam() == TeamIndex.Player)
                {

                }
                else
                {
                    orig(self);
                }
            };

            #region characters
            {
                myCharacter = Resources.Load<GameObject>("Prefabs/CharacterBodies/ScavBody").InstantiateClone("ScavPlayerBody");
                GameObject gameObject = myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
                gameObject.transform.localScale *= 0.25f;
                gameObject.transform.Translate(new Vector3(0, 3, 0));
                myCharacter.GetComponent<CharacterBody>().aimOriginTransform.Translate(new Vector3(0, -3, 0));

                foreach (KinematicCharacterController.KinematicCharacterMotor behaviour in myCharacter.GetComponentsInChildren<KinematicCharacterController.KinematicCharacterMotor>())
                {
                    float currentY = behaviour.Capsule.center.y;

                    behaviour.SetCapsuleDimensions(behaviour.Capsule.radius * 0.25f, behaviour.Capsule.height * 0.25f, -0f);

                }

                SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

                foreach (GenericSkill skill in myCharacter.GetComponentsInChildren<GenericSkill>())
                {
                    DestroyImmediate(skill);
                }

                skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);


                {
                    skillLocator.primary = myCharacter.AddComponent<GenericSkill>();
                    SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                    newFamily.variants = new SkillFamily.Variant[1];
                    LoadoutAPI.AddSkillFamily(newFamily);
                    skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
                }
                {
                    skillLocator.secondary = myCharacter.AddComponent<GenericSkill>();
                    SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                    newFamily.variants = new SkillFamily.Variant[1];

                    LoadoutAPI.AddSkillFamily(newFamily);
                    skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
                }
                {
                    skillLocator.utility = myCharacter.AddComponent<GenericSkill>();
                    SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                    newFamily.variants = new SkillFamily.Variant[1];

                    LoadoutAPI.AddSkillFamily(newFamily);
                    skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
                }
                {
                    skillLocator.special = myCharacter.AddComponent<GenericSkill>();
                    SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                    newFamily.variants = new SkillFamily.Variant[1];

                    LoadoutAPI.AddSkillFamily(newFamily);
                    skillLocator.special.SetFieldValue("_skillFamily", newFamily);
                }

                BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                {
                    list.Add(myCharacter);
                };
                gameObject.AddComponent<Animation>();

                CharacterBody component = myCharacter.GetComponent<CharacterBody>();
                component.baseDamage = 12f;
                component.levelDamage = 0.4f;
                component.baseCrit = 2f;
                component.levelCrit = 1f;
                component.baseMaxHealth = 300f;
                component.levelMaxHealth = 25f;
                component.baseArmor = 20f;
                component.baseRegen = 1f;
                component.levelRegen = 0.4f;
                component.baseMoveSpeed = 8f;
                //component.levelMoveSpeed = 0.25f;
                component.baseAttackSpeed = 1f;
                component.name = "ScavPlayerBody";

                myCharacter.AddComponent<ItemDisplay>();
                myCharacter.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

                SetStateOnHurt stateOnHurt = myCharacter.AddComponent<SetStateOnHurt>();
                stateOnHurt.canBeFrozen = true;
                stateOnHurt.canBeStunned = true;
                stateOnHurt.canBeHitStunned = false;
                stateOnHurt.idleStateMachine = myCharacter.GetComponentsInChildren<EntityStateMachine>();
                stateOnHurt.targetStateMachine = myCharacter.GetComponentsInChildren<EntityStateMachine>()[0];
                stateOnHurt.hurtState = new SerializableEntityStateType(typeof(FrozenState));
                stateOnHurt.targetStateMachine.initialStateType = new SerializableEntityStateType(typeof(SpawnTeleporterState));
                stateOnHurt.targetStateMachine.mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));

                foreach(EntityStateMachine machine in myCharacter.GetComponentsInChildren<EntityStateMachine>())
                {
                    machine.initialStateType = new SerializableEntityStateType(typeof(SpawnTeleporterState));
                    machine.mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));
                }

                SurvivorDef survivorDef = new SurvivorDef
                {
                    bodyPrefab = myCharacter,
                    descriptionToken = "MyDescription",
                    displayPrefab = gameObject,
                    primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                    name = "ScavPlayerBody",
                    unlockableName = ""// "Logs.Stages.limbo"
                };
                SurvivorAPI.AddSurvivor(survivorDef);
            }
            #endregion characters

            skillsetup();
            passivesetup();
        }

        #region skills

        private void passivesetup()
        {

            GenericSkill skill = myCharacter.AddComponent<GenericSkill>();

            SkillFamily passiveFamily = ScriptableObject.CreateInstance<SkillFamily>();
            skill.SetFieldValue<SkillFamily>("_skillFamily", passiveFamily);
            passiveFamily.variants = new SkillFamily.Variant[4];
            LoadoutAPI.AddSkillFamily(passiveFamily);
            SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.skillDescriptionToken = "Start off with some movement items";
                mySkillDef.skillName = "VT_SCAVSTASHMOVEMENT_MISC";
                mySkillDef.skillNameToken = "Scavenger's Stash - Movement";
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.BaseState));

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;

                passiveFamily.variants[1] =
                new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",

                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                };
            }
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.skillDescriptionToken = "Start off with some attack speed items";
                mySkillDef.skillName = "VT_SCAVSTASHATTACKSPEED_MISC";
                mySkillDef.skillNameToken = "Scavenger's Stash - Attack speed";
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.BaseState));

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;

                passiveFamily.variants[2] =
                new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",

                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                };
            }
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.skillDescriptionToken = "Start off with some healing items";
                mySkillDef.skillName = "VT_SCAVSTASHHEALING_MISC";
                mySkillDef.skillNameToken = "Scavenger's Stash - Healing";
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.BaseState));

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;

                passiveFamily.variants[0] =
                new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",

                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                };
            }
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.skillDescriptionToken = "Start off with some lunar items";
                mySkillDef.skillName = "VT_SCAVSTASHLUNAR_MISC";
                mySkillDef.skillNameToken = "Scavenger's Stash - Devotee";
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.BaseState));

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;

                passiveFamily.variants[3] =
                new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",

                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                };
            }


            PlayerCharacterMasterController.onPlayerAdded += PlayerCharacterMasterController_onPlayerAdded;

            //PlayerCharacterMasterController.onPlayerAdded += (master) => { master.master.onBodyDeath.AddListener(() => {}); };
        }

        private void PlayerCharacterMasterController_onPlayerAdded(PlayerCharacterMasterController obj)
        {

            obj.master.onBodyStart += Master_onBodyStart;

        }

        private void Master_onBodyStart(CharacterBody obj)
        {

            GenericSkill[] skills = obj.GetComponents<GenericSkill>();
            if (skills[skills.Count() - 1].skillDef.skillName == "VT_SCAVSTASHHEALING_MISC")
            {
                obj.inventory.GiveItem(ItemIndex.Tooth, 3);
                obj.inventory.GiveItem(ItemIndex.HealOnCrit, 1);
            }
            if (skills[skills.Count() - 1].skillDef.skillName == "VT_SCAVSTASHMOVEMENT_MISC")
            {
                obj.inventory.GiveItem(ItemIndex.SprintBonus, 3);
                obj.inventory.GiveItem(ItemIndex.SprintOutOfCombat, 1);
            }
            if (skills[skills.Count() - 1].skillDef.skillName == "VT_SCAVSTASHATTACKSPEED_MISC")
            {
                obj.inventory.GiveItem(ItemIndex.Syringe, 3);
                obj.inventory.GiveItem(ItemIndex.EnergizedOnEquipmentUse, 1);
            }
            if (skills[skills.Count() - 1].skillDef.skillName == "VT_SCAVSTASHLUNAR_MISC")
            {
                obj.inventory.GiveItem(ItemIndex.LunarPrimaryReplacement, 2);
                obj.inventory.GiveItem(ItemIndex.LunarTrinket, 1);
                obj.inventory.SetEquipmentIndex(EquipmentIndex.CrippleWard);
            }

            obj.master.onBodyStart -= Master_onBodyStart;
        }

        private void skillsetup()
        {

            myCharacter.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill()
            {
                skillNameToken = "Blubber",
                enabled = true,
                skillDescriptionToken = "You are immune to being frozen or stunned!"
            };

            LoadoutAPI.AddSkill(typeof(VTStates.States.Scavenger.PrepSack));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Scavenger.PrepSack));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 16;
                mySkillDef.baseRechargeInterval = 1f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = false;
                mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = true;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 16;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 16;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Throw a stack of stashed thqwibs, they activate any on-kill items you have.";
                mySkillDef.skillName = "VT_SCAVSTASH_SPECIAL";
                mySkillDef.skillNameToken = "Stashed Friends";

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                skillFamily.variants[0] =
                    new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                    };
            }

            LoadoutAPI.AddSkill(typeof(VTStates.States.Scavenger.ScavFlamethrower));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Scavenger.ScavFlamethrower));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 0f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = true;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = true;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0.5f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Shoot a powerful shotgun blast for 8 x 100% damage, Accuracy increases with fire rate.";
                mySkillDef.skillName = "VT_SCAVSHOT_PRIMARY";
                mySkillDef.skillNameToken = "Compressed Air Shotgun";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.primary.skillFamily;


                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };

            }


            LoadoutAPI.AddSkill(typeof(VTStates.States.Scavenger.ScavengerShotgun));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Scavenger.ScavengerShotgun));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 3;
                mySkillDef.baseRechargeInterval = 4f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 3;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 3;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Shoot a powerful shotgun blast";
                mySkillDef.skillName = "VT_SCAVSHOT_SECONDARY";
                mySkillDef.skillNameToken = "Soupcan shotgun";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.secondary.skillFamily;


                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };

            }

            LoadoutAPI.AddSkill(typeof(VTStates.States.Scavenger.ScavengerLeap));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Scavenger.ScavengerLeap));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 4f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Propel yourself forwards";
                mySkillDef.skillName = "VT_SCAVLEAP_UTILITY";
                mySkillDef.skillNameToken = "Leap";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.utility.skillFamily;


                skillFamily.variants[0] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };

            }
        }

        #endregion skills


    }

    namespace VTStates.States.Scavenger
    {
        public class ScavengerLeap : BaseState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                Vector3 direction = base.GetAimRay().direction;
                if (base.isAuthority)
                {
                    base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = (base.GetAimRay().direction + Vector3.up) * 20f;
                }
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                base.GetModelTransform().GetComponent<AimAnimator>().enabled = true;
                base.PlayCrossfade("Gesture, Override", "Leap", 0.1f);
                base.PlayCrossfade("Gesture, AdditiveHigh", "Leap", 0.1f);
                base.PlayCrossfade("Gesture, Override", "Leap", 0.1f);

                base.OnExit();
            }
        }

        public class ScavengerShotgun : EntityStates.ScavMonster.FireEnergyCannon
        {
            public override void OnEnter()
            {
                EntityStates.ScavMonster.FireEnergyCannon.maxRefireCount = 1;
                EntityStates.ScavMonster.FireEnergyCannon.projectileCount = 12;
                base.OnEnter();
                EntityStates.ScavMonster.FireEnergyCannon.maxRefireCount = 3;
                EntityStates.ScavMonster.FireEnergyCannon.projectileCount = 3;
            }

        }

        public class PrepSack : EntityStates.ScavMonster.PrepSack
        {
            public override InterruptPriority GetMinimumInterruptPriority()
            {
                return InterruptPriority.PrioritySkill;
            }
            public override void OnEnter()
            {

                base.OnEnter();

            }

        }
        public class EnterSitSummon : BaseState
        {
            // Token: 0x06002C7D RID: 11389 RVA: 0x000C06E4 File Offset: 0x000BE8E4
            public override void OnEnter()
            {

                base.OnEnter();
                duration = EnterSitSummon.baseDuration / attackSpeedStat;
                //Util.PlaySound(EnterSitSummon.soundString, base.gameObject);
                base.PlayCrossfade("Body", "EnterSit", "Sit.playbackRate", duration, 0.1f);
                base.modelLocator.normalizeToFloor = true;
                base.modelLocator.modelTransform.GetComponent<AimAnimator>().enabled = true;
            }

            // Token: 0x06002C7E RID: 11390 RVA: 0x00021758 File Offset: 0x0001F958
            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (base.fixedAge >= duration)
                {
                    outer.SetNextState(new FindSummon());
                }
            }

            // Token: 0x0400288D RID: 10381
            public static float baseDuration = 0.5f;

            // Token: 0x0400288F RID: 10383
            private float duration;
        }




        public class FindSummon : BaseState
        {
            // Token: 0x06002C6D RID: 11373 RVA: 0x000C042C File Offset: 0x000BE62C
            public override void OnEnter()
            {
                base.OnEnter();
                duration = FindSummon.baseDuration / attackSpeedStat;
                base.PlayCrossfade("Body", "SitRummage", "Sit.playbackRate", duration, 0.1f);


            }

            // Token: 0x06002C6E RID: 11374 RVA: 0x0002168E File Offset: 0x0001F88E
            public override void OnExit()
            {
                if (base.isAuthority)
                {
                    base.characterBody.SendConstructTurret(base.characterBody, base.characterBody.corePosition + (3f * Vector3.up), base.characterBody.gameObject.transform.rotation, MasterCatalog.FindMasterIndex("RoboBallMiniMaster"));
                }

                base.OnExit();
            }

            // Token: 0x06002C6F RID: 11375 RVA: 0x000C060C File Offset: 0x000BE80C
            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (base.fixedAge >= duration && base.isAuthority)
                {
                    outer.SetNextState(new ExitSitSummon());
                }
            }

            // Token: 0x0400287B RID: 10363
            public static float baseDuration = 2.5f;

            // Token: 0x04002883 RID: 10371
            private float duration;
        }

        public class ExitSitSummon : BaseState
        {
            // Token: 0x06002C83 RID: 11395 RVA: 0x000C077C File Offset: 0x000BE97C
            public override void OnEnter()
            {
                base.OnEnter();
                duration = ExitSitSummon.baseDuration / attackSpeedStat;
                Util.PlaySound(ExitSitSummon.soundString, base.gameObject);
                base.PlayCrossfade("Body", "ExitSit", "Sit.playbackRate", duration, 0.1f);
                base.modelLocator.normalizeToFloor = false;
            }

            // Token: 0x06002C84 RID: 11396 RVA: 0x000217D7 File Offset: 0x0001F9D7
            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (base.fixedAge >= duration)
                {
                    outer.SetNextStateToMain();
                }
            }

            // Token: 0x06002C85 RID: 11397 RVA: 0x000217F8 File Offset: 0x0001F9F8
            public override void OnExit()
            {
                if (NetworkServer.active && base.characterBody && base.characterBody.HasBuff(BuffIndex.ArmorBoost))
                {
                    base.characterBody.RemoveBuff(BuffIndex.ArmorBoost);
                }
                base.OnExit();
                outer.SetNextStateToMain();
            }

            // Token: 0x04002892 RID: 10386
            public static float baseDuration = 0.5f;

            // Token: 0x04002893 RID: 10387
            public static string soundString;

            // Token: 0x04002894 RID: 10388
            private float duration;
        }


        // Token: 0x020007DB RID: 2011
        public class ScavFlamethrower : BaseState
        {
            // Token: 0x06002DC8 RID: 11720 RVA: 0x000C2978 File Offset: 0x000C0B78
            public override void OnEnter()
            {
                flamethrowerEffectPrefab = EntityStates.Mage.Weapon.Flamethrower.impactEffectPrefab;

                base.OnEnter();
                stopwatch = 0f;
                duration = 1f / attackSpeedStat;
                Transform modelTransform = base.GetModelTransform();
                if (base.characterBody)
                {
                    base.characterBody.SetAimTimer(duration + 1f);
                }
                if (modelTransform)
                {
                    childLocator = modelTransform.GetComponent<ChildLocator>();
                    Transform transform = childLocator.FindChild(EnergyCannonState.muzzleName);

                    float num = flamethrowerDuration * 2f;
                    tickDamageCoefficient = 1f / num;

                    if (base.isAuthority && base.characterBody)
                    {

                        base.PlayAnimation("Body", "FireEnergyCannon", "FireEnergyCannon.playbackRate", duration);

                        if (transform)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                Ray aimRay = base.GetAimRay();
                                if (base.isAuthority)
                                {
                                    isCrit = Util.CheckRoll(critStat, base.characterBody.master);

                                    new BulletAttack
                                    {
                                        owner = base.gameObject,

                                        weapon = transform.gameObject,
                                        origin = aimRay.origin,
                                        aimVector = aimRay.direction,
                                        minSpread = 0f,
                                        maxSpread = 25f / attackSpeedStat,
                                        damage = 1f * damageStat,
                                        force = 25f,
                                        muzzleName = EnergyCannonState.muzzleName,
                                        hitEffectPrefab = EntityStates.Mage.Weapon.Flamethrower.impactEffectPrefab,
                                        tracerEffectPrefab = flamethrowerEffectPrefab,
                                        isCrit = isCrit,
                                        radius = 2f,
                                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                                        stopperMask = LayerIndex.world.mask,
                                        procCoefficient = 0.25f,
                                        maxDistance = maxDistance * attackSpeedStat,
                                        smartCollision = true,
                                        damageType = (Util.CheckRoll(5f, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic)
                                    }.Fire();

                                }
                            }
                        }

                    }
                }


            }

            public override void FixedUpdate()
            {
                base.FixedUpdate();
                if (base.fixedAge > duration || !base.isAuthority)
                {
                    OnExit();
                    return;
                }
            }

            public override void OnExit()
            {
                outer.SetNextStateToMain();
                base.OnExit();
            }
            public override InterruptPriority GetMinimumInterruptPriority()
            {
                return InterruptPriority.Death;
            }


            // Token: 0x04002AAC RID: 10924
            [SerializeField]
            public GameObject flamethrowerEffectPrefab;

            // Token: 0x04002AAD RID: 10925
            public static GameObject impactEffectPrefab;

            // Token: 0x04002AAE RID: 10926
            public static GameObject tracerEffectPrefab;

            // Token: 0x04002AAF RID: 10927
            [SerializeField]
            public float maxDistance = 100f;

            // Token: 0x04002AB0 RID: 10928
            public static float radius;

            // Token: 0x04002AB1 RID: 10929
            public static float baseEntryDuration = 1f;

            // Token: 0x04002AB2 RID: 10930
            public static float baseFlamethrowerDuration = 2f;

            // Token: 0x04002AB3 RID: 10931
            public static float totalDamageCoefficient = 1.2f;

            // Token: 0x04002AB4 RID: 10932
            public static float procCoefficientPerTick;

            // Token: 0x04002AB5 RID: 10933
            public static float tickFrequency;

            // Token: 0x04002AB6 RID: 10934
            public static float force = 20f;

            // Token: 0x04002AB7 RID: 10935
            public static string startAttackSoundString;

            // Token: 0x04002AB8 RID: 10936
            public static string endAttackSoundString;

            // Token: 0x04002AB9 RID: 10937
            public static float ignitePercentChance;

            // Token: 0x04002ABA RID: 10938
            public static float recoilForce;

            // Token: 0x04002ABB RID: 10939
            private float tickDamageCoefficient;

            // Token: 0x04002ABC RID: 10940
            private readonly float flamethrowerStopwatch;

            // Token: 0x04002ABD RID: 10941
            private float stopwatch;

            // Token: 0x04002ABE RID: 10942
            private float duration;

            // Token: 0x04002ABF RID: 10943
            private readonly float flamethrowerDuration;

            // Token: 0x04002AC0 RID: 10944
            private readonly bool hasBegunFlamethrower;

            // Token: 0x04002AC1 RID: 10945
            private ChildLocator childLocator;

            // Token: 0x04002AC6 RID: 10950
            private bool isCrit;

            // Token: 0x04002AC7 RID: 10951
            private const float flamethrowerEffectBaseDistance = 16f;
        }
    }

}