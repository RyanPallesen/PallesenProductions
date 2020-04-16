using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VanillaTweaks
{
    public static class NoAutoPickup
    {
        public static bool Init()
        {
            On.RoR2.GenericPickupController.OnTriggerStay += (orig, self, other) =>
            {
                {
                    if (PlayerCharacterMasterController.instances.Count() == 1)
                    {
                        orig(self, other);
                    }
                }
            };

            return true;
        }
    }
}
