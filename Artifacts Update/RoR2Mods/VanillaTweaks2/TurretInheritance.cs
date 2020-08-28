using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace VanillaTweaks
{
    public static class TurretInheritance
    {
        public static bool Init()
        {
            On.RoR2.CharacterMaster.AddDeployable += (orig, self, deployable, slot) =>
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
            };

            return true;
        }
    }
}
