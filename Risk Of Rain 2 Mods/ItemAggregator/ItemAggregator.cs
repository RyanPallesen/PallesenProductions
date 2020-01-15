using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using On.RoR2.ConVar;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using static R2API.LobbyConfigAPI;
using static R2API.SkinAPI;

namespace PallesenProductions
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.PallesenProductions.ItemAggregator.1.0.0", "ItemAggregator", "1.0.0")]
    [R2APISubmoduleDependency("LobbyConfigAPI")]
    class NullRain : BaseUnityPlugin
    {
        private static NullRain _instance;

        public static NullRain Instance { get { return _instance; } }

        int levelWave = 1;

        public Inventory inventory;
        public ItemInventoryDisplay itemInventoryDisplay;

        public int ruleValue;

        LobbyCategory lobbyCategory = new LobbyCategory("Item Aggregation", Color.magenta, "Item Aggregation, like in the void fields");

        RuleCategoryDef ruleCategoryDef = new RuleCategoryDef();

        LobbyRule<int> lobbyRule = new LobbyRule<int>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            lobbyRule.AddChoice(0, "None", "Enemies aggregate no items", ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1Item), ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1ItemDark));
            lobbyRule.AddChoice(1, "White", "Enemies aggregate white items every level", ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1Item), ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier1ItemDark));
            lobbyRule.AddChoice(2, "Green", "Enemies also aggregate green items, every 3 levels", ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2Item), ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2ItemDark));
            lobbyRule.AddChoice(3, "Red", "Enemies also aggregate red items, every 5 levels", ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3Item), ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3ItemDark));
            lobbyRule.AddChoice(4, "Lunar", "Enemies also aggregate lunar items, every 8 levels", ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem), ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItemDark));
            lobbyRule.AddChoice(5, "Boss", "Enemies also aggregate boss items, every 10 levels", ColorCatalog.GetColor(ColorCatalog.ColorIndex.BossItem), ColorCatalog.GetColor(ColorCatalog.ColorIndex.BossItemDark));

            lobbyRule.MakeValueDefault(0);

            lobbyCategory.PushRule<int>(lobbyRule);

            On.RoR2.UI.HUD.Awake += (orig, self) =>
            {
                orig(self);


                itemInventoryDisplay = Instantiate(self.itemInventoryDisplay, self.transform);
                itemInventoryDisplay.SetFieldValue<int[]>("itemStacks", ItemCatalog.RequestItemStackArray());
                itemInventoryDisplay.SetFieldValue<ItemIndex[]>("itemOrder", ItemCatalog.RequestItemOrderBuffer());

                if (!inventory)
                {
                    inventory = itemInventoryDisplay.gameObject.AddComponent<Inventory>();
                }

                itemInventoryDisplay.SetSubscribedInventory(inventory);
            };

            RoR2.Run.onRunStartGlobal += (RoR2.Run run) =>
            {
                if (NetworkServer.active && lobbyRule.Value != null)
                {
                    levelWave = 1;

                    Debug.Log(lobbyRule.Value);


                    if (lobbyRule.Value == 1)
                    {
                        Debug.Log("Added white");

                        On.RoR2.GlobalEventManager.OnTeamLevelUp += UpdateLevelWave;
                        On.RoR2.CharacterMaster.OnBodyStart += CopyItemsMaster;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += WhiteOnLevelUp;
                    }
                    if (lobbyRule.Value == 2)
                    {
                        Debug.Log("Added green");

                        On.RoR2.GlobalEventManager.OnTeamLevelUp += UpdateLevelWave;
                        On.RoR2.CharacterMaster.OnBodyStart += CopyItemsMaster;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += WhiteOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += GreenOnLevelUp;
                    }
                    if (lobbyRule.Value == 3)
                    {

                        Debug.Log("Added red");


                        On.RoR2.GlobalEventManager.OnTeamLevelUp += UpdateLevelWave;
                        On.RoR2.CharacterMaster.OnBodyStart += CopyItemsMaster;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += WhiteOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += GreenOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += RedOnLevelUp;
                    }
                    if (lobbyRule.Value == 4)
                    {
                        Debug.Log("Added lunar");



                        On.RoR2.GlobalEventManager.OnTeamLevelUp += UpdateLevelWave;
                        On.RoR2.CharacterMaster.OnBodyStart += CopyItemsMaster;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += WhiteOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += GreenOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += RedOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += LunarOnLevelUp;
                    }
                    if (lobbyRule.Value == 5)
                    {
                        Debug.Log("Added boss");
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += UpdateLevelWave;
                        On.RoR2.CharacterMaster.OnBodyStart += CopyItemsMaster;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += WhiteOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += GreenOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += RedOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += LunarOnLevelUp;
                        On.RoR2.GlobalEventManager.OnTeamLevelUp += BossOnLevelUp;
                    }
                }
            };

            RoR2.Run.onRunDestroyGlobal += (RoR2.Run run) =>
            {
                if (NetworkServer.active && lobbyRule.Value != null)
                {


                    levelWave = 1;

                    if (lobbyRule.Value <= 0)
                    {
                        On.RoR2.GlobalEventManager.OnTeamLevelUp -= UpdateLevelWave;
                        On.RoR2.CharacterMaster.OnBodyStart -= CopyItemsMaster;
                    }
                    if (lobbyRule.Value <= 1)
                    {
                        On.RoR2.GlobalEventManager.OnTeamLevelUp -= WhiteOnLevelUp;
                    }
                    if (lobbyRule.Value <= 2)
                    {
                        On.RoR2.GlobalEventManager.OnTeamLevelUp -= GreenOnLevelUp;
                    }
                    if (lobbyRule.Value <= 3)
                    {
                        On.RoR2.GlobalEventManager.OnTeamLevelUp -= RedOnLevelUp;
                    }
                    if (lobbyRule.Value <= 4)
                    {
                        On.RoR2.GlobalEventManager.OnTeamLevelUp -= LunarOnLevelUp;
                    }
                    if (lobbyRule.Value <= 5)
                    {
                        On.RoR2.GlobalEventManager.OnTeamLevelUp -= BossOnLevelUp;
                    }
                }
            };

        }


        private void UpdateLevelWave(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            if (teamIndex == TeamIndex.Monster)
            {
                levelWave++;
            }
        }

        private void CopyItemsMaster(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);

            if (NetworkServer.active)
            {
                if (!(self.teamIndex == TeamIndex.Player))
                {
                    if (inventory)
                    {
                        int numHealthBoost = self.inventory.GetItemCount(ItemIndex.BoostHp);
                        int numDamageBoost = self.inventory.GetItemCount(ItemIndex.BoostDamage);

                        self.inventory.CopyItemsFrom(inventory);
                        self.inventory.GiveItem(ItemIndex.BoostHp, numHealthBoost);
                        self.inventory.GiveItem(ItemIndex.BoostDamage, numDamageBoost);
                    }
                }
            }

        }

        private void getNonForbiddenItem(ItemTier tier)
        {
            if (NetworkServer.active && inventory && Run.instance)
            {

                List<PickupIndex> items;

                switch (tier)
                {
                    case ItemTier.Tier1:
                        items = Run.instance.availableTier1DropList;
                        break;
                    case ItemTier.Tier2:
                        items = Run.instance.availableTier2DropList;
                        break;
                    case ItemTier.Tier3:
                        items = Run.instance.availableTier3DropList;
                        break;
                    case ItemTier.Lunar:
                        items = Run.instance.availableLunarDropList;
                        break;
                    case ItemTier.Boss:
                        items = Run.instance.availableBossDropList;
                        break;
                    case ItemTier.NoTier:
                        items = Run.instance.availableTier1DropList;
                        break;
                    default:
                        items = Run.instance.availableTier1DropList;
                        break;
                }

                ItemTag[] forbiddenTags = new ItemTag[]
                 {
            ItemTag.AIBlacklist,
            ItemTag.EquipmentRelated,
            ItemTag.SprintRelated,
            ItemTag.OnKillEffect,
                };

                ItemIndex[] forbiddenItems = new ItemIndex[]
                {
                    ItemIndex.ExtraLife,
                    ItemIndex.ShockNearby,
                    ItemIndex.BeetleGland
                };

                List<ItemIndex> allowedItems = new List<ItemIndex>();



                foreach (PickupIndex pickupIndex in items)
                {
                    bool isAllowed = true;

                    PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

                    Debug.Log(pickupIndex.itemIndex + " | " + pickupIndex.equipmentIndex);

                    if (pickupIndex.itemIndex != ItemIndex.None)
                    {
                        ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);

                        for (int i = 0; i < forbiddenTags.Length; i++)
                        {
                            if (itemDef.ContainsTag(forbiddenTags[i]))
                            {
                                isAllowed = false;
                            }
                        }

                        for (int i = 0; i < forbiddenItems.Length; i++)
                        {
                            if (itemDef.itemIndex == forbiddenItems[i])
                            {
                                isAllowed = false;
                            }
                        }

                        if (isAllowed)
                        {
                            allowedItems.Add(pickupIndex.itemIndex);
                        }
                    }
                }

                Debug.Log(allowedItems.Count);

                ItemIndex newItem = allowedItems[Run.instance.stageRng.RangeInt(0, allowedItems.Count)];

                Debug.Log("Total of " + allowedItems.Count + " items allowed in tier " + tier + ", Chose " + newItem + " Wave is " + levelWave);


                SendDropItem(inventory.gameObject, newItem);

            }
        }

        private void WhiteOnLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            orig(teamIndex);

            if (teamIndex == TeamIndex.Monster)
            {
                getNonForbiddenItem(ItemTier.Tier1);

            }

        }

        private void GreenOnLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            orig(teamIndex);

            if (levelWave % 3 == 0)
            {

                getNonForbiddenItem(ItemTier.Tier2);


            }
        }

        private void RedOnLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            orig(teamIndex);

            if (levelWave % 5 == 0)
            {

                getNonForbiddenItem(ItemTier.Tier3);

            }
        }

        private void LunarOnLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            orig(teamIndex);

            if (levelWave % 8 == 0)
            {
                if (teamIndex == TeamIndex.Monster)
                {
                    getNonForbiddenItem(ItemTier.Lunar);

                }
            }
        }

        private void BossOnLevelUp(On.RoR2.GlobalEventManager.orig_OnTeamLevelUp orig, TeamIndex teamIndex)
        {
            orig(teamIndex);

            if (levelWave % 10 == 0)
            {
                if (teamIndex == TeamIndex.Monster)
                {

                    getNonForbiddenItem(ItemTier.Boss);

                }
            }
        }


        public const Int16 HandleId = 72;
        class AddItem : MessageBase
        {
            public GameObject Player;
            public RoR2.ItemIndex ItemIndex;
            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(Player);
                writer.Write((UInt16)ItemIndex);
            }

            public override void Deserialize(NetworkReader reader)
            {
                Player = reader.ReadGameObject();
                ItemIndex = (RoR2.ItemIndex)reader.ReadUInt16();
            }
        }
        static void SendDropItem(GameObject player, RoR2.ItemIndex itemIndex)
        {
            NetworkServer.SendToAll(HandleId, new AddItem
            {
                Player = player,
                ItemIndex = itemIndex
            });

        }
        [RoR2.Networking.NetworkMessageHandler(msgType = HandleId, client = true)]
        static void HandleDropItem(NetworkMessage netMsg)
        {

            var dropItem = netMsg.ReadMessage<AddItem>();
            var Item = dropItem.ItemIndex;

            int[] _itemStacks = NullRain.Instance.inventory.GetFieldValue<int[]>("itemStacks");
            List<ItemIndex> _itemAcquisitionOrder = NullRain.Instance.inventory.GetFieldValue<List<ItemIndex>>("itemAcquisitionOrder");

            _itemStacks[(int)Item] += 1;

            if (!(_itemAcquisitionOrder.Contains(Item)))
            {
                _itemAcquisitionOrder.Add(Item);

            }

            NullRain.Instance.inventory.SetFieldValue<int[]>("itemStacks", _itemStacks);
            NullRain.Instance.inventory.SetFieldValue<List<ItemIndex>>("itemAcquisitionOrder", _itemAcquisitionOrder);

            NullRain.Instance.inventory.GetFieldValue<Action>("onInventoryChanged").Invoke();
        }

    }
}

