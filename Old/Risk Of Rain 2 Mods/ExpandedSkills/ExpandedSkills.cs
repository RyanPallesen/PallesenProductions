using BepInEx;
using BepInEx.Configuration;
using EntityStates;
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
using TMPro;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PallesenProductions
{
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.ExpandedSkills", "ExpandedSkills", "1.2.0")]

    public class ExpandedSkills : BaseUnityPlugin
    {
        //Completely re-wrote and re-networked the summoning skills
        //slightly rebalanced health and damage on summoning skills
        //added new huntress skill
        public void Awake()
        {


            #region huntress
            LoadoutAPI.AddSkill(typeof(VTStates.States.Huntress.JumpingArrow));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Huntress.JumpingArrow));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 0.5f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = true;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Launch a weak arrow that bounces between targets";
                mySkillDef.skillName = "EXPANDEDSKILLS_JUMPINGARROW_PRIMARY";
                mySkillDef.skillNameToken = "Jumping Arrow";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/huntressbody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.primary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }
            LoadoutAPI.AddSkill(typeof(VTStates.States.Huntress.MultiArrow));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Huntress.MultiArrow));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 3;
                mySkillDef.baseRechargeInterval = 2f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 3;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 3;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Launch multiple arrows that splinter between targets on impact";
                mySkillDef.skillName = "EXPANDEDSKILLS_MULTIARROW_PRIMARY";
                mySkillDef.skillNameToken = "Multi Arrow";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/huntressbody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.secondary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }
            #endregion huntress

            #region acrid
            LoadoutAPI.AddSkill(typeof(VTStates.States.Acrid.SummonBrood));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Acrid.SummonBrood));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 40f;
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
                mySkillDef.skillDescriptionToken = "Summon a swarm of 4 miniature acrids that inherit your items";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONBROOD_SPECIAL";
                mySkillDef.skillNameToken = "Pack leader";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/crocobody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }

            #endregion acrid

            #region artificer

            LoadoutAPI.AddSkill(typeof(VTStates.States.Mage.FireSeekingProjectile));
            {

                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mage.FireSeekingProjectile));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 4;
                mySkillDef.baseRechargeInterval = 1.25f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Frozen;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Shoot a slightly weaker firebolt that homes";
                mySkillDef.skillName = "VT_FIRE_PRIMARY";
                mySkillDef.skillNameToken = "Homing Firebolt";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/magebody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.primary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "EQUIPMENT_BFG_NAME",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };

            }

            LoadoutAPI.AddSkill(typeof(VTStates.States.Mage.GravBomb));
            {

                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mage.GravBomb));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 20f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Throw a miniature black hole that sucks in enemies and explodes on first contact, dealing 1000% Damage to all nearby";
                mySkillDef.skillName = "VT_FIRE_SPECIAL";
                mySkillDef.skillNameToken = "Gravity Bomb";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/magebody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "EQUIPMENT_BFG_NAME",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };

            }
            #endregion artificer

            #region engineer
            LoadoutAPI.AddSkill(typeof(VTStates.States.Engineer.SummonDrone));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Engineer.SummonDrone));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 2;
                mySkillDef.baseRechargeInterval = 20f;
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
                mySkillDef.skillDescriptionToken = "Summon a drone that inherits your items";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONDRONE_SPECIAL";
                mySkillDef.skillNameToken = "Drone";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/engibody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }

            LoadoutAPI.AddSkill(typeof(VTStates.States.Engineer.SummonProbe));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Engineer.SummonProbe));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 2;
                mySkillDef.baseRechargeInterval = 40f;
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
                mySkillDef.skillDescriptionToken = "Summon a probe that inherits your items";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONPROBE_SPECIAL";
                mySkillDef.skillNameToken = "Probe";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/engibody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }
            LoadoutAPI.AddSkill(typeof(VTStates.States.Engineer.TrueShield));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Engineer.TrueShield));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 15f;
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
                mySkillDef.skillDescriptionToken = "Freeze yourself, become immune and heal 10% HP over 2 seconds";
                mySkillDef.skillName = "EXPANDEDSKILLS_TRUESHIELD_UTILITY";
                mySkillDef.skillNameToken = "Nanomachines, son";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/engibody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.utility.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }

            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Mage.Weapon.FireLaserbolt));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 8;
                mySkillDef.baseRechargeInterval = 1f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Shoot a high-powered laser beam";
                mySkillDef.skillName = "EXPANDEDSKILLS_BEAM_PRIMARY";
                mySkillDef.skillNameToken = "Augmented Beam";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/engibody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.primary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }


            #endregion engineer

            #region merc
            LoadoutAPI.AddSkill(typeof(VTStates.States.Mercenary.SummonClones));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mercenary.SummonClones));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 2;
                mySkillDef.baseRechargeInterval = 16f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Summon a shadow clone that inherits your items";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONCLONE_SPECIAL";
                mySkillDef.skillNameToken = "Shadow Clones";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/mercbody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }

            LoadoutAPI.AddSkill(typeof(VTStates.States.Mercenary.SummonManyClones));
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mercenary.SummonManyClones));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 8;
                mySkillDef.baseRechargeInterval = 8f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Summon a miniature clone that inherits your items";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONMANYCLONE_SPECIAL";
                mySkillDef.skillNameToken = "Swarming Clones";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/mercbody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.special.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
               
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }
            #endregion merc

            #region commando
            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Commando.CommandoWeapon.FireShotgun));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 6;
                mySkillDef.baseRechargeInterval = 2f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 6;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Fire a stock of 6 bullets";
                mySkillDef.skillName = "EXPANDEDSKILLS_POWERPISTOL_PRIMARY";
                mySkillDef.skillNameToken = "Six shooter";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/commandobody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.primary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }

            {
                SkillDef mySkillDef = new SkillDef();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Huntress.Weapon.FireArrowSnipe));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 2f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = true;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = true;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Shoot a powerful laser";
                mySkillDef.skillName = "EXPANDEDSKILLS_POWERPISTOL2_PRIMARY";
                mySkillDef.skillNameToken = "Pocket laser";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/commandobody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.secondary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
              
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

                };
            }
            #endregion commando
        }



        #region networking

        public const Int16 HandleId = 255;

        public class MyMessage : MessageBase
        {
            public NetworkInstanceId objectID;
            public int summonType;

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(objectID);
                writer.Write(summonType);
            }

            public override void Deserialize(NetworkReader reader)
            {
                objectID = reader.ReadNetworkId();
                summonType = reader.ReadInt32();
            }
        }

        public static void SendNetworkMessage(NetworkInstanceId myObjectID, int summoningType)
        {
            NetworkServer.SendToAll(HandleId, new MyMessage
            {
                objectID = myObjectID,
                summonType = summoningType
            });
        }

        [RoR2.Networking.NetworkMessageHandler(msgType = HandleId, client = true)]
        public static void HandleDropItem(NetworkMessage netMsg)
        {
            var MyMessage = netMsg.ReadMessage<MyMessage>();


            if (NetworkServer.active)
            {
                CharacterBody characterBody = ClientScene.FindLocalObject(MyMessage.objectID).GetComponent<CharacterBody>();
                CharacterMaster characterMaster;
                if (characterBody)
                {
                    {

                        characterMaster = new MasterSummon
                        {
                            masterPrefab = MasterCatalog.FindMasterPrefab("MercMonsterMaster"),
                            position = characterBody.footPosition + characterBody.transform.up,
                            rotation = characterBody.transform.rotation,
                            summonerBodyObject = null,
                            ignoreTeamMemberLimit = true,
                            teamIndexOverride = characterBody.teamComponent.teamIndex

                        }.Perform();



                        characterMaster.bodyPrefab = characterBody.master.bodyPrefab;
                        characterMaster.Respawn(characterMaster.GetBody().footPosition, Quaternion.identity);

                        characterMaster.inventory.CopyItemsFrom(characterBody.inventory);
                        characterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);

                        characterMaster.inventory.CopyEquipmentFrom(characterBody.inventory);

                        SkinDef[] skins = BodyCatalog.GetBodySkins(BodyCatalog.FindBodyIndex(characterMaster.GetBody()));

                        int skinIndex = (int)characterBody.skinIndex;

                        CharacterModel mymodel = characterMaster.GetBody().modelLocator.modelTransform.GetComponentInChildren<CharacterModel>();

                        characterMaster.loadout.bodyLoadoutManager.SetSkinIndex(BodyCatalog.FindBodyIndex(characterMaster.GetBody()), (uint)skinIndex);
                        skins[skinIndex].Apply(mymodel.gameObject);



                        //Array.Resize(ref skins[skinIndex].gameObjectActivations, skins[skinIndex].gameObjectActivations.Length + 1);
                        //skins[skinIndex].gameObjectActivations[skins[skinIndex].gameObjectActivations.Length - 1] = new SkinDef.GameObjectActivation() { gameObject = mymodel.gameObject, shouldActivate = true };

                        skins[skinIndex].Apply(characterMaster.GetBody().modelLocator.modelTransform.GetComponent<CharacterModel>().gameObject);

                        if (MyMessage.summonType == 1) // brood
                        {

                            characterMaster.GetBody().modelLocator.modelBaseTransform.transform.localScale *= 0.5f;
                            characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 16);

                            characterMaster.GetBody().baseMaxHealth /= 4;
                            characterMaster.GetBody().levelMaxHealth /= 4;
                            characterMaster.GetBody().baseDamage /= 4;
                            characterMaster.GetBody().levelDamage /= 4;

                        }
                        else if (MyMessage.summonType == 2) // many clones
                        {

                            characterMaster.GetBody().modelLocator.modelBaseTransform.transform.localScale *= 0.5f;
                            characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 8);
                            characterMaster.GetBody().baseMaxHealth /= 8;
                            characterMaster.GetBody().levelMaxHealth /= 8;
                            characterMaster.GetBody().baseDamage /= 8;
                            characterMaster.GetBody().levelDamage /= 8;
                        }
                        else // normal 2 clones
                        {
                            characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 16);
                            characterMaster.GetBody().baseMaxHealth /= 4;
                            characterMaster.GetBody().levelMaxHealth /= 4;
                            characterMaster.GetBody().baseDamage /= 4;
                            characterMaster.GetBody().levelDamage /= 4;
                        }
                    }
                }
            }

        }
        #endregion networking
    }


}


