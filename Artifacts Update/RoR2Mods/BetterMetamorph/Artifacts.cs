using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace PallesenProductions
{

    //Artifact of Cruelty
    //Enemies can have multiple elite types
    //Artifact of Pride
    //Higher level elites can always spawn, regardless of stages cleared.
    //Artifact of Affinity
    //All enemies will be one of two types. (Stacks with artifact of kin
    //Artifact of Growth
    //Enemies accumulate items with difficulty level.
    //Artifact of The Mountain
    //Each stage has an extra challenge of the mountain
    //Artifact of Gambling
    //Chance shrines have increasing returns when used, and randomized values.
    //Artifact of Capitalism
    //Chance shrines have no maximum limit
    //Artifact of Order
    //Upon picking up an item, you are ordered.
    //Artifact of Bewilderment
    //Upon entering a level, all your items are replaced with another item of the same tier and type.
    //Artifact of Fractures
    //Enemies deal 500% damage but have 10% health
    //Artifact of Journey
    //The next stage is always random, and can be any stage.

    //Artifact of Magnesis
    //All projectiles have homing
    //Artifact of Sand
    //Enemies deal 50% Damage and have 200% health
    //Artifact of Void
    //The air hurts.There are safe pockets.
    //Artifact of Mayhem
    //All damage is increased by 10% for each active artifact
    //Artifact of Greed
    //Double the interactables, double the enemies.
    //Artifact of Plenty
    //Double the interactables, and their costs.
    //Artifact of Profit
    //when an item spawns, it spawns an extra item for each stage you've cleared.
    //Artifact of Discombobulate
    //At the start of each stage, randomize your artifacts.

    public class Artifacts
    {
        public static CombatDirector.EliteTierDef[] eliteDefs = typeof(CombatDirector).GetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers");

        public struct artifactStruct
        {
            public ArtifactDef regDef { get; set; }
            public ArtifactIndex regIndex { get => regDef.artifactIndex; }

            public Func<bool> InitFunc;
        }


        public static List<artifactStruct> artifactStructs = new List<artifactStruct>();


        public static ArtifactDef Transmutation = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Dissidence = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Befuddle = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Cruelty = ScriptableObject.CreateInstance<ArtifactDef>();

        public static void Init()
        {
            FloodWarning.instance.Log("Initializing artifacts");

            Transmutation.nameToken = "Artifact of Transmutation";
            Transmutation.descriptionToken = "Get a randomized loadout every level";
            Transmutation.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.white, Color.white, Color.white, Color.white);
            Transmutation.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.gray, Color.white, Color.white, Color.white);
            artifactStructs.Add(new artifactStruct() { regDef = Transmutation, InitFunc = TransmutationInit });

            Dissidence.nameToken = "Artifact of Dissidence";
            Dissidence.descriptionToken = "All enemies can be elite enemies or bosses, even when they normally couldn't be.";
            Dissidence.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.white, Color.white, Color.white, Color.white);
            Dissidence.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.gray, Color.white, Color.white, Color.white);
            artifactStructs.Add(new artifactStruct() { regDef = Dissidence, InitFunc = DissidenceInit });


            Befuddle.nameToken = "Artifact of Befuddle";
            Befuddle.descriptionToken = "All items are in a random tier.";
            Befuddle.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.white, Color.white, Color.white, Color.white);
            Befuddle.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.gray, Color.white, Color.white, Color.white);
            artifactStructs.Add(new artifactStruct() { regDef = Befuddle, InitFunc = BefuddleInit });

            Cruelty.nameToken = "Artifact of Cruelty";
            Cruelty.descriptionToken = "Enemies can have multiple elite types.";
            Cruelty.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.white, Color.white, Color.white, Color.white);
            Cruelty.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.gray, Color.white, Color.white, Color.white);
            artifactStructs.Add(new artifactStruct() { regDef = Cruelty, InitFunc = CrueltyInit });

            ArtifactCatalog.getAdditionalEntries += (list) =>
            {
                FloodWarning.instance.Log("Injecting artifacts into ArtifactCatalog...");

                foreach (artifactStruct artifactStruct in artifactStructs)
                {
                    FloodWarning.instance.Log("Adding artifact : " + artifactStruct.regDef.nameToken);

                    list.Add(artifactStruct.regDef);
                    artifactStruct.InitFunc.Invoke();
                }
            };

        }

        public static bool TransmutationInit()
        {
            On.RoR2.CharacterMaster.Respawn += (orig, self, footPosition, rotation, tryToGroundSafely) =>
            {
                CharacterBody characterBody = orig(self, footPosition, rotation, tryToGroundSafely);

                if (characterBody.isPlayerControlled && NetworkServer.active)
                {
                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Transmutation.artifactIndex))
                    {
                        FloodWarning.instance.Log("Transmutation begun ");

                        foreach (GenericSkill skill in self.GetBodyObject().GetComponentsInChildren<GenericSkill>())
                        {
                            FloodWarning.instance.Log("Transmuting genericskill " + skill.skillNameToken);
                            int skillslot = (int)characterBody.skillLocator.FindSkillSlot(skill);
                            FloodWarning.instance.Log("skillslot is " + skillslot);
                            uint randomChoice = (uint)Random.Range(0, skill.skillFamily.variants.Length);
                            FloodWarning.instance.Log("Chose variant " + randomChoice);
                            if (skillslot >= 0)
                                self.loadout.bodyLoadoutManager.SetSkillVariant(characterBody.bodyIndex, skillslot, randomChoice);
                        }
                        self.loadout.bodyLoadoutManager.SetSkinIndex(characterBody.bodyIndex, (uint)Random.Range(0, SkinCatalog.GetBodySkinCount(characterBody.bodyIndex)));
                    }
                }

                return characterBody;
            };

            return true;
        }

        private static List<CharacterSpawnCard> allSpawnCards = new List<CharacterSpawnCard>();
        public static bool DissidenceInit()
        {
            On.RoR2.CharacterSpawnCard.Awake += (orig, self) =>
            {
                //allSpawnCards.Add(self);
                orig(self);
            };

            On.RoR2.CharacterSpawnCard.Spawn += CharacterSpawnCard_Spawn;
            On.RoR2.CharacterMaster.Awake += (orig, self) =>
            {
                orig(self);
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Dissidence.artifactIndex))
                {
                    self.onBodyStart += (body) =>
                    {
                        //if (!body.isElite || RunArtifactManager.instance.IsArtifactEnabled(Cruelty.artifactIndex))
                        {
                            //CharacterSpawnCard masterToCompare = allSpawnCards.FirstOrDefault(x => x.prefab.GetComponentInChildren<CharacterMaster>().masterIndex == body.master.masterIndex);

                            if (!body.isPlayerControlled)
                            {
                                float tempCredit = Run.instance.compensatedDifficultyCoefficient * Run.instance.livingPlayerCount * 80f;
                                AddEliteTypes(body, tempCredit, (int)Mathf.Sqrt((int)body.baseMaxHealth * (int)body.baseDamage), !RunArtifactManager.instance.IsArtifactEnabled(Cruelty.artifactIndex));
                            }
                        }
                    };
                }
            };
            return true;
        }

        private static void CharacterSpawnCard_Spawn(On.RoR2.CharacterSpawnCard.orig_Spawn orig, CharacterSpawnCard self, Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest, ref SpawnCard.SpawnResult result)
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Dissidence.artifactIndex))
            {
                self.forbiddenAsBoss = false;
                self.noElites = false;

            }

            orig(self, position, rotation, directorSpawnRequest, ref result);
        }


        private static Dictionary<ItemIndex, ItemTier> BefuddleCache;
        public static bool BefuddleInit()
        {

            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);

                if (BefuddleCache == null)
                {
                    BefuddleCache = new Dictionary<ItemIndex, ItemTier>();

                    ItemIndex itemIndex = 0;
                    ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;

                    while (itemIndex < itemCount)
                    {
                        BefuddleCache.Add(itemIndex, ItemCatalog.GetItemDef(itemIndex).tier);
                        itemIndex++;

                    }
                }

                if (NetworkServer.active)
                {

                    Random.InitState(Run.instance.treasureRng.nextInt);

                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Befuddle.artifactIndex))
                    {
                        ItemIndex itemIndex = 0;
                        ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;

                        while (itemIndex < itemCount)
                        {
                            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);

                            if (itemDef.inDroppableTier)
                            {
                                itemDef.tier = (ItemTier)Random.Range(0, 4);
                            }
                            itemIndex++;
                        }
                    }
                    else
                    {
                        ItemIndex itemIndex = 0;
                        ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;

                        while (itemIndex < itemCount)
                        {
                            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);

                            itemDef.tier = BefuddleCache[itemIndex];

                            itemIndex++;
                        }
                    }
                }

                Run.instance.BuildDropTable();

                typeof(PickupTransmutationManager).GetMethod("RebuildPickupGroups", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[0]);


            };

            return true;
        }

        public static bool CrueltyInit()
        {
            On.RoR2.CombatDirector.Awake += (orig, self) =>
            {

                orig(self);

                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Cruelty.artifactIndex))
                {
                    self.onSpawnedServer.AddListener((obj) =>
                    {
                        if (!NetworkServer.active)
                        {
                            return;
                        }

                        DirectorCard card = self.lastAttemptedMonsterCard;

                        float credit = self.monsterCredit;

                        CharacterBody characterBody = obj.GetComponentInChildren<CharacterMaster>().GetBody();

                        self.monsterCredit = AddEliteTypes(characterBody, credit, card.cost, false);

                    }
                    );
                }
            };


            return true;
        }

        public static float AddEliteTypes(CharacterBody characterBody, float credit, int cardCost, bool forceSingleType = true)
        {
            int currentEliteTypes = 0;

            foreach (BuffIndex buff in BuffCatalog.eliteBuffIndices)
            {
                BuffDef buffDef = BuffCatalog.GetBuffDef(buff);
                if (characterBody.HasBuff(buff))
                {
                    currentEliteTypes += 1;
                }
            }

            //FloodWarning.instance.Log("AddEliteTypes called. has " + currentEliteTypes + " elite types.");


            if (currentEliteTypes >= 1 && forceSingleType) { return 0; }



            foreach (CombatDirector.EliteTierDef tierDef in eliteDefs.OrderBy(x => x.costMultiplier))
            {

                foreach (EliteIndex eliteIndex in tierDef.eliteTypes)
                {

                    if (characterBody.baseNameToken == "URCHINTURRET_BODY_NAME" && eliteIndex == EliteIndex.Poison) { continue; }
                    //FloodWarning.instance.Log("Testing against " + eliteIndex.ToString() + ". " + credit + " credit available. Card cost is " + cardCost);

                    //FloodWarning.instance.Log(credit  + " > " + cardCost * tierDef.costMultiplier * (currentEliteTypes + 1) + " -> " + (credit > cardCost * tierDef.costMultiplier * (currentEliteTypes + 1)));

                    //Can spawn this elite, multiplying by the number of already held elite types + 1.
                    if (credit > cardCost * tierDef.costMultiplier * (currentEliteTypes + 1))
                    {

                        credit -= cardCost * tierDef.costMultiplier;

                        BuffDef buffDef = BuffCatalog.GetBuffDef(BuffIndex.None);

                        //Finding the correct buff tied to this elite type
                        foreach (BuffIndex buff in BuffCatalog.eliteBuffIndices)
                        {
                            BuffDef tempDef = BuffCatalog.GetBuffDef(buff);

                            if (tempDef.eliteIndex == eliteIndex)
                            {
                                buffDef = tempDef;
                            }
                        }

                        //FloodWarning.instance.Log("New buffindex is " + buffDef.buffIndex.ToString());

                        if (buffDef == BuffCatalog.GetBuffDef(BuffIndex.None))
                        {
                            continue;
                        }


                        currentEliteTypes++;
                        credit -= cardCost * tierDef.costMultiplier * Mathf.Min(1f, currentEliteTypes / 3f);

                        characterBody.AddBuff(buffDef.buffIndex);

                        float num3 = tierDef.healthBoostCoefficient;
                        float damageBoostCoefficient = tierDef.damageBoostCoefficient;

                        int livingPlayerCount = Run.instance.livingPlayerCount;
                        num3 *= Mathf.Pow((float)livingPlayerCount, 1f);


                        characterBody.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
                        characterBody.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((damageBoostCoefficient - 1f) * 10f));

                        EliteDef eliteDef = EliteCatalog.GetEliteDef(eliteIndex);
                        EquipmentIndex equipmentIndex = (eliteDef != null) ? eliteDef.eliteEquipmentIndex : EquipmentIndex.None;

                        if (equipmentIndex != EquipmentIndex.None)
                        {
                            if (characterBody.inventory.GetEquipmentIndex() == EquipmentIndex.None)
                            {
                                characterBody.inventory.SetEquipmentIndex(equipmentIndex);
                            }
                        }

                        //FloodWarning.instance.Log("Added an elite type");

                        if (currentEliteTypes >= 1 && forceSingleType) { return credit; }
                    }
                }
            }

            return credit;
        }
    }

}