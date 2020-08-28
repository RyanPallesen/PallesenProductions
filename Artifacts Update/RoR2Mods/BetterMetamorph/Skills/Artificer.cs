
using EntityStates;
using EntityStates.ArtifactShell;
using PallesenProductions;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace ExpandedSkills2
{
    public static class Artificer
    {
        public static void Setup()
        {
            #region artificer

            LoadoutAPI.AddSkill(typeof(VTStates.States.Mage.FireSeekingProjectile));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                //mySkillDef.icon = Resources.Load<Sprite>()
                mySkillDef.skillDescriptionToken = "Launch 2 <style=cIsUtility>homing</style> bolts of fire, dealing <style=cIsDamage> 80% Damage</style> each";
                mySkillDef.skillName = "VT_FIRE_PRIMARY";
                mySkillDef.skillNameToken = "Heat Seekers";

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

            {

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/magebody");
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily primary = component.primary.skillFamily;
                SkillFamily secondary = component.secondary.skillFamily;


                int cacheNumPrimary = primary.variants.Length;
                int cacheNumSecondary = secondary.variants.Length;

                {
                    for (int i = 0; i < cacheNumPrimary; i++)
                    {
                        Array.Resize(ref secondary.variants, secondary.variants.Length + 1);
                        secondary.variants[secondary.variants.Length - 1] = primary.variants[i];
                    }

                    for (int i = 0; i < cacheNumSecondary; i++)
                    {
                        Array.Resize(ref primary.variants, primary.variants.Length + 1);
                        primary.variants[primary.variants.Length - 1] = secondary.variants[i];
                    }
                }

            }
            LoadoutAPI.AddSkill(typeof(VTStates.States.Mage.GravBomb));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mage.GravBomb));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 30f;
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
                mySkillDef.skillDescriptionToken = "Launch a <style=cIsUtility>tethering black hole</style> that explodes on impact with the world, dealing <style=cIsDamage> 800% Damage</style>";
                mySkillDef.skillName = "VT_FIRE_SPECIAL";
                mySkillDef.skillNameToken = "Gravitic Compulsion";

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

            LoadoutAPI.AddSkill(typeof(VTStates.States.Mage.SummonFlameImp));
            {

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Mage.SummonFlameImp));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 30f;
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
                mySkillDef.skillDescriptionToken = "Summon a Blazing Imp, with random items that match how many you have.";
                mySkillDef.skillName = "VT_FIREIMP_SPECIAL";
                mySkillDef.skillNameToken = "Summon Elemental";

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

        }
    }
}

namespace VTStates.States.Mage
{

    public class FireSeekingProjectile : BaseState
    {

        TeamFilter filter;
        ProjectileTargetComponent targeter;
        ProjectileSphereTargetFinder targetfinder;
        ProjectileSteerTowardTarget steertoward;
        ProjectileSimple projectile;

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

            if (NetworkServer.active)
            {
                if (!projectilePrefab)
                {
                    projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/magefireboltbasic").InstantiateClone("magefireboltcopy");

                }

                if (!filter)
                {
                    filter = projectilePrefab.AddComponent<TeamFilter>();
                }

                if (!targeter)
                {
                    targeter = projectilePrefab.AddComponent<ProjectileTargetComponent>();
                }



                if (!steertoward)
                {
                    steertoward = projectilePrefab.AddComponent<ProjectileSteerTowardTarget>();
                    steertoward.rotationSpeed = 360f;

                }

                if (!targetfinder)
                {
                    targetfinder = projectilePrefab.AddComponent<ProjectileSphereTargetFinder>();
                    targetfinder.lookRange = 16;
                    targetfinder.allowTargetLoss = false;
                    targetfinder.onlySearchIfNoTarget = true;
                    targetfinder.testLoS = true;
                    targetfinder.targetSearchInterval = 0.15f;
                }

                if (!projectile)
                {
                    projectile = projectilePrefab.AddComponent<ProjectileSimple>();
                    projectile.velocity = 50f;
                    projectile.updateAfterFiring = true;

                }

                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.searchOrigin = aimRay.origin;
                bullseyeSearch.searchDirection = aimRay.direction;
                bullseyeSearch.maxDistanceFilter = 100f;
                bullseyeSearch.maxAngleFilter = 60f;
                bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
                bullseyeSearch.RefreshCandidates();
                List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
                HurtBox hurtBox = (list.Count > 0) ? list[0] : null;
                if (hurtBox)
                {
                    targeter.target = hurtBox.transform; 
                }
                for (int i = 0; i < 2; i++)
                {
                    Quaternion direction = Util.QuaternionSafeLookRotation(aimRay.direction + (new Vector3(Random.Range(-36f, 36f), Random.Range(-36f, 36f), Random.Range(-36f, 36f)) / 360f));

                    ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, direction, base.gameObject, 0.8f * damageStat, 0f, base.characterBody.RollCrit(), DamageColorIndex.Default, hurtBox ? hurtBox.gameObject : null);

                }
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

    public class GravBomb : BaseState
    {
        public GameObject projectilePrefab;
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
            if (NetworkServer.active)
            {

                if (!projectilePrefab)
                {
                    projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/gravsphere").InstantiateClone("ArtificerSpecialGrav");
                    var temp = projectilePrefab.AddComponent<ProjectileImpactExplosion>();
                    temp.destroyOnEnemy = false;
                    temp.destroyOnWorld = true;
                    temp.blastRadius = 8f;
                    temp.blastAttackerFiltering = AttackerFiltering.Default;

                    temp.blastDamageCoefficient = 8f;
                    temp.childrenCount = 0;
                    temp.blastProcCoefficient = 0.2f;

                    var temp2 = projectilePrefab.GetComponentInChildren<RadialForce>();
                    temp2.radius = 8;
                    temp2.forceMagnitude = -1500;
                }

                bool crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                Vector3 position = base.characterBody.inputBank ? base.characterBody.inputBank.aimOrigin : base.transform.position;


                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = projectilePrefab,
                    position = position,
                    rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction),
                    owner = base.characterBody.gameObject,
                    damage = base.characterBody.damage * 8f,
                    force = 20f,
                    crit = crit,
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                    speedOverride = 16f,
                    useFuseOverride = true,
                    fuseOverride = 4f
                };

                ProjectileManager.instance.FireProjectile(info);


            }

            OnExit();
        }

        public override void OnExit()
        {
            base.outer.SetNextStateToMain();
            base.OnExit();
        }
    }

    public class SummonFlameImp : BaseState
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
            if (NetworkServer.active)
            {
                CharacterMaster characterMaster = new MasterSummon
                {
                    masterPrefab = MasterCatalog.FindMasterPrefab("ImpMaster"),
                    position = base.characterBody.transform.position,
                    rotation = base.transform.rotation,
                    summonerBodyObject = base.characterBody.gameObject,
                    ignoreTeamMemberLimit = true,
                    teamIndexOverride = TeamIndex.Player,


                }.Perform();

                characterMaster.GetBody().AddBuff(BuffIndex.AffixRed);
                characterMaster.GetBody().inventory.GiveRandomItems(base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier1) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier2) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier3) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Lunar) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Boss));
                characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 30);
            }

            base.OnExit();
        }

    }
}

