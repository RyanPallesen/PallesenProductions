using System;
using System.Collections.Generic;
using System.Text;

namespace VanillaTweaks
{
    static class ChanceShrineChanges
    {
        public static bool Init()
        {
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);

                self.tier1Weight *= UnityEngine.Random.Range(0.8f, 1.2f);
                self.tier2Weight *= UnityEngine.Random.Range(0.8f, 1.2f);
                self.tier3Weight *= UnityEngine.Random.Range(0.8f, 1.2f);
                self.equipmentWeight *= UnityEngine.Random.Range(0.8f, 1.2f);
                self.failureWeight *= UnityEngine.Random.Range(0.8f, 1.2f);

                self.maxPurchaseCount += UnityEngine.Random.Range(0, 2);
                self.costMultiplierPerPurchase *= UnityEngine.Random.Range(0.8f, 1.2f);
            };

            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, activator) =>
            {
                orig(self, activator);

                self.failureWeight *= 0.8f;
                self.equipmentWeight *= 1f;
                self.tier1Weight *= 1.2f;
                self.tier2Weight *= 1.4f;
                self.tier3Weight *= 1.6f;
            };

            return true;
        }
    }
}
