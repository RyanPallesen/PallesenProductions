using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VanillaTweaks
{
    static class DeathRewards
    {
        public static bool Init()
        {
            On.RoR2.GlobalEventManager.OnCharacterDeath += (orig, self, damageReport) =>
            {
                orig(self, damageReport);

                if (Util.CheckRoll(0.25f, damageReport.attackerMaster) && damageReport.victimBody && damageReport.victimBody.isElite)
                {
                    PickupDropletController.CreatePickupDroplet(new PickupIndex(damageReport.victimBody.inventory.currentEquipmentIndex), damageReport.victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f * 2f);
                }
                if (damageReport.victimIsBoss && damageReport.victim.body.isChampion)
                {
                    if (Util.CheckRoll(15f, damageReport.attackerMaster) && damageReport.victimBody && damageReport.victimBody.isElite)
                    {
                        PickupDropletController.CreatePickupDroplet(new PickupIndex(damageReport.victimBody.inventory.currentEquipmentIndex), damageReport.victimBody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f * 2f);
                    }
                }
            };









            return true;
        }
    }
}