#region entitystates

namespace VTStates.States.Engineer
{
    public class SummonDrone : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.characterBody.SendConstructTurret(base.characterBody, base.characterBody.corePosition + Vector3.up, Quaternion.identity, MasterCatalog.FindMasterIndex("DroneBackupMaster"));

            base.OnExit();
        }
    }

    public class SummonProbe : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.characterBody.SendConstructTurret(base.characterBody, base.characterBody.corePosition + Vector3.up, Quaternion.identity, MasterCatalog.FindMasterIndex("RoboBallMiniMaster"));

            base.OnExit();
        }
    }

    public class TrueShield : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            GameObject newObject = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("prefabs/projectiles/engibubbleshield"), base.characterBody.transform);
            newObject.layer = 11;
            base.characterBody.healthComponent.RechargeShieldFull();
            base.characterBody.GetComponent<SetStateOnHurt>().SetFrozen(2f);
            base.characterBody.AddTimedBuff(BuffIndex.Immune, 2f);
            base.characterBody.AddTimedBuff(BuffIndex.EngiShield, 2f);
            base.characterBody.healthComponent.HealFraction(0.2f, new ProcChainMask());

            base.OnExit();
        }
    }
}

namespace VTStates.States.Acrid
{
    public class SummonBrood : EntityStates.BeetleQueenMonster.SpawnState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            for (int i = 0; i < 4; i++)
            {
                ExpandedSkills.SendNetworkMessage(base.characterBody.networkIdentity.netId, 1);
            }

