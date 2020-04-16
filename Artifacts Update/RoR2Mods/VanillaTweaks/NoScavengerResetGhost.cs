using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VanillaTweaks
{
    public static class NoScavengerResetGhost
    {
        public static bool Init()
        {
            On.RoR2.ScavengerItemGranter.Start += (orig, self) =>
            {
                if (self.GetComponent<Inventory>())
                {
                    if (self.GetComponent<Inventory>().GetItemCount(ItemIndex.Ghost) > 0)
                    {
                    }
                    else
                    {
                        orig(self);
                    }
                }
            };

            return true;
        }
    }
}
