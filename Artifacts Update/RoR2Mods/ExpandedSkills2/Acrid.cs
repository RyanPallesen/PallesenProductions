
using EntityStates;
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

namespace ExpandedSkills2
{
    public static class Acrid
    {
        public static void Setup()
        {
            //SUMMON BROOD
            LoadoutAPI.AddSkill(typeof(VTStates.States.Acrid.SummonBrood));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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

        }
    }
}

namespace VTStates.States.Acrid
{
    public class SummonBrood : EntityStates.BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            for (int i = 0; i < 4; i++)
            {
               // ExpandedSkills.SendNetworkMessage(base.characterBody.networkIdentity.netId, 1);
            }

            base.OnExit();
        }
    }
}