            base.OnExit();
        }
    }
}

namespace VTStates.States.Mercenary
{
    public class SummonClones : EntityStates.Commando.CommandoWeapon.CastSmokescreen
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.OnEnter();

            //for (int i = 0; i < 4; i++)
            {
                ExpandedSkills.SendNetworkMessage(base.characterBody.networkIdentity.netId, 3);
            }

            base.OnExit();
        }


    }

    public class SummonManyClones : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            //for (int i = 0; i < 4; i++)
            {
                ExpandedSkills.SendNetworkMessage(base.characterBody.networkIdentity.netId, 2);
            }

            base.OnExit();
        }

    }
}
namespace VTStates.States.Mage
{

    public class FireSeekingProjectile : BaseState
    {
        public override void OnEnter()
        {


            base.OnEnter();
            stopwatch = 0f;
            duration = 1 / attackSpeedStat;
            //Util.PlayScaledSound(attackSoundString, base.gameObject, attackSoundPitch);
            base.characterBody.SetAimTimer(2f);
            animator = base.GetModelAnimator();
            if (animator)
            {
                childLocator = animator.GetComponent<ChildLocator>();
            }
            EntityStates.Mage.Weapon.FireFireBolt.Gauntlet gauntlet = this.gauntlet;
            if (gauntlet != EntityStates.Mage.Weapon.FireFireBolt.Gauntlet.Left)
            {
                if (gauntlet != EntityStates.Mage.Weapon.FireFireBolt.Gauntlet.Right)
                {
                    return;
                }
                muzzleString = "MuzzleRight";
                if (attackSpeedStat < EntityStates.Mage.Weapon.FireFireBolt.attackSpeedAltAnimationThreshold)
                {
                    base.PlayCrossfade("Gesture, Additive", "Cast1Right", "FireGauntlet.playbackRate", duration, 0.1f);
                    base.PlayAnimation("Gesture Left, Additive", "Empty");
                    base.PlayAnimation("Gesture Right, Additive", "Empty");
                    return;
                }
                base.PlayAnimation("Gesture Right, Additive", "FireGauntletRight", "FireGauntlet.playbackRate", duration);
                base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", duration);
                FireGauntlet();
                return;
            }
            else
            {
                muzzleString = "MuzzleLeft";
                if (attackSpeedStat < EntityStates.Mage.Weapon.FireFireBolt.attackSpeedAltAnimationThreshold)
                {
                    base.PlayCrossfade("Gesture, Additive", "Cast1Left", "FireGauntlet.playbackRate", duration, 0.1f);
                    base.PlayAnimation("Gesture Left, Additive", "Empty");
                    base.PlayAnimation("Gesture Right, Additive", "Empty");
                    return;
                }
                base.PlayAnimation("Gesture Left, Additive", "FireGauntletLeft", "FireGauntlet.playbackRate", duration);
                base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", duration);
                FireGauntlet();
                return;
            }
        }

