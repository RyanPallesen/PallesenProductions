
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using EntityStates.Pounder;
using EntityStates.Treebot.Weapon;
using PallesenProductions;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VTStates.States.Engineer;

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
                EntityStates.Mage.Weapon.FireLaserbolt.force = -600f;

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
                mySkillDef.stockToConsume = 1;
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
            {
                LoadoutAPI.AddSkill(typeof(SummonPounder));
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(SummonPounder));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 2;
                mySkillDef.baseRechargeInterval = 15f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = false;
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
                mySkillDef.skillDescriptionToken = "Summon a pounder that periodically damages enemies and pulls them towards itself";
                mySkillDef.skillName = "EXPANDEDSKILLS_SUMMONPOUNDER_SECONDARY";
                mySkillDef.skillNameToken = "Leftover Mining Equipment";

                LoadoutAPI.AddSkillDef(mySkillDef);

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/engibody");
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

            if (NetworkServer.active)
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

            base.OnExit();
            base.outer.SetNextStateToMain();


        }
    }

    public class TrueShield : BaseState
    {
        private bool initialized = false;
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active)
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

            base.OnExit();
            base.outer.SetNextStateToMain();

        }
    }

    public class SummonPounder : BaseState
    {
        public float baseDuration = CreatePounder.baseDuration;

        public float damageCoefficient = 1f;
 
        public float maxDistance = CreatePounder.maxDistance;

        public float maxSlopeAngle = CreatePounder.maxSlopeAngle;

        private float duration;

        private float stopwatch;

        private bool goodPlacement;

        private GameObject areaIndicatorInstance;

        private GameObject cachedCrosshairPrefab;

        private GameObject editedPounder;

        private SkillDef cacheSkill;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = CreatePounder.baseDuration / this.attackSpeedStat;
            this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
            base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.duration);
            Util.PlaySound(CreatePounder.prepSoundString, base.gameObject);
            cacheSkill = base.skillLocator.secondary.skillDef;
            this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(CreatePounder.areaIndicatorPrefab);
            this.UpdateAreaIndicator();
        }

        private void UpdateAreaIndicator()
        {
            this.goodPlacement = false;

            if (this.areaIndicatorInstance)
            { 
                this.areaIndicatorInstance.SetActive(true);
                float num = CreatePounder.maxDistance;
                float num2 = 0f;
                RaycastHit raycastHit;
                if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(base.GetAimRay(), base.gameObject, out num2), out raycastHit, num + num2, LayerIndex.world.mask))
                {
                    this.areaIndicatorInstance.transform.position = raycastHit.point;
                    this.areaIndicatorInstance.transform.up = raycastHit.normal;
                    this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < CreatePounder.maxSlopeAngle);
                }

                base.characterBody.crosshairPrefab = (this.goodPlacement ? CreatePounder.goodCrosshairPrefab : CreatePounder.badCrosshairPrefab);
                this.areaIndicatorInstance.SetActive(this.goodPlacement);
            }
        }

        public override void Update()
        {
            base.Update();
            this.UpdateAreaIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= this.duration && !base.inputBank.skill2.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (this.goodPlacement)
            {
                Util.PlaySound(CreatePounder.fireSoundString, base.gameObject);
                if (this.areaIndicatorInstance && NetworkServer.active)
                {
                    if(editedPounder == null)
                    {
                        editedPounder = CreatePounder.projectilePrefab;
                        editedPounder.AddComponent<PounderSpawnOnAwake>().stocks = base.skillLocator.secondary.stock;
                    }

                    base.skillLocator.secondary.RemoveAllStocks();

                    bool crit = Util.CheckRoll(this.critStat, base.characterBody.master);
                    ProjectileManager.instance.FireProjectile(CreatePounder.projectilePrefab, this.areaIndicatorInstance.transform.position, Quaternion.identity, base.gameObject, this.damageStat * 0.8f, 0f, crit, DamageColorIndex.Default, null, -1f);
                }
            }

            EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
            base.OnExit();

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}

public class PoundEdited : Pound
{
    // Token: 0x06003511 RID: 13585
    public override void OnEnter()
    {

        base.OnEnter();
        this.projectileDamage = base.GetComponent<ProjectileDamage>();
        blastFrequency = base.projectileController.owner.GetComponent<CharacterBody>().attackSpeed;
    }

    // Token: 0x06003512 RID: 13586 RVA: 0x000D8DB0 File Offset: 0x000D6FB0
    public override void FixedUpdate()
    {
        fixedAge += Time.fixedDeltaTime;
        this.poundTimer -= Time.fixedDeltaTime;
        if (this.poundTimer <= 0f && base.projectileController.owner)
        {
            this.poundTimer += 1f / blastFrequency;
            if (NetworkServer.active)
            {
                new BlastAttack
                {
                    attacker = base.projectileController.owner,
                    baseDamage = this.projectileDamage.damage,
                    baseForce = blastForce,
                    crit = this.projectileDamage.crit,
                    damageType = this.projectileDamage.damageType,
                    falloffModel = BlastAttack.FalloffModel.None,
                    position = base.transform.position,
                    radius = blastRadius + blastRadiusMod,
                    teamIndex = base.projectileController.teamFilter.teamIndex
                }.Fire();
                EffectManager.SpawnEffect(Pound.blastEffectPrefab, new EffectData
                {
                    origin = base.transform.position,
                    scale = blastRadius + blastRadiusMod
                }, true);
            }
            base.PlayAnimation("Base", "Pound");
        }
        if (NetworkServer.active && base.fixedAge > duration)
        {
            EntityState.Destroy(base.gameObject);
        }
    }

    // Token: 0x04002DB9 RID: 11705
    public new float blastRadius = 20f;
    public float blastRadiusMod = 1f;

    // Token: 0x04002DBA RID: 11706
    public new float blastProcCoefficient = 0.1f;

    // Token: 0x04002DBB RID: 11707
    public new float blastForce = -1500f;

    // Token: 0x04002DBC RID: 11708
    public new float blastFrequency;

    // Token: 0x04002DBD RID: 11709
    public new float duration = 5f;

    // Token: 0x04002DBF RID: 11711
    private ProjectileDamage projectileDamage;

    // Token: 0x04002DC0 RID: 11712
    private float poundTimer;
}

public class PounderSpawnOnAwake : MonoBehaviour
{
    public int stocks = 0;
    public void OnEnable()
    {
        GetComponentInChildren<EntityStateMachine>().SetNextState(new PoundEdited() { blastRadiusMod = stocks });
    }
}
//[Info: Unity Log]
//TreebotPounderProjectile(UnityEngine.Networking.NetworkIdentity)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.Projectile.ProjectileController)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.Projectile.ProjectileNetworkTransform)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.Projectile.ProjectileDamage)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.TeamFilter)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.EntityStateMachine)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.NetworkStateMachine)
//[Info: Unity Log]
//TreebotPounderProjectile(RoR2.ModelLocator)
//[Info: Unity Log]
//mdlTreebotPounder(ChildLocator)
//[Info: Unity Log] AreaIndicator(RoR2.AnimateShaderAlpha)