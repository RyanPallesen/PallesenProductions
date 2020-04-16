using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VanillaTweaks
{
    public static class MultiShopNoDuplicates
    {
        public static bool Init()
        {
            On.RoR2.MultiShopController.CreateTerminals += (orig, self) =>
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
            };


            return true;
        }
    }
}