        // Token: 0x06002DA7 RID: 11687 RVA: 0x000B1899 File Offset: 0x000AFA99
        public override void OnExit()
        {
            base.OnExit();
        }

        // Token: 0x06002DA8 RID: 11688 RVA: 0x000C1E10 File Offset: 0x000C0010
        private void FireGauntlet()
        {
            if (hasFiredGauntlet)
            {
                return;
            }
            base.characterBody.AddSpreadBloom(EntityStates.Mage.Weapon.FireFireBolt.bloom);
            hasFiredGauntlet = true;
            Ray aimRay = base.GetAimRay();
            if (childLocator)
            {
                muzzleTransform = childLocator.FindChild(muzzleString);
            }
            //if (muzzleflashEffectPrefab)
            {
                //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, muzzleString, false);
            }
            if (base.isAuthority)
            {
                //Debug.Log(projectilePrefab);
                if (!projectilePrefab)
                {
                    projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/magefireboltbasic").InstantiateClone("magefireboltcopy");
                }
                //Debug.Log(" 1 " + projectilePrefab);

                TeamFilter filter = projectilePrefab.GetComponent<TeamFilter>();
                ProjectileTargetComponent targeter = projectilePrefab.GetComponent<ProjectileTargetComponent>();
                ProjectileSphereTargetFinder targetfinder = projectilePrefab.GetComponent<ProjectileSphereTargetFinder>();
                ProjectileSteerTowardTarget steertoward = projectilePrefab.GetComponent<ProjectileSteerTowardTarget>();
                ProjectileSimple projectile = projectilePrefab.GetComponent<ProjectileSimple>();

                //Debug.Log("Got all");

                if (!filter) { filter = projectilePrefab.AddComponent<TeamFilter>(); }
                //Debug.Log("Got filter");

                if (!targeter) { targeter = projectilePrefab.AddComponent<ProjectileTargetComponent>(); }
                //Debug.Log("Got targeter");

                if (!targetfinder) { targetfinder = projectilePrefab.AddComponent<ProjectileSphereTargetFinder>(); }
                //Debug.Log("Got targetfinder");

                if (!steertoward) { steertoward = projectilePrefab.AddComponent<ProjectileSteerTowardTarget>(); }
                //Debug.Log("Got steertoward");

                if (!projectile) { projectile = projectilePrefab.AddComponent<ProjectileSimple>(); }
                //Debug.Log("Got projectile");

                targetfinder.lookRange = 5;
                steertoward.rotationSpeed = 360f;
                projectile.velocity = 50f;
                projectile.updateAfterFiring = true;
                targetfinder.onlySearchIfNoTarget = false;

                //foreach(MonoBehaviour behaviour in projectilePrefab.GetComponents<MonoBehaviour>())
                //{
                //    Debug.Log(behaviour);
                //}
                //Debug.Log("Prefab = " + projectilePrefab);

                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, 0.25f * damageStat, 0f, base.characterBody.RollCrit());
            }
        }

