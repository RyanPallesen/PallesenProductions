using BepInEx;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.Networking;
using RoR2;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace PallesenProductions
{
    [R2APISubmoduleDependency("SurvivorAPI")]
    [R2APISubmoduleDependency("DifficultyAPI")]
    [R2APISubmoduleDependency("SkinAPI")]
    [R2APISubmoduleDependency("SkillAPI")]
    [R2APISubmoduleDependency("EntityAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.VanillaTweaks", "VanillaTweaks", "1.2.0")]


    public class VanillaTweaks : BaseUnityPlugin
    {
        //Added allow elite on all.
        //Added multi-elte types
        //Added halycon seed progression
        //Added new elite types for progression
        //Added compatablity with alternate artificer
        //fixed setstateinternal error

        //Note make aurelionate scale with number of players
        //Note make aurelionite check for how many seeds are owned.
        int GoldSpawns
        {
            get
            {

                int numItems = 0;
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);

                for (int i = 0; i < teamMembers.Count; i++)
                {
                    if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                    {
                        CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                        if (component && component.inventory)
                        {
                            numItems += component.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
                        }
                    }
                }

                return Mathf.FloorToInt(numItems / Run.instance.participatingPlayerCount);
            }
        }
        public void Awake()
        {
            //Taken from https://github.com/Legendsmith/ThunderDownUnder/blob/master/SolidIceWall/SolidIceWallLoader.cs
            GameObject pillarprefab = Resources.Load<GameObject>("prefabs/projectiles/mageicewallpillarprojectile");
            pillarprefab.layer = 11;
            On.RoR2.ArenaMissionController.EndRound += TrueSuicideOnNullVoid;

            On.RoR2.MultiShopController.CreateTerminals += MultishopNoDuplicates;

            On.RoR2.CharacterMaster.OnBodyStart += SizeTweaks;

            On.RoR2.ShrineChanceBehavior.Awake += ShrineChanceInit;
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceReturns;

            On.RoR2.ShrineRestackBehavior.Start += ShrineRestackBehaviours;

            On.RoR2.GenericPickupController.OnTriggerStay += NoAutoPickup;

            On.RoR2.ScavengerItemGranter.Start += ScavengerNoResetGhost;

            On.RoR2.Run.Awake += RunEdits;

            On.RoR2.GenericPickupController.Start += ItemDropChanges;

            On.RoR2.EquipmentSlot.PerformEquipmentAction += CustomEquipment;

            On.RoR2.CharacterMaster.AddDeployable += TurretEliteInheritance;

            On.RoR2.Util.GetBestBodyName += AdditionalEliteTypeNames;

            On.RoR2.SummonMasterBehavior.OpenSummonReturnMaster += OwnershipEliteInheritance;

            On.RoR2.GlobalEventManager.OnCharacterDeath += IncreaseDeathRewards;

            On.RoR2.SceneDirector.Start += BazaarEdits;

            On.EntityStates.Missions.LunarScavengerEncounter.WaitForAllMonstersDead.FixedUpdateServer += FealtyStacks;

            On.RoR2.CharacterSpawnCard.Awake += (orig, self) =>
            {
                orig(self);
                self.forbiddenAsBoss = false;
                self.noElites = false;
            };

            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
            };

            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
                {
                    if (slot == (DeployableSlot)7)
                    {
                        return self.inventory.GetItemCount(ItemIndex.EquipmentMagazine);
                    }

                    return orig(self, slot);
                };

            On.RoR2.CharacterBody.OnEquipmentGained += (orig, self, def) =>
            {

                if (def.equipmentIndex == EquipmentIndex.AffixGold)
                {
                    self.AddBuff(BuffIndex.BugWings);
                    self.AddBuff(BuffIndex.AffixBlue);
                    self.AddBuff(BuffIndex.AffixWhite);
                    self.AddBuff(BuffIndex.AffixRed);
                }
                if (def.equipmentIndex == EquipmentIndex.AffixYellow)
                {
                    self.AddBuff(BuffIndex.AffixHaunted);
                    self.AddBuff(BuffIndex.AffixHauntedRecipient);
                    self.AddBuff(BuffIndex.AffixPoison);
                    self.AddBuff(BuffIndex.AffixBlue);
                    self.AddBuff(BuffIndex.AffixWhite);
                    self.AddBuff(BuffIndex.AffixRed);
                }

                orig(self, def);

            };

            On.RoR2.CharacterBody.OnEquipmentLost += (orig, self, def) =>
            {
                if (def.equipmentIndex == EquipmentIndex.AffixGold)
                {
                    self.RemoveBuff(BuffIndex.BugWings);
                    self.RemoveBuff(BuffIndex.AffixBlue);
                    self.RemoveBuff(BuffIndex.AffixWhite);
                    self.RemoveBuff(BuffIndex.AffixRed);
                }
                if (def.equipmentIndex == EquipmentIndex.AffixYellow)
                {
                    self.RemoveBuff(BuffIndex.AffixHaunted);
                    self.RemoveBuff(BuffIndex.AffixHauntedRecipient);
                    self.RemoveBuff(BuffIndex.AffixPoison);
                    self.RemoveBuff(BuffIndex.AffixBlue);
                    self.RemoveBuff(BuffIndex.AffixWhite);
                    self.RemoveBuff(BuffIndex.AffixRed);
                }


                orig(self, def);
            };

            On.RoR2.TeleporterInteraction.Awake += (orig, self) =>
            {
                orig(self);

                if (NetworkServer.active)
                {
                    if (Run.instance.stageRngGenerator.RangeFloat(0, 100) < 10)
                    {
                        self.shouldAttemptToSpawnGoldshoresPortal = true;
                    }
                }

            };

            On.EntityStates.Missions.Goldshores.GoldshoresBossfight.SpawnBoss += (orig, self) =>
            {
                CharacterBody bossInstance = SpawnTitanByCompletion(TeamIndex.Monster);

                if (bossInstance)
                {
                    ScriptedCombatEncounter scriptedCombatEncounter = self.GetFieldValue<ScriptedCombatEncounter>("scriptedCombatEncounter");
                    if (!scriptedCombatEncounter)
                    {
                        scriptedCombatEncounter = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Missions.Goldshores.GoldshoresBossfight.combatEncounterPrefab).GetComponent<ScriptedCombatEncounter>();
                        scriptedCombatEncounter.GetComponent<BossGroup>().dropPosition = bossInstance.transform;
                        NetworkServer.Spawn(scriptedCombatEncounter.gameObject);
                    }
                    scriptedCombatEncounter.GetComponent<BossGroup>().combatSquad.AddMember(bossInstance.master);

                    self.SetFieldValue<CharacterBody>("bossInstanceBody", bossInstance);
                    self.InvokeMethod("SetBossImmunity", (true));
                    self.SetFieldValue("hasSpawnedBoss", true);
                    self.SetFieldValue<ScriptedCombatEncounter>("scriptedCombatEncounter", scriptedCombatEncounter);
                }
                else
                {
                    orig(self);
                }

            };

            On.RoR2.GoldshoresMissionController.SpawnBeacons += (orig, self) =>
            {
                self.beaconsToSpawnOnMap += GoldSpawns;
                self.beaconsRequiredToSpawnBoss = 0;

                orig(self);
            };

            On.RoR2.TeleporterInteraction.TrySpawnTitanGold += (orig, self) =>
            {
                SpawnTitanByCompletion(TeamIndex.Player);
            };

            On.RoR2.CombatDirector.Awake += MultiEliteHook;
        }

        CharacterBody SpawnTitanByCompletion(TeamIndex index = TeamIndex.Monster)
        {
            int cacheGoldSpawns = GoldSpawns;
            int tempGoldSpawns = cacheGoldSpawns;


            CharacterBody tempBody = null;

            while (cacheGoldSpawns > 0)
            {
                if (cacheGoldSpawns > 8)
                {
                    tempGoldSpawns = 8;
                }
                else
                {
                    tempGoldSpawns = cacheGoldSpawns;
                }

                if (tempGoldSpawns == 1)
                {
                    tempBody = SpawnTitan(EquipmentIndex.None, index);

                }
                if (tempGoldSpawns == 2)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixWhite, index);

                }
                if (tempGoldSpawns == 3)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixBlue, index);

                }
                if (tempGoldSpawns == 4)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixRed, index);

                }
                if (tempGoldSpawns == 5)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixPoison, index);

                }
                if (tempGoldSpawns == 6)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixGold, index);

                }
                if (tempGoldSpawns == 7)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixHaunted, index);

                }
                if (tempGoldSpawns == 8)
                {
                    tempBody = SpawnTitan(EquipmentIndex.AffixYellow, index);

                }
                cacheGoldSpawns -= tempGoldSpawns;
            }

            return tempBody;
        }

        CharacterBody SpawnTitan(EquipmentIndex affix, TeamIndex index = TeamIndex.Monster)
        {
            int numGoldItems = 0;

            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
            for (int i = 0; i < teamMembers.Count; i++)
            {
                if (Util.LookUpBodyNetworkUser(teamMembers[i].gameObject))
                {
                    CharacterBody component = teamMembers[i].GetComponent<CharacterBody>();
                    if (component && component.inventory)
                    {
                        numGoldItems += component.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP);
                    }
                }
            }

            if (index == TeamIndex.Monster)
            {
                numGoldItems += Run.instance.stageClearCount;
                numGoldItems *= Run.instance.livingPlayerCount;
            }

            Vector3 position = base.transform.position;
            if (index == TeamIndex.Player)
            {
                position = TeleporterInteraction.instance.transform.position;
            }

            DirectorPlacementRule placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                minDistance = 20f,
                maxDistance = 130f,
                position = position
            };
            if (index == TeamIndex.Monster)
            {
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.Random;
            }

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscTitanGold"), placementRule, Run.instance.spawnRng);
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.teamIndexOverride = new TeamIndex?(index);
            GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            if (gameObject)
            {


                float num2 = 1f;
                float num3 = 1f;
                num3 += Run.instance.difficultyCoefficient / 8f;
                num2 += Run.instance.difficultyCoefficient / 2f;
                CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                if (index == TeamIndex.Monster)
                {
                    component2.isBoss = true;

                }
                CharacterBody body = component2.GetBody();

                if (index == TeamIndex.Monster)
                {
                    body.isChampion = true;
                }

                int livingPlayerCount = Run.instance.livingPlayerCount;
                num2 *= Mathf.Pow((float)numGoldItems, 1f);
                num3 *= Mathf.Pow((float)numGoldItems, 0.5f);
                component2.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num2 - 1f) * 10f));
                component2.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((num3 - 1f) * 10f));
                component2.inventory.SetEquipmentIndex(affix);

                return body;
            }

            return null;
        }
        private void MultiEliteHook(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            orig(self);

            self.onSpawnedServer.AddListener((OG) => doMultiElite(OG, self));
        }

        private void doMultiElite(GameObject gameObject, CombatDirector self)
        {
            DirectorCard card = self.lastAttemptedMonsterCard;

            float credit = self.monsterCredit;

            CharacterBody body = gameObject.GetComponentInChildren<CharacterMaster>().GetBody();
            CombatDirector.EliteTierDef[] eliteDefs = typeof(CombatDirector).GetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers");

            int currentEliteTypes = 1;

            foreach (BuffIndex buff in BuffCatalog.eliteBuffIndices)
            {
                BuffDef buffDef = BuffCatalog.GetBuffDef(buff);
                if (body.HasBuff(buff))
                {
                    currentEliteTypes += 1;
                }
            }
            if (currentEliteTypes > 1)
            {

                foreach (CombatDirector.EliteTierDef tierDef in eliteDefs)
                {
                    foreach (EliteIndex eliteIndex in tierDef.eliteTypes)
                    {
                       
                        if (credit > card.cost * tierDef.costMultiplier * currentEliteTypes)
                        {
                            BuffDef buffDef = BuffCatalog.GetBuffDef(BuffIndex.None);

                            foreach (BuffIndex buff in BuffCatalog.eliteBuffIndices)
                            {
                                BuffDef tempDef = BuffCatalog.GetBuffDef(buff);

                                if (tempDef.eliteIndex == eliteIndex)
                                {
                                    buffDef = tempDef;
                                }
                            }

                            if (buffDef != null)
                            {
                                if (!(body.HasBuff(buffDef.buffIndex)))
                                {
                                    body.AddBuff(buffDef.buffIndex);
                                    self.monsterCredit -= card.cost * tierDef.costMultiplier * currentEliteTypes;
                                    credit -= card.cost * tierDef.costMultiplier * currentEliteTypes;
                                    currentEliteTypes++;

                                    float num3 = tierDef.healthBoostCoefficient;
                                    float damageBoostCoefficient = tierDef.damageBoostCoefficient;
                                    if (self.combatSquad)
                                    {
                                        int livingPlayerCount = Run.instance.livingPlayerCount;
                                        num3 *= Mathf.Pow((float)livingPlayerCount, 1f);
                                    }
                                    body.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
                                    body.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((damageBoostCoefficient - 1f) * 10f));

                                    EliteDef eliteDef = EliteCatalog.GetEliteDef(eliteIndex);
                                    EquipmentIndex equipmentIndex = (eliteDef != null) ? eliteDef.eliteEquipmentIndex : EquipmentIndex.None;
                                    if (equipmentIndex != EquipmentIndex.None)
                                    {
                                        if (body.inventory.GetEquipmentIndex() == EquipmentIndex.None)
                                        {
                                            body.inventory.SetEquipmentIndex(equipmentIndex);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

                if (currentEliteTypes > 6)
                {
                    body.inventory.SetEquipmentIndex(EquipmentIndex.AffixGold);
                }

            }
        }

        private void FealtyStacks(On.EntityStates.Missions.LunarScavengerEncounter.WaitForAllMonstersDead.orig_FixedUpdateServer orig, EntityStates.Missions.LunarScavengerEncounter.WaitForAllMonstersDead self)
        {


            List<string> masters = new List<string>() { "ScavLunar1Master", "ScavLunar2Master", "ScavLunar3Master", "ScavLunar4Master" };

            if (TeamComponent.GetTeamMembers(TeamIndex.Monster).Count == 0)
            {
                for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
                {
                    if (CharacterMaster.readOnlyInstancesList[i].inventory.GetItemCount(ItemIndex.LunarTrinket) > 0)
                    {
                        CharacterMaster.readOnlyInstancesList[i].inventory.RemoveItem(ItemIndex.LunarTrinket);

                        CharacterMaster characterMaster = new MasterSummon
                        {
                            masterPrefab = MasterCatalog.FindMasterPrefab(masters[UnityEngine.Random.Range(0, masters.Count)]),
                            position = CharacterMaster.readOnlyInstancesList[i].gameObject.transform.position - new Vector3(0, 5, 0),
                            rotation = base.transform.rotation,
                            summonerBodyObject = null,
                            ignoreTeamMemberLimit = true,
                            teamIndexOverride = TeamIndex.Monster

                        }.Perform();

                        characterMaster.onBodyDeath.AddListener(() => NetworkServer.Spawn(GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/networkedobjects/ScavLunarBackpack"), characterMaster.transform.position, characterMaster.transform.rotation)));

                        if (characterMaster)
                        {

                            return;

                        }



                    }
                }
            }

            orig(self);
        }

        private void TrueSuicideOnNullVoid(On.RoR2.ArenaMissionController.orig_EndRound orig, ArenaMissionController self)
        {
            if (self.currentRound >= self.totalRoundsMax)
            {
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
                for (int j = teamMembers.Count - 1; j >= 0; j--)
                {
                    teamMembers[j].body.inventory.ResetItem(ItemIndex.ExtraLife);
                }
            }

            orig(self);
        }

        public class ShrineCanvas : MonoBehaviour
        {
            public ShrineChanceBehavior self;

            public TextMeshPro[] textMeshPros = new TextMeshPro[5];

            public void Init()
            {


                for (int i = 0; i < 5; i++)
                {
                    GameObject canvas = Instantiate(Resources.Load<GameObject>("Prefabs/CostHologramContent"), self.transform);

                    CostHologramContent content = canvas.GetComponentInChildren<CostHologramContent>();

                    content.costType = CostTypeIndex.PercentHealth;
                    content.displayValue = 69;

                    TextMeshPro targetTextMesh = content.targetTextMesh;
                    canvas.transform.position = self.symbolTransform.position;
                    canvas.transform.Translate(new Vector3(0, -2 - i, 0) + transform.forward);
                    canvas.transform.rotation = self.transform.rotation;
                    canvas.transform.Rotate(new Vector3(0, 180, 0));

                    canvas.SetActive(true);
                    textMeshPros[i] = targetTextMesh;
                    MonoBehaviour.Destroy(content);


                }

                UpdateTextMeshPros();
            }

            public void UpdateTextMeshPros()
            {
                Vector4 percents = Vector4.zero;

                float max = (self.equipmentWeight + self.failureWeight + self.tier1Weight + self.tier2Weight + self.tier3Weight) / 100;

                for (int i = 0; i < 5; i++)
                {
                    string outputText;
                    Color outputColor = Color.black;

                    float tierWeight = 0f;

                    switch (i)
                    {
                        case 0:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3Item);
                            tierWeight = self.tier3Weight;
                            break;

                        case 1:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2Item);
                            tierWeight = self.tier2Weight;
                            break;

                        case 2:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1Item);
                            tierWeight = self.tier1Weight;
                            break;

                        case 3:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Equipment);
                            tierWeight = self.equipmentWeight;
                            break;

                        case 4:
                            outputColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItemDark);
                            tierWeight = self.failureWeight;
                            break;
                    }
                    GameObject canvas = textMeshPros[i].gameObject;


                    outputText = (int)(tierWeight / max) + "%";

                    textMeshPros[i].color = outputColor;
                    textMeshPros[i].text = outputText;
                    textMeshPros[i].overflowMode = TextOverflowModes.Overflow;

                    if (i != 4)
                    {
                        percents[i] = (tierWeight / max);
                    }
                    else
                    {

                        SendUpdateShrineCanvas(self.netId, percents);

                    }
                }
            }
        }

        private void ShrineChanceInit(On.RoR2.ShrineChanceBehavior.orig_Awake orig, ShrineChanceBehavior self)
        {
            orig(self);

            self.tier1Weight *= UnityEngine.Random.Range(0.8f, 1.2f);
            self.tier2Weight *= UnityEngine.Random.Range(0.8f, 1.2f);
            self.tier3Weight *= UnityEngine.Random.Range(0.8f, 1.2f);
            self.equipmentWeight *= UnityEngine.Random.Range(0.8f, 1.2f);
            self.failureWeight *= UnityEngine.Random.Range(0.8f, 1.2f);

            self.maxPurchaseCount += UnityEngine.Random.Range(0, 2);
            self.costMultiplierPerPurchase *= UnityEngine.Random.Range(0.8f, 1.2f);

            addCanvasToShrine(self);
        }

        public static ShrineCanvas addCanvasToShrine(ShrineChanceBehavior shrine)
        {
            ShrineCanvas shrineCanvas = shrine.gameObject.AddComponent<ShrineCanvas>();
            shrineCanvas.self = shrine;
            shrineCanvas.Init();
            return shrineCanvas;
        }

        private bool ShrineChanceCanvas(On.RoR2.ShrineChanceBehavior.orig_OnSerialize orig, ShrineChanceBehavior self, NetworkWriter writer, bool forceAll)
        {
            bool temp = orig(self, writer, forceAll);


            return temp;
        }

        private void BazaarEdits(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);

            if (NetworkServer.active)
            {
                if (SceneManager.GetActiveScene().name.Contains("bazaar"))
                {
                    //                    BrokenDrone1
                    //BrokenDrone2
                    //BrokenEmergencyDrone
                    //BrokenEquipmentDrone
                    //BrokenFlameDrone
                    //BrokenMegaDrone
                    //BrokenMissileDrone
                    //BrokenTurret1
                    //CasinoChest
                    //CategoryChestDamage
                    //CategoryChestHealing
                    //CategoryChestUtility
                    //Chest1
                    //Chest1Stealthed
                    //Chest2
                    //Duplicator
                    //DuplicatorLarge
                    //DuplicatorMilitary
                    //DuplicatorWild
                    //EquipmentBarrel
                    //GoldChest
                    //GoldshoresBeacon
                    //GoldshoresPortal
                    //Lockbox
                    //LunarChest
                    //MSPortal
                    //RadarTower
                    //ScavBackpack
                    //ScavLunarBackpack
                    //ShopPortal
                    //ShrineBlood
                    //ShrineBoss
                    //ShrineChance
                    //ShrineCleanse
                    //ShrineCombat
                    //ShrineGoldshoresAccess
                    //ShrineHealing
                    //ShrineRestack
                    //Teleporter
                    //TripleShop
                    //TripleShopLarge



                    {
                        SpawnCard spawnCard = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldChest");
                        GameObject gameObject = spawnCard.DoSpawn(new Vector3(-87.1f, -24f, -7.2f), Quaternion.identity, null);

                        gameObject.GetComponent<PurchaseInteraction>().Networkcost = 1;
                        gameObject.GetComponent<PurchaseInteraction>().costType = CostTypeIndex.VolatileBattery;
                        gameObject.GetComponent<ChestBehavior>().tier1Chance = 0f;
                        gameObject.GetComponent<ChestBehavior>().tier2Chance = 0f;
                        gameObject.GetComponent<ChestBehavior>().tier3Chance = 1f;
                        gameObject.GetComponent<ChestBehavior>().lunarChance = 0f;


                        NetworkServer.Spawn(gameObject);

                    }

                    
                    {
                        SpawnCard spawnCard = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineCleanse");
                        GameObject gameObject = spawnCard.DoSpawn(new Vector3(-65.7f, -24f, -18.9f), Quaternion.identity, null);

                        NetworkServer.Spawn(gameObject);
                    }

                    {
                        SpawnCard spawnCard = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineChance");
                        GameObject gameObject = spawnCard.DoSpawn(new Vector3(-127.0f, -25.0f, -31.5f), Quaternion.identity, null);

                        ShrineChanceBehavior shrineChanceBehavior = gameObject.GetComponent<ShrineChanceBehavior>();

                        shrineChanceBehavior.costMultiplierPerPurchase = 1f;
                        shrineChanceBehavior.maxPurchaseCount = int.MaxValue;
                        shrineChanceBehavior.GetComponent<PurchaseInteraction>().cost = 1;
                        shrineChanceBehavior.GetComponent<PurchaseInteraction>().Networkcost = 1;
                        shrineChanceBehavior.GetComponent<PurchaseInteraction>().costType = CostTypeIndex.LunarCoin;
                        NetworkServer.Spawn(gameObject);
                    }
                    //(-65.7, -21.8, -18.9)(0.0, 0.0, 0.0, 1.0) for cleanser
                    // chance shrine (-120.5, -23.1, -6.5)

                    //triple shops

                    //                [Info: Unity Log] (-138.7, -24.5, -14.8) (0.0, 0.0, 0.0, 1.0)
                    //[Info: Unity Log] (-136.6, -24.3, -18.4) (0.0, 0.0, 0.0, 1.0)
                    //[Info: Unity Log] (-134.2, -24.4, -21.9) (0.0, 0.0, 0.0, 1.0)

                    //(-74.8, -24.8, -42.4) (0.0, 0.0, 0.0, 1.0) scavenger sack maybe

                }
            }
        }

        private void MultishopNoDuplicates(On.RoR2.MultiShopController.orig_CreateTerminals orig, MultiShopController self)
        {
            orig(self);


            List<ShopTerminalBehavior> terminalBehaviors = new List<ShopTerminalBehavior>();
            foreach (GameObject gameObject in self.GetFieldValue<GameObject[]>("terminalGameObjects"))
            {
                if (gameObject.GetComponent<ShopTerminalBehavior>())
                {
                    terminalBehaviors.Add(gameObject.GetComponent<ShopTerminalBehavior>());
                }
            }

            List<PickupIndex> pickups = new List<PickupIndex>();

            foreach (ShopTerminalBehavior shopTerminalBehavior in terminalBehaviors)
            {
                pickups.Add(shopTerminalBehavior.NetworkpickupIndex);
            }

            foreach (ShopTerminalBehavior shopTerminalBehavior in terminalBehaviors)
            {
                if (pickups.FindAll(x => x == shopTerminalBehavior.NetworkpickupIndex).Count > 1)
                {
                    switch (self.itemTier)
                    {
                        case ItemTier.Tier1:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableTier1DropList));
                            break;
                        case ItemTier.Tier2:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableTier2DropList));
                            break;
                        case ItemTier.Tier3:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableTier3DropList));
                            break;
                        case ItemTier.Lunar:
                            shopTerminalBehavior.SetPickupIndex(Run.instance.treasureRng.NextElementUniform<PickupIndex>(Run.instance.availableLunarDropList));
                            break;
                    }

                }
            }
        }

        private void IncreaseDeathRewards(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (Util.CheckRoll(0.5f, damageReport.attackerMaster) && damageReport.victimBody && damageReport.victimBody.isElite)
            {
                PickupDropletController.CreatePickupDroplet(new PickupIndex(damageReport.victimBody.inventory.currentEquipmentIndex), damageReport.victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f * 2f);
            }
            if (damageReport.victimIsBoss && damageReport.victim.body.isChampion)
            {
                if (Util.CheckRoll(25f, damageReport.attackerMaster) && damageReport.victimBody && damageReport.victimBody.isElite)
                {
                    PickupDropletController.CreatePickupDroplet(new PickupIndex(damageReport.victimBody.inventory.currentEquipmentIndex), damageReport.victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f * 2f);
                }
            }
        }

        private CharacterMaster OwnershipEliteInheritance(On.RoR2.SummonMasterBehavior.orig_OpenSummonReturnMaster orig, SummonMasterBehavior self, Interactor activator)
        {
            CharacterMaster characterMaster = orig(self, activator);
            characterMaster.inventory.CopyEquipmentFrom(activator.GetComponent<CharacterBody>().inventory);
            characterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);
            return characterMaster;
        }

        private string AdditionalEliteTypeNames(On.RoR2.Util.orig_GetBestBodyName orig, GameObject bodyObject)
        {
            string original = orig(bodyObject);
            if (bodyObject)
            {
                if (bodyObject.GetComponent<CharacterBody>())
                {
                    if (bodyObject.GetComponent<CharacterBody>().inventory.currentEquipmentIndex == EquipmentIndex.AffixGold)
                    {
                        original = "Gilded " + original;
                    }
                    else if (bodyObject.GetComponent<CharacterBody>().inventory.currentEquipmentIndex == EquipmentIndex.AffixYellow)
                    {
                        original = "Supreme " + original;
                    }
                }
            }

            return original;
        }

        private void TurretEliteInheritance(On.RoR2.CharacterMaster.orig_AddDeployable orig, CharacterMaster self, Deployable deployable, DeployableSlot slot)
        {

            orig(self, deployable, slot);

            if (deployable.gameObject.GetComponentInChildren<CharacterMaster>())
            {
                deployable.gameObject.GetComponentInChildren<CharacterMaster>().inventory.CopyEquipmentFrom(self.inventory);
                deployable.gameObject.GetComponentInChildren<CharacterMaster>().inventory.ResetItem(ItemIndex.AutoCastEquipment);
                deployable.gameObject.GetComponentInChildren<CharacterMaster>().inventory.ResetItem(ItemIndex.TonicAffliction);

            }
            else if (deployable.gameObject.GetComponentInParent<CharacterMaster>())
            {
                deployable.gameObject.GetComponentInParent<CharacterMaster>().inventory.CopyEquipmentFrom(self.inventory);
                deployable.gameObject.GetComponentInParent<CharacterMaster>().inventory.ResetItem(ItemIndex.AutoCastEquipment);
                deployable.gameObject.GetComponentInParent<CharacterMaster>().inventory.ResetItem(ItemIndex.TonicAffliction);
            }
        }

        private bool CustomEquipment(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            CharacterBody characterBody = self.characterBody;
            string characterMaster = "";

            bool temp = orig(self, equipmentIndex);

            if (self.characterBody.inputBank.activateEquipment.justPressed)
            {
                switch (equipmentIndex)
                {
                    case EquipmentIndex.AffixWhite:
                        {
                            characterMaster = "BisonMaster";

                            temp = true;
                        }

                        break;
                    case EquipmentIndex.AffixGold:
                        {
                            characterMaster = "TitanGoldMaster";

                            temp = true;
                        }
                        break;
                    case EquipmentIndex.AffixHaunted:
                        {
                            characterMaster = "BellMaster";

                            temp = true;
                        }
                        break;
                    case EquipmentIndex.AffixPoison:
                        {
                            characterMaster = "ImpMaster";

                            temp = true;
                        }
                        break;
                    case EquipmentIndex.AffixRed:
                        {
                            characterMaster = "LemurianMaster";

                        }
                        break;
                    case EquipmentIndex.AffixBlue:
                        {
                            characterMaster = "WispMaster";

                        }
                        break;
                    case EquipmentIndex.AffixYellow:
                        {
                            characterMaster = "ElectricWormMaster";
                            temp = true;
                        }
                        break;
                }

                if (characterMaster != "")
                {
                    CharacterMaster newcharacterMaster = new MasterSummon
                    {
                        masterPrefab = MasterCatalog.FindMasterPrefab(characterMaster),
                        position = characterBody.footPosition + characterBody.transform.up,
                        rotation = characterBody.transform.rotation,
                        summonerBodyObject = null,
                        ignoreTeamMemberLimit = true,
                        teamIndexOverride = characterBody.teamComponent.teamIndex

                    }.Perform();

                    newcharacterMaster.inventory.CopyItemsFrom(characterBody.inventory);
                    newcharacterMaster.inventory.ResetItem(ItemIndex.AutoCastEquipment);
                    newcharacterMaster.inventory.CopyEquipmentFrom(characterBody.inventory);
                    newcharacterMaster.GetBody().modelLocator.modelBaseTransform.localScale *= 0.25f;

                    Deployable deployable = newcharacterMaster.gameObject.GetComponent<Deployable>();
                    if (!deployable)
                    {
                        deployable = newcharacterMaster.gameObject.AddComponent<Deployable>();
                    }


                    int deployableCount = characterBody.master.GetDeployableCount((DeployableSlot)7);

                    self.characterBody.master.AddDeployable(deployable, (DeployableSlot)7);
                    deployable.onUndeploy = new UnityEngine.Events.UnityEvent();
                    deployable.onUndeploy.AddListener(() => newcharacterMaster.TrueKill());


                }
            }
            return temp;
        }

        private void ItemDropChanges(On.RoR2.GenericPickupController.orig_Start orig, GenericPickupController self)
        {
            orig(self);

            self.pickupDisplay.verticalWave.amplitude *= 5f;
        }

        private void RunEdits(On.RoR2.Run.orig_Awake orig, Run self)
        {
            orig(self);
        }

        private void ScavengerNoResetGhost(On.RoR2.ScavengerItemGranter.orig_Start orig, ScavengerItemGranter self)
        {
            if (base.GetComponent<Inventory>())
            {
                if (base.GetComponent<Inventory>().GetItemCount(ItemIndex.Ghost) > 0)
                {
                }
                else
                {
                    orig(self);
                }
            }
        }

        private void NoAutoPickup(On.RoR2.GenericPickupController.orig_OnTriggerStay orig, GenericPickupController self, UnityEngine.Collider other)
        {
            if (PlayerCharacterMasterController.instances.Count() == 1)
            {
                orig(self, other);
            }
        }

        private void SizeTweaks(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {

            orig(self, body);

            if (body.isElite)
            {
                body.modelLocator.modelTransform.localScale *= 1.2f;
            }
            if (body.isChampion)
            {

                body.modelLocator.modelTransform.localScale *= 1.4f;
            }
            if (body.isBoss)
            {
                body.modelLocator.modelTransform.localScale *= 1.3f;
            }

        }

        private void ShrineRestackBehaviours(On.RoR2.ShrineRestackBehavior.orig_Start orig, ShrineRestackBehavior self)
        {
            orig(self);

            self.costMultiplierPerPurchase = 1f;
            self.GetComponent<PurchaseInteraction>().cost = 1;
            self.maxPurchaseCount = 4;

        }

        private void ShrineChanceReturns(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            orig(self, activator);

            self.failureWeight *= 0.8f;
            self.equipmentWeight *= 1f;
            self.tier1Weight *= 1.2f;
            self.tier2Weight *= 1.4f;
            self.tier3Weight *= 1.6f;

            self.GetComponent<ShrineCanvas>().UpdateTextMeshPros();

        }

        public const Int16 HandleId = 88;
        class UpdateShrineCanvas : MessageBase
        {
            public NetworkInstanceId objectID;
            public Vector4 percents;

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(objectID);
                writer.Write(percents);
            }

            public override void Deserialize(NetworkReader reader)
            {
                objectID = reader.ReadNetworkId();
                percents = reader.ReadVector4();
            }
        }
        static void SendUpdateShrineCanvas(NetworkInstanceId shrineID, Vector4 readPercents)
        {
            NetworkServer.SendToAll(HandleId, new UpdateShrineCanvas
            {

                objectID = shrineID,
                percents = readPercents
            });
        }

        [RoR2.Networking.NetworkMessageHandler(msgType = HandleId, client = true)]
        static void HandleDropItem(NetworkMessage netMsg)
        {
            var dropItem = netMsg.ReadMessage<UpdateShrineCanvas>();
            var percents = dropItem.percents;

            if (!dropItem.objectID.IsEmpty())
            {
                GameObject obj = ClientScene.FindLocalObject(dropItem.objectID);
                {
                    if (obj.GetComponent<ShrineChanceBehavior>())
                    {
                        if (!obj.GetComponent<ShrineCanvas>())
                        {
                            addCanvasToShrine(obj.GetComponent<ShrineChanceBehavior>());
                        }
                        else
                        {
                            ShrineCanvas canvas = obj.GetComponent<ShrineCanvas>();
                            canvas.self.tier3Weight = percents.x;
                            canvas.self.tier2Weight = percents.y;
                            canvas.self.tier1Weight = percents.z;
                            canvas.self.equipmentWeight = percents.w;
                            canvas.self.failureWeight = 100 - (percents.x + percents.y + percents.z + percents.w);
                            if (!NetworkServer.active)
                            {
                                canvas.UpdateTextMeshPros();
                            }
                        }
                    }

                }
            }
        }
    }
}

