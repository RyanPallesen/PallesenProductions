using EntityStates;
using EntityStates.Huntress.Weapon;
using Microsoft.Win32;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ExpandedSkills2
{
    public static class Huntress
    {
        public static void Setup()
        {
            On.RoR2.Orbs.LightningOrb.Begin += LightningOrb_Begin;
            #region huntress
            LoadoutAPI.AddSkill(typeof(VTStates.States.Huntress.JumpingArrow));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
                mySkillDef.stockToConsume = 0;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "<style=cIsUtility>Agile</style>. Quickly fire a seeking arrow for <style=cIsDamage>100% damage</style>. It <style=cIsUtility>Bounces</style> twice";
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
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 0;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Launch an arrow that deals <style=cIsDamage>75% damage</style> and bounces between up to <style=cIsDamage>2</style> times, <style=cIsUtility>Doubling Damage and splitting into two each time</style>";
                mySkillDef.skillName = "EXPANDEDSKILLS_MULTIARROW_SECONDARY";
                mySkillDef.skillNameToken = "Splinter Shot";

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
            LoadoutAPI.AddSkill(typeof(VTStates.States.Huntress.ChargedArrow));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Huntress.ChargedArrow));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 4;
                mySkillDef.baseRechargeInterval = 4f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 0;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Charge an arrow for up to <style=cIsDamage>600% damage</style> per stock.";
                mySkillDef.skillName = "EXPANDEDSKILLS_CHARGED_SECONDARY";
                mySkillDef.skillNameToken = "Charged Shot";

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

        }

        private static void LightningOrb_Begin(On.RoR2.Orbs.LightningOrb.orig_Begin orig, LightningOrb self)
        {
            if (self.lightningType == (LightningOrb.LightningType)(-1))
            {
                EffectData effectData = new EffectData
                {
                    origin = self.origin,
                    genericFloat = 0.1f
                };
                effectData.SetHurtBoxReference(self.target);
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/orbeffects/arroworbeffect"), effectData, true);
            }
            else
            {
                orig(self);
            }
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
                base.OnEnter();

                initialOrbTarget = huntressTracker.GetTrackingTarget();


                Transform modelTransform = base.GetModelTransform();
                if (modelTransform)
                {
                    childLocator = modelTransform.GetComponent<ChildLocator>();
                    animator = modelTransform.GetComponent<Animator>();
                }

                if (initialOrbTarget && base.isAuthority && huntressTracker)
                {
                    Util.PlayScaledSound(EntityStates.Huntress.HuntressWeapon.FireArrow.attackSoundString, base.gameObject, attackSpeedStat);
                    huntressTracker = base.GetComponent<HuntressTracker>();

                    duration = 0.75f / attackSpeedStat;
                    base.PlayCrossfade("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);
                    base.PlayCrossfade("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);

                    if (base.characterBody)
                    {
                        base.characterBody.SetAimTimer(duration + 1f);
                    }
                }
                else
                {
                    base.skillLocator.secondary.AddOneStock();
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

            hasFiredArrow = true;

            LightningOrb lightningOrb = new LightningOrb();
            lightningOrb.lightningType = (LightningOrb.LightningType)(-1);
            lightningOrb.damageValue = base.characterBody.damage * 0.5f;
            lightningOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            lightningOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            lightningOrb.attacker = base.gameObject;
            lightningOrb.procCoefficient = 0.5f;
            lightningOrb.bouncesRemaining = 2;
            lightningOrb.speed = 55;
            lightningOrb.bouncedObjects = new List<HealthComponent>();
            lightningOrb.range = 100;
            lightningOrb.damageCoefficientPerBounce = 1f;
            lightningOrb.SetFieldValue("canBounceOnSameTarget", true);

            HurtBox hurtBox = initialOrbTarget;
            if (hurtBox)
            {
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
            if (animator.GetFloat("FireSeekingShot.fire") > 0f && initialOrbTarget && base.isAuthority && huntressTracker && hasFiredArrow == false)
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
        private bool hasFiredArrow;

        // Token: 0x04002D57 RID: 11607
        private HurtBox initialOrbTarget;
    }
    // Token: 0x02000837 RID: 2103
    public class MultiArrow : BaseState
    {
        // Token: 0x06002F97 RID: 12183 RVA: 0x000CBC18 File Offset: 0x000C9E18
        public override void OnEnter()
        {
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
                base.OnEnter();

                Transform modelTransform = base.GetModelTransform();
                if (modelTransform)
                {
                    childLocator = modelTransform.GetComponent<ChildLocator>();
                    animator = modelTransform.GetComponent<Animator>();
                }

                if (initialOrbTarget && base.isAuthority && huntressTracker)
                {
                    Util.PlayScaledSound(EntityStates.Huntress.HuntressWeapon.FireArrow.attackSoundString, base.gameObject, attackSpeedStat);
                    huntressTracker = base.GetComponent<HuntressTracker>();

                    duration = 0.75f / attackSpeedStat;
                    base.PlayCrossfade("Gesture, Override", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);
                    base.PlayCrossfade("Gesture, Additive", "FireSeekingShot", "FireSeekingShot.playbackRate", duration, duration * 0.2f / attackSpeedStat);

                    if (base.characterBody)
                    {
                        base.characterBody.SetAimTimer(duration + 1f);
                    }
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

            base.skillLocator.secondary.DeductStock(1);
            hasFiredArrow = true;

            LightningOrb lightningOrb = new LightningOrb();
            lightningOrb.lightningType = (LightningOrb.LightningType)(-1);
            lightningOrb.damageValue = base.characterBody.damage * 0.75f;
            lightningOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
            lightningOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            lightningOrb.attacker = base.gameObject;
            lightningOrb.procCoefficient = 0.05f;
            lightningOrb.bouncesRemaining = 3;
            lightningOrb.targetsToFindPerBounce = 2;
            lightningOrb.speed = 20;
            lightningOrb.bouncedObjects = new List<HealthComponent>();
            lightningOrb.range = 105;
            lightningOrb.damageCoefficientPerBounce = 2f;
            lightningOrb.SetFieldValue("canBounceOnSameTarget", true);



            HurtBox hurtBox = initialOrbTarget;
            if (hurtBox)
            {
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
            if (animator.GetFloat("FireSeekingShot.fire") > 0f && initialOrbTarget && base.isAuthority && huntressTracker && hasFiredArrow == false)
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
        private bool hasFiredArrow;

        // Token: 0x04002D57 RID: 11607
        private HurtBox initialOrbTarget;
    }

    public class ChargedArrow : BaseState
    {
        float power = 1f;
        private float stopwatch;
        private ChildLocator childLocator;
        private Animator animator;
        private float totalDuration;
        private float maxChargeTime;
        private string muzzleString;
        private float baseTotalDuration = 0.35f;
        private float baseMaxChargeTime;
        private int maxStocks;
        private int currentCharge = 0;
        // Token: 0x06002F97 RID: 12183 RVA: 0x000CBC18 File Offset: 0x000C9E18
        public override void OnEnter()
        {
            base.OnEnter();
            maxStocks = base.skillLocator.secondary.stock;
            totalDuration = (baseTotalDuration / base.characterBody.attackSpeed) * maxStocks;
            muzzleString = "Muzzle";
            Transform modelTransform = base.GetModelTransform();
            childLocator = modelTransform.GetComponent<ChildLocator>();
            animator = base.GetModelAnimator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.deltaTime;

            if ((stopwatch / totalDuration) * maxStocks > currentCharge)
            {
                base.skillLocator.secondary.DeductStock(1);
                Util.PlaySound("ror2_char_engi_walkingTurret_step_04", base.characterBody.gameObject);
                currentCharge++;
            }
            if ((stopwatch > totalDuration || !base.inputBank.skill2.down) && base.isAuthority)
            {
                FireArrowCharged fireArrow = new FireArrowCharged();
                fireArrow.charge = (stopwatch / totalDuration) * maxStocks;
                this.outer.SetNextState(fireArrow);
                return;
            }
        }

        // Token: 0x06002F9B RID: 12187 RVA: 0x0000B933 File Offset: 0x00009B33
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }       

    }

    public class FireArrowCharged : FireArrowSnipe
    {

        public override void OnEnter()
        {
            base.OnEnter();
            
        }

        // Token: 0x06003AB3 RID: 15027 RVA: 0x000F3280 File Offset: 0x000F1480
        protected override void ModifyBullet(BulletAttack bulletAttack)
        {
            base.ModifyBullet(bulletAttack);
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            bulletAttack.damage = damageStat * 6 * charge;
            bulletAttack.force = 100 * charge;
            bulletAttack.procCoefficient = 0.05f * charge;
            bulletAttack.radius = 0.1f;
            if (!tracerEffectPrefab)
            {
                tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/TracerHuntressSnipe");
            }

            Util.PlaySound("gravekeeper_impact_body_04", base.characterBody.gameObject);

            bulletAttack.tracerEffectPrefab = tracerEffectPrefab;

           
        }

        public override void OnExit()
        {
            base.outer.SetNextStateToMain();
            base.OnExit();
        }

        public new float charge;

    }
}