        // Token: 0x06002DA9 RID: 11689 RVA: 0x000C1EF0 File Offset: 0x000C00F0
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (animator.GetFloat("FireGauntlet.fire") > 0f && !hasFiredGauntlet)
            {
                FireGauntlet();
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        // Token: 0x06002DAA RID: 11690 RVA: 0x0000B933 File Offset: 0x00009B33
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        // Token: 0x06002DAB RID: 11691 RVA: 0x000C1F49 File Offset: 0x000C0149
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)gauntlet);
        }

        // Token: 0x06002DAC RID: 11692 RVA: 0x000C1F5F File Offset: 0x000C015F
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            gauntlet = (EntityStates.Mage.Weapon.FireFireBolt.Gauntlet)reader.ReadByte();
        }

        private GameObject projectilePrefab;
        // Token: 0x04002A62 RID: 10850
        private float stopwatch;

        // Token: 0x04002A63 RID: 10851
        private float duration;

        // Token: 0x04002A64 RID: 10852
        private bool hasFiredGauntlet;

        // Token: 0x04002A65 RID: 10853
        private string muzzleString;

        // Token: 0x04002A66 RID: 10854
        private Transform muzzleTransform;

        // Token: 0x04002A67 RID: 10855
        private Animator animator;

        // Token: 0x04002A68 RID: 10856
        private ChildLocator childLocator;

        // Token: 0x04002A69 RID: 10857
        private EntityStates.Mage.Weapon.FireFireBolt.Gauntlet gauntlet;

    }



    public class GravBomb : EntityStates.Mage.Weapon.Flamethrower
    {

        public override void OnEnter()
        {

            base.OnEnter();
            float baseMaxDuration = 2f;
            float autoFireDuration = baseMaxDuration / attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture, Additive", "FireNovaBomb", "FireShotgun", autoFireDuration * 3f);
            base.PlayAnimation("Gesture, Override", "FireNovaBomb", "FireShotgun", autoFireDuration * 3f);

            bool isAuthority = base.isAuthority;
            if (isAuthority)
            {
                {
                    GameObject prefab = Resources.Load<GameObject>("Prefabs/Projectiles/gravsphere");

                    bool crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                    Vector3 position = base.characterBody.inputBank ? base.characterBody.inputBank.aimOrigin : base.transform.position;


                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        projectilePrefab = prefab,
                        position = position,
                        rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction),
                        owner = base.characterBody.gameObject,
                        damage = base.characterBody.damage * 5f,
                        force = 2000f,
                        crit = crit,
                        damageColorIndex = DamageColorIndex.Item,
                        target = null,
                        speedOverride = 32f,
                        useFuseOverride = true,
                        fuseOverride = 2f
                    };

                    ProjectileManager.instance.FireProjectile(info);
                }
                {


                    GameObject prefab = Resources.Load<GameObject>("Prefabs/Projectiles/magefirebombprojectile");

                    bool crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                    Vector3 position = base.characterBody.inputBank ? base.characterBody.inputBank.aimOrigin : base.transform.position;

                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        projectilePrefab = prefab,
                        position = position,
                        rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction),
                        owner = base.characterBody.gameObject,
                        damage = base.characterBody.damage * 10f,
                        force = 2000f,
                        crit = crit,
                        damageColorIndex = DamageColorIndex.Item,
                        target = null,
                        speedOverride = 32f,
                        useFuseOverride = true,
                        fuseOverride = 2f
                    };

                    ProjectileManager.instance.FireProjectile(info);
                }
            }
            base.OnExit();
        }
    }
}

