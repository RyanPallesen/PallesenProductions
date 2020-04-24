using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace name
{
    [R2APISubmoduleDependency("SurvivorAPI")]
    [R2APISubmoduleDependency("PrefabAPI")]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.new.skill", "PlayableTC", "1.0.0")]
    public class PlayableTC : BaseUnityPlugin
    {
        //Can't sprint
        //Can't be frozen

        //sprites for skills <steal or made>

        //jump and sprint to go up and down, monobehaviour.

        //remove red/green lights

        public GameObject myCharacter;

        public void Awake()
        {
            myCharacter = Resources.Load<GameObject>("Prefabs/CharacterBodies/megadronebody").InstantiateClone("Prefabs/CharacterBodies/megadroneplayerbody");
            //Note; if your character cannot interact with things, play around with the following value after uncommenting them
            GameObject gameObject = myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
            myCharacter.transform.localScale *= 0.5f;
            myCharacter.GetComponent<CharacterBody>().aimOriginTransform.Translate(new Vector3(0, 0, 0));

            SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

            foreach (GenericSkill skill in myCharacter.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(skill);
            }


            foreach (MonoBehaviour skill in myCharacter.GetComponentsInChildren<MonoBehaviour>())
            {
                Debug.Log(skill);

                if (skill as RoR2.RigidbodyStickOnImpact)
                {
                    DestroyImmediate(skill);
                }

                if (skill as CameraRigController)
                {
                    CameraRigController cameraRigController = skill as CameraRigController;

                    cameraRigController.cameraMode = CameraRigController.CameraMode.Fly;
                }

            }

            myCharacter.AddComponent<EquipmentSlot>();

            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);

            skillSetup();

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(myCharacter);
            };

            CharacterBody component = myCharacter.GetComponent<CharacterBody>();

            component.GetComponent<ModelLocator>().modelBaseTransform.GetComponentInChildren<SkinnedMeshRenderer>().material.DisableKeyword("EMISSION");

            component.baseDamage = 6f;
            component.levelDamage = 1.25f;
            component.baseCrit = 1f;
            component.levelCrit = 0;
            component.baseMaxHealth = 90;
            component.levelMaxHealth = 20f;
            component.baseArmor = 5f;
            component.baseRegen = 2f;
            component.levelRegen = 0.1f;
            component.baseMoveSpeed = 16f;
            component.baseAttackSpeed = 2f;
            component.baseNameToken = "MyCharacterName";

            myCharacter.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            SurvivorDef survivorDef = new SurvivorDef
            {
                bodyPrefab = myCharacter,
                descriptionToken = "MyDescription \r\n",
                displayPrefab = myCharacter,
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                name = "MyCharacterName",
                unlockableName = ""
            };

            SurvivorAPI.AddSurvivor(survivorDef);



            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equip) =>
            {
                Debug.Log(equip.ToString());
                return orig(self, equip);
            };
        }

        void skillSetup()
        {
            PrimarySkills();
            SecondarySkills();
            UtilitySkills();
            SpecialSkills();
        }

        void PrimarySkills()
        {
            {
                SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

                skillLocator.primary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }

            {
                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.primary.skillFamily;

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Drone.DroneWeapon.FireMegaTurret));
                mySkillDef.activationStateMachineName = "WeaponGun";
                mySkillDef.baseMaxStock = 1;
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
                mySkillDef.shootDelay = 1f;
                mySkillDef.stockToConsume = 1;
                mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/commandobody").GetComponent<SkillLocator>().special.skillFamily.variants[0].skillDef.icon;
                mySkillDef.skillDescriptionToken = "Spray bullets in the direction of enemies";
                mySkillDef.skillName = "PP_PLAYABLETC_PRIMARY";
                mySkillDef.skillNameToken = "Supressive Fire";

                LoadoutAPI.AddSkillDef(mySkillDef);


                skillFamily.variants[0] =
                    new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                    };
            }
        }

        void SecondarySkills()
        {
            {
                SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

                skillLocator.secondary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];

                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }

            {
                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.secondary.skillFamily;

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Drone.DroneWeapon.FireTwinRocket));
                mySkillDef.activationStateMachineName = "WeaponRocket";
                mySkillDef.baseMaxStock = 2;
                mySkillDef.baseRechargeInterval = 4f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = false;
                mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = true;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0.25f;
                mySkillDef.stockToConsume = 1;
                mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/engibody").GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.icon;
                mySkillDef.skillDescriptionToken = "Fire two large rockets";
                mySkillDef.skillName = "PP_PLAYABLETC_SPECIAL";
                mySkillDef.skillNameToken = "Explosive Ordenance";

                LoadoutAPI.AddSkillDef(mySkillDef);


                skillFamily.variants[0] =
                    new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                    };
            }

        }

        void UtilitySkills()
        {
            {
                SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

                skillLocator.utility = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[3];

                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            }

            {
                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.utility.skillFamily;

                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Toolbot.ToolbotDash));
                mySkillDef.activationStateMachineName = "WeaponGun";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 12f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = true;
                mySkillDef.interruptPriority = InterruptPriority.Any;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = true;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = false;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 1f;
                mySkillDef.stockToConsume = 1;
                mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<SkillLocator>().utility.skillFamily.variants[0].skillDef.icon;
                mySkillDef.skillDescriptionToken = "Gain a boost of movement";
                mySkillDef.skillName = "PP_PLAYABLETC_Utility";
                mySkillDef.skillNameToken = "Movement Boost";

                LoadoutAPI.AddSkillDef(mySkillDef);


                skillFamily.variants[0] =
                    new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                    };
            }


            LoadoutAPI.AddSkill(typeof(EnterPainter));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(EnterPainter));
                mySkillDef.activationStateMachineName = "WeaponGun";
                mySkillDef.baseMaxStock = 16;
                mySkillDef.baseRechargeInterval = 2f;
                mySkillDef.beginSkillCooldownOnSkillEnd = true;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = false;
                mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = true;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 0;
                mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/engibody").GetComponent<SkillLocator>().utility.skillFamily.variants[1].skillDef.icon;
                mySkillDef.skillDescriptionToken = "Enter target-painting mode.";
                mySkillDef.skillName = "PP_PLAYABLETC_PAINT";
                mySkillDef.skillNameToken = "Target Acquisition";

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.utility.skillFamily;

                skillFamily.variants[1] =
                    new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                    };
            }

            LoadoutAPI.AddSkill(typeof(AllyPing));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(AllyPing));
                mySkillDef.activationStateMachineName = "WeaponGun";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 10f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = false;
                mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
                mySkillDef.isBullets = false;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = true;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/huntressbody").GetComponent<SkillLocator>().utility.skillFamily.variants[1].skillDef.icon;
                mySkillDef.skillDescriptionToken = "Command your drones to target an enemy or ally that you have pinged for 5 seconds";
                mySkillDef.skillName = "PP_PLAYABLETC_ALLYPING";
                mySkillDef.skillNameToken = "Additional Directives";

                LoadoutAPI.AddSkillDef(mySkillDef);
                GameObject gameObject = myCharacter;
                SkillLocator component = gameObject.GetComponent<SkillLocator>();
                SkillFamily skillFamily = component.utility.skillFamily;

                skillFamily.variants[2] =
                    new SkillFamily.Variant
                    {
                        skillDef = mySkillDef,
                        unlockableName = "",
                        viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                    };
            }
        }

        void SpecialSkills()
        {
            {
                SkillLocator skillLocator = myCharacter.GetComponent<SkillLocator>();

                skillLocator.special = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];

                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            }

            LoadoutAPI.AddSkill(typeof(DroneSummonBasic));
            {
                SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
                mySkillDef.activationState = new SerializableEntityStateType(typeof(DroneSummonBasic));
                mySkillDef.activationStateMachineName = "WeaponGun";
                mySkillDef.baseMaxStock = 1;
                mySkillDef.baseRechargeInterval = 16f;
                mySkillDef.beginSkillCooldownOnSkillEnd = false;
                mySkillDef.canceledFromSprinting = false;
                mySkillDef.fullRestockOnAssign = false;
                mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
                mySkillDef.isBullets = true;
                mySkillDef.isCombatSkill = false;
                mySkillDef.mustKeyPress = false;
                mySkillDef.noSprint = true;
                mySkillDef.rechargeStock = 1;
                mySkillDef.requiredStock = 1;
                mySkillDef.shootDelay = 0f;
                mySkillDef.stockToConsume = 1;
                mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/drone1body").GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.icon;
                mySkillDef.skillDescriptionToken = "Summon two basic gunner drones that inherit all your items";
                mySkillDef.skillName = "PP_PLAYABLETC_SPECIAL_BASIC";
                mySkillDef.skillNameToken = "Calling Backup";

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

            //LoadoutAPI.AddSkill(typeof(DroneSummonHealing));
            //{
            //    SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //    mySkillDef.activationState = new SerializableEntityStateType(typeof(DroneSummonHealing));
            //    mySkillDef.activationStateMachineName = "WeaponGun";
            //    mySkillDef.baseMaxStock = 1;
            //    mySkillDef.baseRechargeInterval = 16f;
            //    mySkillDef.beginSkillCooldownOnSkillEnd = false;
            //    mySkillDef.canceledFromSprinting = false;
            //    mySkillDef.fullRestockOnAssign = false;
            //    mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            //    mySkillDef.isBullets = true;
            //    mySkillDef.isCombatSkill = false;
            //    mySkillDef.mustKeyPress = false;
            //    mySkillDef.noSprint = true;
            //    mySkillDef.rechargeStock = 1;
            //    mySkillDef.requiredStock = 1;
            //    mySkillDef.shootDelay = 3f;
            //    mySkillDef.stockToConsume = 1;
            //    mySkillDef.icon = Resources.Load<GameObject>("Prefabs/CharacterBodies/drone2body").GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.icon;
            //    mySkillDef.skillDescriptionToken = "Summon two healing drones that inherit all your items";
            //    mySkillDef.skillName = "PP_PLAYABLETC_SPECIAL_HEALING";
            //    mySkillDef.skillNameToken = "Emergency Services";

            //    LoadoutAPI.AddSkillDef(mySkillDef);

            //    GameObject gameObject = myCharacter;
            //    SkillLocator component = gameObject.GetComponent<SkillLocator>();
            //    SkillFamily skillFamily = component.special.skillFamily;

            //    skillFamily.variants[1] =
            //        new SkillFamily.Variant
            //        {
            //            skillDef = mySkillDef,
            //            unlockableName = "",
            //            viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            //        };
            //}


        }

    }

    public class AllyPing : BaseState
    {
        private static List<MinionOwnership.MinionGroup> _instancesList;

        private static bool GetMinionMembers(MinionOwnership.MinionGroup minionGroup, out MinionOwnership[] members)
        {
            members = null;

            if (minionGroup != null)
            {
                members = minionGroup.GetFieldValue<MinionOwnership[]>("members");
            }
            else
            {
                Debug.LogError("Miniongroup did not exist");
            }
            return members != null;
        }
        private static void SetMemberTarget(MinionOwnership member, GameObject targetGameObject)
        {
            if (!member)
            {
                return;
            }
            BaseAI component = member.GetComponent<BaseAI>();
            if (!component)
            {
                return;
            }
            if (component.currentEnemy == null)
            {
                return;
            }

            if (targetGameObject)
                component.currentEnemy.gameObject = targetGameObject;
        }

        public static List<MinionOwnership.MinionGroup> instancesList
        {
            get
            {
                if (AllyPing._instancesList == null)
                {
                    AllyPing._instancesList = (List<MinionOwnership.MinionGroup>)typeof(MinionOwnership.MinionGroup).GetField("instancesList", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                }
                return AllyPing._instancesList;
            }
        }

        public override void OnEnter()
        {

            base.OnEnter();

            Debug.Log("Called allyping");

            MinionOwnership[] array;



            foreach (MinionOwnership.MinionGroup minionGroup in instancesList)
            {
                if (minionGroup.ownerId == base.characterBody.master.netId)
                {
                    if (AllyPing.GetMinionMembers(minionGroup, out array))
                    {
                        foreach (PlayerCharacterMasterController playerNetworkUser in PlayerCharacterMasterController.instances)
                        {
                            if (playerNetworkUser.netId == base.characterBody.master.netId)
                            {
                                PingerController pingerController = playerNetworkUser.GetComponent<PingerController>();
                                minions = array;
                                target = pingerController.currentPing.targetGameObject;
                            }
                        }
                    }
                }
            }
        }

        GameObject target;
        MinionOwnership[] minions;
        float currentTime = 0;
        public override void FixedUpdate()
        {
            currentTime += Time.deltaTime;

            if (minions == null)
            {
                base.OnExit();
                return;
            }

            for (int i = 0; i < minions.Length; i++)
            {
                AllyPing.SetMemberTarget(minions[i], target);
            }

            if (currentTime > 5)
            {
                currentTime = 0;
                base.OnExit();
            }

            base.FixedUpdate();
        }
    }

    public class DroneSummonBasic : BaseState
    {
        public override void OnEnter()
        {

            if (!Stage.instance.sceneDef.suppressNpcEntry)
            {
                base.OnEnter();

                Vector3 direction = base.GetAimRay().direction;

                base.characterBody.SendConstructTurret(base.characterBody, transform.position, Quaternion.Euler(direction), MasterCatalog.FindMasterIndex("DroneBackupMaster"));

                base.OnExit();

                outer.SetNextStateToMain();
            }

        }

    }

    public class DroneSummonHealing : BaseState
    {
        public override void OnEnter()
        {

            if (!Stage.instance.sceneDef.suppressNpcEntry)
            {
                base.OnEnter();

                Vector3 direction = base.GetAimRay().direction;

                base.characterBody.SendConstructTurret(base.characterBody, transform.position, Quaternion.Euler(direction), MasterCatalog.FindMasterIndex("Drone2Master"));

                base.OnExit();
            }

        }

    }

    public class EnterPainter : EntityStates.Engi.EngiMissilePainter.Paint
    {
        public override void OnEnter()
        {
            if (base.isAuthority)
            {
                base.OnEnter();
            }
        }
        public override void FixedUpdate()
        {
            if (base.isAuthority)
            {
                base.FixedUpdate();
            }

            base.characterBody.SetAimTimer(1f);

        }
    }

}