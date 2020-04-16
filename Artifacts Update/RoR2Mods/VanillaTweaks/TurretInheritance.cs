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
            On.RoR2.CharacterMaster.OnBodyStart += (orig, self, body) =>
            {
                orig(self, body);


                foreach (BuffIndex buffIndex in BuffCatalog.eliteBuffIndices)
                {
                    if (body.HasBuff(buffIndex))
                    {
                        body.modelLocator.modelBaseTransform.localScale *= 1.2f;
                    }
                }

                if (body.isChampion)
                {

                    body.modelLocator.modelBaseTransform.localScale *= 2f;
                }

            };


            return true;
        }
    }
}