namespace VTStates.States.Huntress
{
    // Token: 0x02000837 RID: 2103
    public class JumpingArrow : BaseState
    {
        // Token: 0x06002F97 RID: 12183 RVA: 0x000CBC18 File Offset: 0x000C9E18
        public override void OnEnter()
        {
            base.OnEnter();
            OnExit();
            if (!huntressTracker)
            {
                huntressTracker = GetComponent<HuntressTracker>();
                if (!huntressTracker)
                {
                    huntressTracker = base.gameObject.AddComponent<HuntressTracker>();
                }
            }

            if (huntressTracker && base.isAuthority)
            {
                initialOrbTarget = huntressTracker.GetTrackingTarget();
            }

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
                animator = modelTransform.GetComponent<Animator>();
            }

            if (initialOrbTarget && base.isAuthority && huntressTracker)
            {
                Util.PlayScaledSound(EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.attackSoundString, base.gameObject, attackSpeedStat);
                huntressTracker = base.GetComponent<HuntressTracker>();

                duration = EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.baseDuration / attackSpeedStat;
                base.PlayCrossfade("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);
                base.PlayCrossfade("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);

                if (base.characterBody)
                {
                    base.characterBody.SetAimTimer(duration + 1f);
                }
            }
        }

        // Token: 0x06002F98 RID: 12184 RVA: 0x000CBD2D File Offset: 0x000C9F2D
        public override void OnExit()
        {
            base.OnExit();
            outer.SetNextStateToMain();
            FireOrbArrow();
        }

        // Token: 0x06002F99 RID: 12185 RVA: 0x000CBD3C File Offset: 0x000C9F3C
        private void FireOrbArrow()
        {

            if (hasFiredArrow || !NetworkServer.active)
            {
                return;
            }

            GenericOrb lightningOrb = new GenericOrb();
            lightningOrb.lightningType = LightningOrb.LightningType.HuntressGlaive;
            lightningOrb.damageValue = base.characterBody.damage * 0.5f;
            lightningOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            lightningOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            lightningOrb.attacker = base.gameObject;
            lightningOrb.procCoefficient = EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.orbProcCoefficient;
            lightningOrb.bouncesRemaining = 4;
            lightningOrb.speed = 50;
            lightningOrb.bouncedObjects = new List<HealthComponent>();
            lightningOrb.range = 25;
            lightningOrb.damageCoefficientPerBounce = 1f;
            lightningOrb.SetFieldValue<bool>("canBounceOnSameTarget", true);
            lightningOrb.prefabPath = "prefabs/effects/orbeffects/arroworbeffect";



            HurtBox hurtBox = initialOrbTarget;
            if (hurtBox)
            {
                Transform transform = childLocator.FindChild(EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.muzzleString);
                EffectManager.SimpleMuzzleFlash(EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.muzzleflashEffectPrefab, base.gameObject, EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.muzzleString, true);
                lightningOrb.origin = transform.position;
                lightningOrb.target = hurtBox;
                OrbManager.instance.AddOrb(lightningOrb);

            }



        }

        // Token: 0x06002F9A RID: 12186 RVA: 0x000CBE1C File Offset: 0x000CA01C
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.deltaTime;
            if (animator.GetFloat("FireSeekingShot.fire") > 0f && initialOrbTarget && base.isAuthority && huntressTracker)
            {
                FireOrbArrow();
            }
            if (stopwatch > duration && base.isAuthority)
            {
                OnExit();
                return;
            }
        }

