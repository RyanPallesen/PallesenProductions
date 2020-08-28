
using EntityStates;
using PallesenProductions;
using R2API;
using R2API.Utils;
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
    public static class Commando
    {

        private static List<(SkillDef, string)> Passives = new List<(SkillDef, string)>();
        public static void SetupPassive()
        {
            {

                {
                    SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                    mySkillDef.activationState = new SerializableEntityStateType(typeof(BaseState));
                    mySkillDef.activationStateMachineName = "Weapon";

                    //mySkillDef.icon = Resources.Load<Sprite>()
                    mySkillDef.skillDescriptionToken = "Higher Passive Regen ";
                    mySkillDef.skillName = "EXPANDEDSKILLS_COMMANDOPASSIVE_REGEN";
                    mySkillDef.skillNameToken = "Hardened Body";

                    Passives.Add((mySkillDef, "Hardened Body"));
                }

                {
                    SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                    mySkillDef.activationState = new SerializableEntityStateType(typeof(BaseState));
                    mySkillDef.activationStateMachineName = "Weapon";

                    //mySkillDef.icon = Resources.Load<Sprite>()
                    mySkillDef.skillDescriptionToken = "Higher Passive Speed ";
                    mySkillDef.skillName = "EXPANDEDSKILLS_COMMANDOPASSIVE_SPEED";
                    mySkillDef.skillNameToken = "Quick Legs";

                    Passives.Add((mySkillDef, "Quick Legs"));
                }

                {
                    SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                    mySkillDef.activationState = new SerializableEntityStateType(typeof(BaseState));
                    mySkillDef.activationStateMachineName = "Weapon";

                    //mySkillDef.icon = Resources.Load<Sprite>()
                    mySkillDef.skillDescriptionToken = "Higher Passive Damage ";
                    mySkillDef.skillName = "EXPANDEDSKILLS_COMMANDOPASSIVE_DAMAGE";
                    mySkillDef.skillNameToken = "Trained Gunman";

                    Passives.Add((mySkillDef, "Trained Gunman"));
                }

                On.RoR2.CharacterMaster.Awake += (orig, self) =>
                {
                    orig(self);

                    self.onBodyStart += (body) =>
                    {
                        if (body.bodyIndex == SurvivorCatalog.GetBodyIndexFromSurvivorIndex(SurvivorIndex.Commando))
                        {
                            switch (body.GetComponentsInChildren<GenericSkill>().FirstOrDefault(x => x.skillFamily.variants[0].skillDef.skillName == "EXPANDEDSKILLS_COMMANDOPASSIVE_REGEN").skillDef.skillName)
                            {
                                case "EXPANDEDSKILLS_COMMANDOPASSIVE_REGEN":
                                    body.baseRegen *= 1.5f;
                                    break;
                                case "EXPANDEDSKILLS_COMMANDOPASSIVE_SPEED":
                                    body.baseMoveSpeed *= 1.5f;
                                    break;
                                case "EXPANDEDSKILLS_COMMANDOPASSIVE_DAMAGE":
                                    body.baseDamage *= 1.5f;
                                    break;
                            }
                        }

                    };
                };

                GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/commandobody");
                GenericSkill component = gameObject.AddComponent<GenericSkill>();

                component.SetFieldValue("_skillFamily", Skills.FromList(Passives, "Passive"));
            }
        }


        public static void Setup()
        {
            SetupPassive();

            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
                mySkillDef.skillDescriptionToken = "Hold up to <style=cIsUtility>6</style> bullets, each dealing <style=cIsDamage>100% damage</style>. <style=cIsUtility>Reloading returns all bullets</style>";
                mySkillDef.skillName = "EXPANDEDSKILLS_POWERPISTOL_PRIMARY";
                mySkillDef.skillNameToken = "Stacked Shooter";

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
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Huntress.Weapon.FireArrowSnipe));
                mySkillDef.activationStateMachineName = "Weapon";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 5f;
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
                mySkillDef.skillDescriptionToken = "Launch a <style=cIsUtility>piercing</style> laser, dealing <style=cIsDamage> 900% Damage</style>";
                mySkillDef.skillName = "EXPANDEDSKILLS_POWERPISTOL2_PRIMARY";
                mySkillDef.skillNameToken = "Portable Pocket Railgun";

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

        }
    }


}
