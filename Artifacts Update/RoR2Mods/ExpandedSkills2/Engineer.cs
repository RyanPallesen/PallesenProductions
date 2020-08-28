
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using PallesenProductions;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ExpandedSkills2
{
    public static class Engineer
    {
        public static void Setup()
        {
            LoadoutAPI.AddSkill(typeof(VTStates.States.Engineer.SummonDrone));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
                mySkillDef.skillDescriptionToken = "Spawn a drone that <style=cIsUtility>inherits all your items.</style> Fires 4 shots for <style=cIsDamage>100% damage</style>. Can have up to 2.";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONDRONE_SPECIAL";
                mySkillDef.skillNameToken = "Backup Drone";

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
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
                mySkillDef.skillDescriptionToken = "Spawn a miniature probe that <style=cIsUtility>inherits all your items.</style> Fires a beam for <style=cIsDamage>100% damage</style>. Can have up to 2.";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONPROBE_SPECIAL";
                mySkillDef.skillNameToken = "TR-8R Probe";

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
            {


                EntityStates.Mage.Weapon.FireLaserbolt.damageCoefficient = 2f;
                EntityStates.Mage.Weapon.FireLaserbolt.force = -100f;

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Mage.Weapon.FireLaserbolt));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
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
                mySkillDef.stockToConsume = 0;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Fire a beam that deals <style=cIsDamage>200% damage</style> and <style=cIsUtility> Brings enemies towards you </style>";
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
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Engineer.MinorShield));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 3;
                mySkillDef.baseRechargeInterval = 8f;
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
                mySkillDef.stockToConsume = 0;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Give each ally shields equal to their maximum health that last 5 seconds and shock anyone nearby them. Each ally takes one stock of this skill.";
                mySkillDef.skillName = "EXPANDEDSKILLS_SHIELDALL_UTILITY";
                mySkillDef.skillNameToken = "Panic Button";

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
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Engineer.TrueShield));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 3;
                mySkillDef.baseRechargeInterval = 15f;
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
                mySkillDef.stockToConsume = 0;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Make all allies <style=cIsUtility>immune</style> for 3 seconds,  gain <style=cIsUtility>damage Reduction</style> for 8 seconds and <style=cIsUtility>regenerate their shields</style>. Each ally takes one stock of this skill.";
                mySkillDef.skillName = "EXPANDEDSKILLS_SHIELDALL_UTILITY";
                mySkillDef.skillNameToken = "Panic Button";

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
        }
    }
}

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


    public class MinorShield : BaseState
    {
        private bool initialized = false;
        public override void OnEnter()
        {
            base.OnEnter();

            if (base.teamComponent && NetworkServer.active)
            {
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(base.teamComponent.teamIndex);
                Vector3 position = base.transform.position;
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                    if (component)
                    {
                        if (base.characterBody.skillLocator.utility.stock > 0 && !component.HasBuff(BuffIndex.EngiTeamShield))
                        {
                            component.AddTimedBuff(BuffIndex.EngiTeamShield, 8f);
                            component.AddTimedBuff(BuffIndex.TeslaField, 2f);

                            HealthComponent component2 = component.GetComponent<HealthComponent>();
                            if (component2)
                            {
                                component2.RechargeShieldFull();
                            }

                            base.skillLocator.utility.DeductStock(1);
                        }
                    }


                }
            }



        }
    }

    public class TrueShield : BaseState
    {
        private bool initialized = false;
        public override void OnEnter()
        {
            base.OnEnter();

            if (base.teamComponent && NetworkServer.active)
            {
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(base.teamComponent.teamIndex);
                Vector3 position = base.transform.position;
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                    if (component)
                    {
                        if (base.characterBody.skillLocator.utility.stock > 0 && !component.HasBuff(BuffIndex.EngiShield) && !component.HasBuff(BuffIndex.ElephantArmorBoost))
                        {
                            component.AddTimedBuff(BuffIndex.EngiShield, 3f);
                            component.AddTimedBuff(BuffIndex.Immune, 3f);
                            component.AddTimedBuff(BuffIndex.ElephantArmorBoost, 6f);

                            HealthComponent component2 = component.GetComponent<HealthComponent>();
                            if (component2)
                            {
                                component2.RechargeShieldFull();
                            }

                            base.skillLocator.utility.DeductStock(1);
                        }
                    }


                }
            }



        }
    }
}