        // Token: 0x06002F9B RID: 12187 RVA: 0x0000B933 File Offset: 0x00009B33
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        // Token: 0x06002F9C RID: 12188 RVA: 0x000CBE80 File Offset: 0x000CA080
        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(initialOrbTarget));
        }

        // Token: 0x06002F9D RID: 12189 RVA: 0x000CBE94 File Offset: 0x000CA094
        public override void OnDeserialize(NetworkReader reader)
        {
            initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }


        // Token: 0x04002D51 RID: 11601
        private float duration;

        // Token: 0x04002D52 RID: 11602
        private float stopwatch;

        // Token: 0x04002D53 RID: 11603
        private ChildLocator childLocator;

        // Token: 0x04002D54 RID: 11604
        private HuntressTracker huntressTracker;

        // Token: 0x04002D55 RID: 11605
        private Animator animator;

        // Token: 0x04002D56 RID: 11606
        private readonly bool hasFiredArrow;

        // Token: 0x04002D57 RID: 11607
        private HurtBox initialOrbTarget;
    }
    // Token: 0x02000837 RID: 2103
    public class MultiArrow : BaseState
    {
        // Token: 0x06002F97 RID: 12183 RVA: 0x000CBC18 File Offset: 0x000C9E18
        public override void OnEnter()
        {
            base.OnEnter();
            OnExit();
            if (!huntressTracker)
            {
                huntressTracker = GetComponent<HuntressTracker>();
                if (!huntressTracker)
                {
                    huntressTracker = base.gameObject.AddComponent<HuntressTracker>();
                }
            }

            if (huntressTracker && base.isAuthority)
            {
                initialOrbTarget = huntressTracker.GetTrackingTarget();
            }

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
                animator = modelTransform.GetComponent<Animator>();
            }

            if (initialOrbTarget && base.isAuthority && huntressTracker)
            {
                Util.PlayScaledSound(EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.attackSoundString, base.gameObject, attackSpeedStat);
                huntressTracker = base.GetComponent<HuntressTracker>();

                duration = EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.baseDuration / attackSpeedStat;
                base.PlayCrossfade("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);
                base.PlayCrossfade("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);

                if (base.characterBody)
                {
                    base.characterBody.SetAimTimer(duration + 1f);
                }
            }
        }

        // Token: 0x06002F98 RID: 12184 RVA: 0x000CBD2D File Offset: 0x000C9F2D
        public override void OnExit()
        {
            base.OnExit();
            outer.SetNextStateToMain();
            FireOrbArrow();
        }

        // Token: 0x06002F99 RID: 12185 RVA: 0x000CBD3C File Offset: 0x000C9F3C
        private void FireOrbArrow()
        {

            if (hasFiredArrow || !NetworkServer.active)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                GenericOrb lightningOrb = new GenericOrb();
                lightningOrb.lightningType = LightningOrb.LightningType.HuntressGlaive;
                lightningOrb.damageValue = base.characterBody.damage * 1f;
                lightningOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                lightningOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                lightningOrb.attacker = base.gameObject;
                lightningOrb.procCoefficient = EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.orbProcCoefficient;
                lightningOrb.bouncesRemaining = 2;
                lightningOrb.targetsToFindPerBounce = 2;
                lightningOrb.speed = 50;
                lightningOrb.bouncedObjects = new List<HealthComponent>();
                lightningOrb.range = 25;
                lightningOrb.damageCoefficientPerBounce = 1f;
                lightningOrb.SetFieldValue<bool>("canBounceOnSameTarget", true);
                lightningOrb.prefabPath = "prefabs/effects/orbeffects/arroworbeffect";



                HurtBox hurtBox = initialOrbTarget;
                if (hurtBox)
                {
                    Transform transform = childLocator.FindChild(EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.muzzleString);
                    EffectManager.SimpleMuzzleFlash(EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.muzzleflashEffectPrefab, base.gameObject, EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.muzzleString, true);
                    lightningOrb.origin = transform.position;
                    lightningOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(lightningOrb);

                }
            }


        }

        // Token: 0x06002F9A RID: 12186 RVA: 0x000CBE1C File Offset: 0x000CA01C
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.deltaTime;
            if (animator.GetFloat("FireSeekingShot.fire") > 0f && initialOrbTarget && base.isAuthority && huntressTracker)
            {
                FireOrbArrow();
            }
            if (stopwatch > duration && base.isAuthority)
            {
                OnExit();
                return;
            }
        }

        // Token: 0x06002F9B RID: 12187 RVA: 0x0000B933 File Offset: 0x00009B33
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        // Token: 0x06002F9C RID: 12188 RVA: 0x000CBE80 File Offset: 0x000CA080
        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(initialOrbTarget));
        }

        // Token: 0x06002F9D RID: 12189 RVA: 0x000CBE94 File Offset: 0x000CA094
        public override void OnDeserialize(NetworkReader reader)
        {
            initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }


        // Token: 0x04002D51 RID: 11601
        private float duration;

        // Token: 0x04002D52 RID: 11602
        private float stopwatch;

        // Token: 0x04002D53 RID: 11603
        private ChildLocator childLocator;

        // Token: 0x04002D54 RID: 11604
        private HuntressTracker huntressTracker;

        // Token: 0x04002D55 RID: 11605
        private Animator animator;

        // Token: 0x04002D56 RID: 11606
        private readonly bool hasFiredArrow;

        // Token: 0x04002D57 RID: 11607
        private HurtBox initialOrbTarget;
    }

}

