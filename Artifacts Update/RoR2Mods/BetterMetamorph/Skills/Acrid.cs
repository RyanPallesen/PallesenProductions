
using EntityStates;
using EntityStates.Treebot.Weapon;
using PallesenProductions;
using R2API;
using RoR2;
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
    public static class Acrid
    {
        public static void Setup()
        {
            //SUMMON BROOD
            LoadoutAPI.AddSkill(typeof(VTStates.States.Acrid.SummonManyAtPoint));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(VTStates.States.Acrid.SummonManyAtPoint));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 4;
                mySkillDef.baseRechargeInterval = 30f;
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
                mySkillDef.stockToConsume = 0;
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

        }
    }
}

namespace VTStates.States.Acrid
{
    public class SummonManyAtPoint : BaseState
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
                    this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < 360f);
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
            if (this.stopwatch >= this.duration && !base.inputBank.skill4.down && base.isAuthority)
            {
                OnExit();
            }
        }

        public override void OnExit()
        {
            if (this.goodPlacement && base.characterBody.isPlayerControlled)
            {
                Util.PlaySound(CreatePounder.fireSoundString, base.gameObject);
                if (this.areaIndicatorInstance && NetworkServer.active)
                {
                    if (editedPounder == null)
                    {
                        editedPounder = CreatePounder.projectilePrefab;
                    }

                    bool crit = Util.CheckRoll(this.critStat, base.characterBody.master);

                    int cacheNumToSpawn = base.skillLocator.special.stock;

                    for (int i = 0; i < cacheNumToSpawn; i++)
                    {
                        CharacterMaster characterMaster = new MasterSummon
                        {
                            masterPrefab = MasterCatalog.FindMasterPrefab("CrocoMonsterMaster"),
                            position = areaIndicatorInstance.transform.position,
                            rotation = characterBody.transform.rotation,
                            summonerBodyObject = base.gameObject,
                            ignoreTeamMemberLimit = false,
                            teamIndexOverride = characterBody.teamComponent.teamIndex

                        }.Perform();

                        base.skillLocator.special.DeductStock(1);

                        characterMaster.loadout.bodyLoadoutManager.SetSkillVariant(characterBody.bodyIndex, 0, (uint)Random.Range(0, characterBody.skillLocator.primary.skillFamily.variants.Length));
                        characterMaster.loadout.bodyLoadoutManager.SetSkillVariant(characterBody.bodyIndex, 1, (uint)Random.Range(0, characterBody.skillLocator.secondary.skillFamily.variants.Length));
                        characterMaster.loadout.bodyLoadoutManager.SetSkillVariant(characterBody.bodyIndex, 2, (uint)Random.Range(0, characterBody.skillLocator.utility.skillFamily.variants.Length));
                        characterMaster.loadout.bodyLoadoutManager.SetSkillVariant(characterBody.bodyIndex, 3, (uint)Random.Range(0, characterBody.skillLocator.special.skillFamily.variants.Length));
                        characterMaster.loadout.bodyLoadoutManager.SetSkinIndex(characterBody.bodyIndex, (uint)Random.Range(0, SkinCatalog.GetBodySkinCount(characterBody.bodyIndex)));
                        characterMaster.inventory.CopyItemsFrom(base.characterBody.inventory);

                        characterMaster.inventory.GiveItem(ItemIndex.HealthDecay, 15);
                    }
                }
            }

            EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
            base.OnExit();
            this.outer.SetNextStateToMain();

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