//this.PickNextTarget(this.target.transform.position); for artificer homing orbs
#endregion entitystates

public class GenericOrb : LightningOrb
{
    public string prefabPath;
    public bool canBounceOnSameTarget;
    public override void Begin()
    {

        base.duration = 0.1f;

        EffectData effectData = new EffectData
        {
            origin = origin,
            genericFloat = base.duration
        };
        if (target)
        {
            effectData.SetHurtBoxReference(target);
        }
        EffectManager.SpawnEffect(Resources.Load<GameObject>(prefabPath), effectData, true);
    }

    public override void OnArrival()
    {

        
        if (target)
        {
            for (int i = 0; i < targetsToFindPerBounce; i++)
            {
                if (bouncesRemaining > 0)
                {
                    if (canBounceOnSameTarget)
                    {
                        bouncedObjects.Clear();
                    }
                    if (bouncedObjects != null)
                    {
                        bouncedObjects.Add(target.healthComponent);
                    }
                    HurtBox hurtBox = PickNextTarget(target.transform.position);
                    if (hurtBox)
                    {
                        GenericOrb lightningOrb = new GenericOrb();
                        //lightningOrb.search = this.search;
                        lightningOrb.origin = target.transform.position;
                        lightningOrb.target = hurtBox;
                        lightningOrb.attacker = attacker;
                        lightningOrb.teamIndex = teamIndex;
                        lightningOrb.damageValue = damageValue * damageCoefficientPerBounce;
                        lightningOrb.bouncesRemaining = bouncesRemaining - 1;
                        lightningOrb.isCrit = isCrit;
                        lightningOrb.bouncedObjects = bouncedObjects;
                        lightningOrb.lightningType = lightningType;
                        lightningOrb.procChainMask = procChainMask;
                        lightningOrb.procCoefficient = procCoefficient;
                        lightningOrb.damageColorIndex = damageColorIndex;
                        lightningOrb.damageCoefficientPerBounce = damageCoefficientPerBounce;
                        lightningOrb.speed = speed;
                        lightningOrb.range = range;
                        lightningOrb.prefabPath = prefabPath;

                        OrbManager.instance.AddOrb(lightningOrb);
                    }
                }
            }

            HealthComponent healthComponent = target.healthComponent;
            if (healthComponent)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = damageValue;
                damageInfo.attacker = attacker;
                damageInfo.inflictor = null;
                damageInfo.force = Vector3.zero;
                damageInfo.crit = isCrit;
                damageInfo.procChainMask = procChainMask;
                damageInfo.procCoefficient = procCoefficient;
                damageInfo.position = target.transform.position;
                damageInfo.damageColorIndex = damageColorIndex;
                damageInfo.damageType = damageType;
                healthComponent.TakeDamage(damageInfo);
                GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
            }
        }
    }
}